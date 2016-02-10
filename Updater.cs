using System;
using System.ComponentModel;
using System.Net;

namespace AutoRename
{
    public class Updater
    {
        /// <summary>
        /// Latest application version
        /// </summary>
        public string LatestVersion { get; private set; }

        /// <summary>
        /// File with latest application version
        /// </summary>
        public string UpdateFile { get; set; }

        /// <summary>
        /// Asynchronious action occurs when update is available after calling IsUpdateAvailableAsync()
        /// </summary>
        public Action UpdateAvailableAction { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="updateFile">File with latest application version</param>
        public Updater(string updateFile)
        {
            UpdateFile = updateFile;
        }

        /// <summary>
        /// Asynchroniously check if update is available
        /// </summary>
        public void IsUpdateAvailableAsync()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += IsUpdateAvailableBackground;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (((bool)e.Result) && UpdateAvailableAction != null)
            {
                UpdateAvailableAction();
            }
        }

        private void IsUpdateAvailableBackground(object sender, DoWorkEventArgs e)
        {
            e.Result = IsUpdateAvailable();
        }

        /// <summary>
        /// Check if update is available
        /// </summary>
        public bool IsUpdateAvailable()
        {
            try
            {
                using (WebClient update = new WebClient())
                {
                    LatestVersion = update.DownloadString(UpdateFile);
                }

                return (LatestVersion != Utility.CurrentApplication.Version.ToString());
            }
            catch
            {
                return false;
            }
        }
    }
}
