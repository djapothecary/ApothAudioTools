﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApothVidLib.Exceptions;
using ApothVidLib.Helpers;

namespace ApothVidLib
{
    public class YouTube : ServiceBase<YouTubeVideo>
    {
        private const string Playback = "videoplayback";
        private static string _signatureKey;
        public static YouTube Default { get; } = new YouTube();


        internal async override Task<IEnumerable<YouTubeVideo>> GetAllVideosAsync(
            string videoUri, Func<string, Task<string>> sourceFactory)
        {
            if (!TryNormalize(videoUri, out videoUri))
                throw new ArgumentException("URL is not a valid YouTube URL!");

            string source = await
                sourceFactory(videoUri)
                .ConfigureAwait(false);

            return ParseVideos(source);
        }

        private bool TryNormalize(string videoUri, out string normalized)
        {
            // If you fix something in here, please be sure to fix in 
            // DownloadUrlResolver.TryNormalizeYoutubeUrl as well.

            normalized = null;

            var builder = new StringBuilder(videoUri);

            videoUri = builder.Replace("youtu.be/", "youtube.com/watch?v=")
                .Replace("youtube.com/embed/", "youtube.com/watch?v=")
                .Replace("/v/", "/watch?v=")
                .Replace("/watch#", "/watch?")
                .ToString();

            var query = new Query(videoUri);

            string value;

            if (!query.TryGetValue("v", out value))
                return false;

            normalized = "https://youtube.com/watch?v=" + value;
            return true;
        }

