using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ID3Tagging.MP3Lib.MP3;
using ID3Tagging.Utils;

namespace ID3Tagging.ID3Editor
{
    /// <summary>
    /// the main presenter
    /// </summary>
    public class MainPresenter : IDisposable
    {
        #region Fields
        /// <summary>
        /// the form
        /// </summary>
        protected MainForm _form;

        private string _rootdir;

        #endregion

        #region Properties

        /// <summary>
        /// gets or set the root directory
        /// </summary>
        public string RootDir
        {
            get
            {
                return _rootdir;
            }

            set
            {
                _rootdir = value;
            }
        }

        #endregion

        #region IDisposable pattern

        /// <summary>
        /// Dispose of (clean up and deallocate) resources used by this class.
        /// </summary>
        /// <param name="fromUser">
        /// True if called directly or indirectly from user code.
        /// False if called from the finalizer (i.e. from the class' destructor).
        /// </param>
        /// <remarks>
        /// When called from user code, it is safe to clean up both managed and unmanaged objects.
        /// When called from the finalizer, it is only safe to dispose of unmanaged objects.
        /// This method should expect to be called multiple times without causing an exception.
        /// </remarks>
        internal void Dispose(bool fromUSer)
        {
            if (fromUSer)
            {
                //called from user code rather than garbage collection
                GC.SuppressFinalize(this);  // no need for the finalizer to do all of this again
            }
        }

        /// <summary>
        /// Dispose of all resources (both managed and unmanaged) used by this class.
        /// </summary>
        public void Dispose()
        {
            // Call our private Dispose method, indicating that the call originated from user code.
            // Diagnostics.TraceInfo("Disposed by user code.");
            this.Dispose(true);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MainPresenter"/> class. 
        /// Destructor, called by the finalizer during garbage collection.
        /// There is no guarantee that this method will be called. For example, if <see cref="Dispose"/> has already
        /// been called in user code for this object, then finalization may have been suppressed.
        /// </summary>
        ~MainPresenter()
        {
            // call our private Dispose method, indicating that the call originated from the finalizer.
            // Diagnostics.TraceInfo("Finalizer is disposing MusicEntityDAL instance");
            this.Dispose(false);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPresenter"/> class.
        /// </summary>
        /// <param name="form">
        /// The form.
        /// </param>
        public MainPresenter(MainForm form)
        {
            _form = form;
            _rootdir = Properties.Settings.Default.Directory;
        }

        /// <summary>
        /// The start
        /// </summary>
        public void Start()
        {
            if (!string.IsNullOrEmpty(_rootdir))
            {
                ScanDirectory(_rootdir);
            }
        }

        /// <summary>
        /// the scan
        /// </summary>
        public void Scan()
        {
            DirBrowser dirBrowser = new DirBrowser();

            if(dirBrowser.ShowDialog() == DialogResult.Ok)
            {
                ScanDirectory(dirBrowser.DirectoryPath);
            }
        }

        private void ScanDirectory(string scandir)
        {
            try
            {
                using (new CursorKeeper(Cursors.WaitCursor))
                {
                    IEnumerable<string> browserEnum = FileIterator.GetFiles(scandir, "*.mp3");
                    string[] files = browserEnum.ToArray();

                    _form.SetDirectoryList(files);

                    // success:  save the directory
                    _rootdir = scandir;
                    Properties.Settings.DefaultDirectory = _rootdir;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Utils.ExceptionMessageBox.Show(ex, "Directory scan failed.");
            }
        }

        /// <summary>
        /// The edit tag.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void EditTag(string filename)
        {
            Mp3File mp3file;

            try
            {
                // create mp3 file wrapper; open it and read the tags
                mp3file = new Mp3File(filename);
            }
            catch (Exception ex)
            {
                Utils.ExceptionMessageBox.Show(_form, ex, "Error Reading Tag");
                return;
            }

            // create dialog and give it the ID3v2 block for editing
            // this is a bit sneaky; it uses the edit dialog straight out of TagScanner.exe as if it was a dll
            ID3AdapterEdit id3Edit = new ID3AdapterEdit(mp3file);

            if (id3Edit.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (new CursorKeeper(Cursors.WaitCursor))
                    {
                        mp3file.Update();
                    }
                }
                catch (Exception e)
                {
                    ExceptionMessageBox.Show(_form, e, "Error writing tag");
                }
            }
        }

        /// <summary>
        /// The edit extended tag.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void EditExtendedTag(string filename)
        {
            Mp3File mp3File;

            try
            {
                // create mp3 file wrapper; open it and read the tags
                mp3File = new Mp3File(filename);
            }
            catch (Exception Ex)
            {
                ExceptionMessageBox.Show(_form, Ex, "Error reading tag");
                return;
            }

            if (mp3File.TagModel != null)
            {
                // create dialog and give it the ID3v2 block for editing
                // this is a bit sneaky; it uses the edit dialog straight out of TagScanner.exe as it it was a dll
                ID3PowerEdit id3PowerEdit = new ID3PowerEdit();
                ID3PowerEdit.TagModel = mp3File.TagModel;
                id3PowerEdit.ShowDialog();
            }
        }

        /// <summary>
        /// The compact.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void Compact(string filename, bool keepBackup)
        {
            try
            {
                // create mp3 file wrapper; open it and read the tags
                Mp3File mp3File = new Mp3File(filename);

                try
                {
                    using (new CursorKeeper(Cursors.WaitCursor))
                    {
                        mp3File.UpdatePacked(keepBackup);
                    }
                }
                catch (Exception e)
                {
                    ExceptionMessageBox.Show(_form, e, "Error Writing Tag");
                }
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(_form, e, "Error Reading Tag");
            }
        }

        /// <summary>
        /// The launch.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void Launch(string filename)
        {
            using (new CursorKeeper(Cursors.WaitCursor))
            {
                Process winamp = new Process();
                winamp.StartInfo.FileName = filename;
                winamp.Start();
            }
        }

        /// <summary>
        /// The remove v 2 tag.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        internal void RemoveV2tag(string filename)
        {
            try
            {
                // create mp3 file wrapper; open it and read the tags
                Mp3File mp3File = new Mp3File(filename);

                try
                {
                    using (new CursorKeeper(Cursors.WaitCursor))
                    {
                        mp3File.UpdateNoV2tag();
                    }
                }
                catch (Exception e)
                {
                    ExceptionMessageBox.Show(_form, e, "Error Writing Tag");
                }
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(_form, e, "Error Reading Tag");
            }
        }
    }
}
