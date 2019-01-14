using System;
using System.IO;
using System.Windows;
using AutoRename.Services;

namespace AutoRename
{
    public partial class MainWindow : Window
    {
	    private readonly MainViewModel model;
		private readonly FileNameProcessor fileNameProcessor;
        private readonly Persistence persistence;

        public MainWindow()
        {
            InitializeComponent();

			fileNameProcessor = new FileNameProcessor();
            persistence = new Persistence(Properties.Resources.ConfigFile);
            DataContext = model = new MainViewModel(fileNameProcessor);

            string appName = Utility.CurrentApplication.Name;
            Version appVersion = Utility.CurrentApplication.Version;
            Title = $"{appName} {appVersion.Major}{(appVersion.Minor > 0 ? "." + appVersion.Minor : "")}{(appVersion.Build > 0 ? "." + appVersion.Build : "")}";

            LoadSettings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			bool renameAutomatically = false;

            ProcessArgv(ref renameAutomatically);

            if (renameAutomatically)
            {
                model.RenameAll();
            }
            else
            {
	            Updater updater = new Updater(Properties.Resources.UpdateFile)
	            {
					UpdateAvailableAction = OnUpdateAvailable
	            };
	            updater.CheckForUpdateAvailableAsync();
            }
        }

        private void ProcessArgv(ref bool renameAutomatically)
        {
            string[] argv = Environment.GetCommandLineArgs();
            bool showHelpBox = false;

            for (int i = 1; i < argv.Length; i++)
            {
                string arg = argv[i];

                switch (arg.ToLower().Replace('/', '-'))
                {
                    case "-b":
                        model.RemoveBrackets = true;
                        break;

                    case "-uc":
                        model.StartWithUpperCase = true;
                        break;

                    case "-sn":
                        model.RemoveStartingNumber = true;
                        break;

                    case "-f":
                        model.ForceOverwrite = true;
                        break;

                    case "-y":
                        renameAutomatically = true;
                        break;

                    case "-e":
                        model.ExitAfterRename = true;
                        break;

                    case "/?":
                    case "-help":
                    case "--help":
                        showHelpBox = true;
                        break;

                    default:
                        if (File.Exists(arg) || Directory.Exists(arg))
                        {
                            model.AddFile(arg);
                        }
                        else
                        {
                            showHelpBox = true;
                        }
                        break;
                }
            }

            if (showHelpBox)
            {
                MessageBox.Show(Properties.Resources.CmdParametersHelp, "Command line parameters", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists(persistence.FilePath))
            {
                return;
            }
            try
            {
                persistence.LoadSettings(model, this);
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
                persistence.SaveSettings(model, this);
            }
            catch
            {
                // ignored
            }
        }

        private void OnUpdateAvailable()
	    {
		    model.WebsiteButton = Properties.Resources.WebsiteButtonUpdate;
	    }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effects = DragDropEffects.All;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                model.AddFiles(files);
            }
        }
    }
}