        private IEnumerable<YouTubeVideo> ParseVideos(string source)
        {
            string title = Html.GetNode("title", source);
            IEnumerable<UnscrambledQuery> queries;
            string jsPlayer = ParseJsPlayer(source);
            if (jsPlayer == null)
            {
                yield break;
            }
            var playerResponseMap = Json.GetKey("player_response", source);
            var playerResponseJson = JToken.Parse(Regex.Unescape(playerResponseMap).Replace(@"\u0026", "&"));
            if (string.Equals(playerResponseJson.SelectToken("playabilityStatus.status")?.Value<string>(), "error", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnavailableStreamException($"Video has unavailable stream.");
            }
            var errorReason = playerResponseJson.SelectToken("playabilityStatus.reason")?.Value<string>();
            if (string.IsNullOrWhiteSpace(errorReason))
            {
                var isLiveStream = playerResponseJson.SelectToken("videoDetails.isLive")?.Value<bool>() == true;
                if (isLiveStream)
                {
                    throw new UnavailableStreamException($"This is live stream so unavailable stream.");
                }
                // url_encoded_fmt_stream_map
                string map = Json.GetKey("url_encoded_fmt_stream_map", source);
                if (!string.IsNullOrWhiteSpace(map))
                {
                    queries = map.Split(',').Select(Unscramble);
                    foreach (var query in queries)
                        yield return new YouTubeVideo(title, query, jsPlayer);
                }
                else // player_response
                {
                    List<JToken> streamObjects = new List<JToken>();
                    // Extract Muxed streams
                    var streamFormat = playerResponseJson.SelectToken("streamingData.formats");
                    if (streamFormat != null)
                    {
                        streamObjects.AddRange(streamFormat.ToArray());
                    }
                    // Extract AdaptiveFormat streams
                    var streamAdaptiveFormats = playerResponseJson.SelectToken("streamingData.adaptiveFormats");
                    if (streamAdaptiveFormats != null)
                    {
                        streamObjects.AddRange(streamAdaptiveFormats.ToArray());
                    }

                    foreach (var item in streamObjects)
                    {
                        var urlValue = item.SelectToken("url")?.Value<string>();
                        if (!string.IsNullOrEmpty(urlValue))
                        {
                            var query = new UnscrambledQuery(urlValue, false);
                            yield return new YouTubeVideo(title, query, jsPlayer);
                            continue;
                        }
                        var cipherValue = item.SelectToken("cipher")?.Value<string>();
                        if (!string.IsNullOrEmpty(cipherValue))
                        {
                            yield return new YouTubeVideo(title, Unscramble(cipherValue), jsPlayer);
                        }
                    }
                }
                // adaptive_fmts
                string adaptiveMap = Json.GetKey("adaptive_fmts", source);
                if (!string.IsNullOrWhiteSpace(adaptiveMap))
                {
                    queries = adaptiveMap.Split(',').Select(Unscramble);
                    foreach (var query in queries)
                        yield return new YouTubeVideo(title, query, jsPlayer);
                }
                else
                {
                    // dashmpd
                    string dashmpdMap = Json.GetKey("dashmpd", source);
                    if (!string.IsNullOrWhiteSpace(adaptiveMap))
                    {
                        using (HttpClient hc = new HttpClient())
                        {
                            IEnumerable<string> uris = null;
                            try
                            {

                                dashmpdMap = WebUtility.UrlDecode(dashmpdMap).Replace(@"\/", "/");

                                var manifest = hc.GetStringAsync(dashmpdMap)
                                    .GetAwaiter().GetResult()
                                    .Replace(@"\/", "/");

                                uris = Html.GetUrisFromManifest(manifest);
                            }
                            catch (Exception e)
                            {
                                throw new UnavailableStreamException(e.Message);
                            }

                            if (uris != null)
                            {
                                foreach (var v in uris)
                                {
                                    yield return new YouTubeVideo(title,
                                        UnscrambleManifestUri(v),
                                        jsPlayer);
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ParseJsPlayer(string source)
        {
            string jsPlayer = Json.GetKey("js", source).Replace(@"\/", "/");
            if (string.IsNullOrWhiteSpace(jsPlayer))
            {
                return null;
            }

            if (jsPlayer.StartsWith("/yts"))
            {
                return $"https://www.youtube.com{jsPlayer}";
            }

            // Fall back on old implementation (not sure it's needed)
            if (!jsPlayer.StartsWith("http"))
            {
                jsPlayer = $"https:{jsPlayer}";
            }

            return jsPlayer;
        }

        private UnscrambledQuery Unscramble(string queryString)
        {
            queryString = queryString.Replace(@"\u0026", "&");
            var query = new Query(queryString);
            string uri = query["url"];

            query.TryGetValue("sp", out _signatureKey);

            bool encrypted = false;
            string signature;

            if (query.TryGetValue("s", out signature))
            {
                encrypted = true;
                uri += GetSignatureAndHost(GetSignatureKey(), signature, query);
            }
            else if (query.TryGetValue("sig", out signature))
                uri += GetSignatureAndHost(GetSignatureKey(), signature, query);

            uri = WebUtility.UrlDecode(
                WebUtility.UrlDecode(uri));

            var uriQuery = new Query(uri);

            if (!uriQuery.ContainsKey("ratebypass"))
                uri += "&ratebypass=yes";

            return new UnscrambledQuery(uri, encrypted);
        }

        private string GetSignatureAndHost(string key, string signature, Query query)
        {
            string result = $"&{key}={signature}";

            string host;

            if (query.TryGetValue("fallback_host", out host))
                result += "&fallback_host=" + host;

            return result;
        }

        private UnscrambledQuery UnscrambleManifestUri(string manifestUri)
        {
            int start = manifestUri.IndexOf(Playback) + Playback.Length;
            string baseUri = manifestUri.Substring(0, start);
            string parametersString = manifestUri.Substring(start, manifestUri.Length - start);
            var parameters = parametersString.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var builder = new StringBuilder(baseUri);
            builder.Append("?");
            for (var i = 0; i < parameters.Length; i += 2)
            {
                builder.Append(parameters[i]);
                builder.Append('=');
                builder.Append(parameters[i + 1].Replace("%2F", "/"));
                if (i < parameters.Length - 2)
                {
                    builder.Append('&');
                }
            }

            return new UnscrambledQuery(builder.ToString(), false);
        }

        public static string GetSignatureKey()
        {
            return string.IsNullOrWhiteSpace(_signatureKey) ? "signature" : _signatureKey;
        }
    }
}