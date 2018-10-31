using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows.Forms;

namespace ID3Tagging.ID3Editor
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        #region fields

        private readonly MainPresenter _presenter;

        /// <summary>
        /// Gets the presenter.
        /// </summary>
        public MainPresenter Presenter
        {
            get
            {
                return _presenter;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _presenter = new TagEditor.MainPresenter(this);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            _presenter.Start();
        }

        private void _scanMenuItem_Click(object sender, EventArgs e)
        {
            _presenter.Scan();
        }

        /// <summary>
        /// The set directory list.
        /// </summary>
        /// <param name="files">The files.</param>
        public virtual void SetDirectoryList(string[] files)
        {
            var fileObjects = new object[files.Length];
            Array.Copy(files, fileObjects, files.Length);

            _mainListBox.Items.Clear();
            _mainListBox.Items.AddRange(fileObjects);
        }

        /// <summary>
        /// when right mouse button is clicked, select the item under the mouse
        /// this makes it a lot easier to use the context menu on the list of files
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void _mainListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int indexover = _mainListBox.IndexFromPoint(e.X, e.Y);
            if (indexover >= 0 && indexover < _mainListBox.Items.Count)
            {
                _mainListBox.SelectedIndex = indexover;
            }

            _mainListBox.Refresh();
        }

        private void _mainListBoxMenu_EditExtendedTag(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex != -1)
                _presenter.EditExtendedTag((string)_mainListBox.Items[_mainListBox.SelectedIndex]);
        }

        private void _mainListBoxMenu_EditTag(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex != -1)
                _presenter.EditTag((string)_mainListBox.Items[_mainListBox.SelectedIndex]);
        }

        private void _mainListBox_DoubleClick(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex >= 0)
                _presenter.EditTag((string)_mainListBox.Items[_mainListBox.SelectedIndex]);
        }

        private void _mainListBoxMenu_Compact(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex != -1)
                _presenter.Compact((string)_mainListBox.Items[_mainListBox.SelectedIndex], true);
        }

        private void mainListBoxMenu_CompactNoBackup(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex != -1)
                _presenter.Compact((string)_mainListBox.Items[_mainListBox.SelectedIndex], false);
        }

        private void _mainListBoxMenu_Launch(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex != -1)
                _presenter.Launch((string)_mainListBox.Items[_mainListBox.SelectedIndex]);
        }

        private void _removeV2tag_Click(object sender, EventArgs e)
        {
            if (_mainListBox.SelectedIndex != -1)
                _presenter.RemoveV2tag((string)_mainListBox.Items[_mainListBox.SelectedIndex]);
        }
    }
}
