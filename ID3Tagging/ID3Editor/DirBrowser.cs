using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ID3Tagging.ID3Editor
{
    /// <summary>
    /// Directory Browser
    /// </summary>
    public class DirBrowser : FolderNameEditor
    {
        #region Fields

        private string _description = "Select Directory";
        private string _directory = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// gets or sets the description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                this._description = value;
            }
        }

        /// <summary>
        /// gets the directory path
        /// </summary>
        public string DirectoryPath
        {
            get
            {
                return _directory;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// the show dialog
        /// </summary>
        /// <returns>
        /// The <see cref="DialogResult"/>.
        /// </returns>
        public DialogResult ShowDialog()
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.Description = _description;
            browser.StartLocation = FolderNameEditor.FolderBrowserFolder.MyComputer;
            DialogResult result == DialogResult.OK ? browser.DirectoryPath : string.Empty;
            return result;
        }
        #endregion
    }
}
