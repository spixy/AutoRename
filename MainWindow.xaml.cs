using System;
using System.IO;
using System.Windows;

namespace AutoRename
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

		private bool renameAutomatically = false;

        private readonly MainViewModel model;

	    private readonly FileNameProcessor fileNameProcessor;

        #endregion

        #region Start and Exit

        public MainWindow()
        {
            InitializeComponent();

			fileNameProcessor = new FileNameProcessor();
			model = new MainViewModel(fileNameProcessor);

	        // bind to UI
            DataContext = model;
        }

        private void LoadArgv()
        {
            string[] argv = Environment.GetCommandLineArgs();

            for (int i = 1; i < argv.Length; i++)
            {
	            string arg = argv[i];

	            switch (arg.ToLower())
                {
                    case "-s":
                        model.StartWithUpperCase = true;
                        continue;
                    case "-f":
						fileNameProcessor.ForceOverwrite = true;
                        continue;
                    case "-y":
                        renameAutomatically = true;
                        continue;
                }

                if (Utility.ItemExists(arg))
                {
					model.AddFile(arg);
                }
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists(Properties.Resources.ConfigFile))
                return;

            try
            {
                string[] lines = File.ReadAllLines(Properties.Resources.ConfigFile);

                foreach (string line in lines)
                {
                    if (line.Contains("uppercase "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("uppercase ", ""), out val))
                            model.StartWithUpperCase = val;
                    }

                    if (line.Contains("extension "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("extension ", ""), out val))
                            model.ShowExtension = val;
                    }

                    if (line.Contains("full path "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("full path ", ""), out val))
                            model.ShowFullPath = val;
                    }

                    if (line.Contains("window ") && line.Length > 9)
                    {
                        string res = line.Replace("window ", "");
                        int delim = res.IndexOf('x');

                        Width = Convert.ToInt32(res.Substring(0, delim));
                        Height = Convert.ToInt32(res.Substring(delim + 1));
                    }

                    if (line.Contains("UpperCaseExceptions ") && line.Length > 20)
                    {
                        fileNameProcessor.UpperCaseExceptions = line.Replace("UpperCaseExceptions ","").Split('|');
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SaveSettings()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Properties.Resources.ConfigFile, false))
                {
                    sw.WriteLine("uppercase " + fileNameProcessor.StartWithUpperCase);
                    sw.WriteLine("extension " + model.ShowExtension);
                    sw.WriteLine("full path " + model.ShowFullPath);
                    sw.WriteLine("window " + Width + "x" + Height);
                    sw.WriteLine("UpperCaseExceptions " + string.Join("|", fileNameProcessor.UpperCaseExceptions));
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            LoadArgv();

            if (renameAutomatically)
            {
				model.RenameAll();
            }
            else
            {
	            Updater updater = new Updater(Properties.Resources.UpdateFile)
	            {
					UpdateAvailableAction = () => model.WebsiteButton = Properties.Resources.WebsiteButtonUpdate
	            };
	            updater.IsUpdateAvailableAsync();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effects = DragDropEffects.All;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
            {
                model.AddFiles(files);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            model.RemoveAll();
        }

	    private void Clear_Click(object sender, RoutedEventArgs e)
	    {
		    model.RemoveSelected();
	    }

	    #endregion
    }
}
