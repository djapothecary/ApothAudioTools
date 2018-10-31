using System;
using System.Reflection;
using System.Windows.Forms;

namespace ID3Tagging.ID3Editor
{
    // <summary>
    /// The program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            UpgradeUserSettings();

            Application.Run(new MainForm());
        }

        private static void UpgradeUserSettings()
        {
            // hook up to user.config from previous build
            Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            Version appVersion = a.GetName().Version;
            string appVersionString = appVersion.ToString();

            if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ApplicationVersion = appVersionString;
            }
        }
    }
}
