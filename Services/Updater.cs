using System;
using System.ComponentModel;
using System.Net;

namespace AutoRename
{
    public class Updater
    {
        private readonly string updateFileUrl;

        /// <summary>
        /// Latest application version
        /// </summary>
        public string LatestVersion { get; private set; }

        /// <summary>
        /// Asynchronous action occurs when update is available after calling IsUpdateAvailableAsync()
        /// </summary>
        public Action UpdateAvailableAction { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="updateFileUrl">File with latest application version</param>
        public Updater(string updateFileUrl)
        {
            this.updateFileUrl = updateFileUrl;
        }

        /// <summary>
        /// Asynchroniously check if update is available
        /// </summary>
        public void CheckForUpdateAvailableAsync()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += IsUpdateAvailableBackground;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                UpdateAvailableAction?.Invoke();
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
					LatestVersion = update.DownloadString(updateFileUrl).Trim();
                }

				string currentVersion = Utility.CurrentApplication.Version.ToString().Trim();
				return currentVersion != LatestVersion;
            }
            catch
            {
                return false;
            }
        }
    }
}
