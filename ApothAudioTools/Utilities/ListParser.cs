using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothAudioTools.Utilities
{
    public class ListParser
    {
        public List<LinkInfo> BuildList(string linkList)
        {
            string[] stringSeparator = new string[] { "\r" };
            string[] links = linkList.Split(stringSeparator, StringSplitOptions.None);
            List<LinkInfo> cleanedLinkList = new List<LinkInfo>();

            foreach (var link in links)
            {
                cleanedLinkList.Add(MakeLinkInfo(link));
            }

            return cleanedLinkList;
        }

        public LinkInfo MakeLinkInfo(string link)
        {
            var newLinkInfo = new LinkInfo(link);
            return newLinkInfo;
        }
    }
}
