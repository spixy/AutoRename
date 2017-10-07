using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AutoRename
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
	    private readonly MainViewModel model;
		private readonly FileNameProcessor fileNameProcessor;

		public MainWindow()
        {
            InitializeComponent();

			fileNameProcessor = new FileNameProcessor();

			model = new MainViewModel(fileNameProcessor);

			DataContext = model;

			LoadSettings();
        }

        private void ProcessArgv(ref bool renameAutomatically, ref bool forceOverwrite)
		{
			string[] argv = Environment.GetCommandLineArgs();
			bool showHelpBox = false;

			for (int i = 1; i < argv.Length; i++)
            {
	            string arg = argv[i];

	            switch (arg.ToLower().Replace('/','-'))
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
						forceOverwrite = true;
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
						if (Utility.ItemExists(arg))
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
            if (!File.Exists(Properties.Resources.ConfigFile))
                return;

            try
            {
                string[] lines = File.ReadAllLines(Properties.Resources.ConfigFile);
				bool boolValue;
				string strValue;
				Point vec2Value;

				foreach (string line in lines)
                {
					string lineInLower = line.ToLowerInvariant();

					if (Utility.TryGetBoolValue(lineInLower, "overwrite", out boolValue))
					{
						this.fileNameProcessor.ForceOverwrite = boolValue;
					}
					else if (Utility.TryGetBoolValue(lineInLower, "uppercase", out boolValue))
					{
						this.model.StartWithUpperCase = boolValue;
					}
					else if (Utility.TryGetBoolValue(lineInLower, "remove brackets", out boolValue))
	                {
		                this.model.RemoveBrackets = boolValue;
					}
	                else if (Utility.TryGetBoolValue(lineInLower, "remove starting number", out boolValue))
					{
						this.model.RemoveStartingNumber = boolValue;
					}
					else if (Utility.TryGetBoolValue(lineInLower, "extension", out boolValue))
	                {
		                this.model.ShowExtension = boolValue;
					}
					else if (Utility.TryGetBoolValue(lineInLower, "full path", out boolValue))
					{
						this.model.ShowFullPath = boolValue;
					}
					else if (Utility.TryGetBoolValue(lineInLower, "grid lines", out boolValue))
	                {
		                this.model.ShowGridLines = boolValue;
					}
					else if (Utility.TryGetBoolValue(lineInLower, "Exit after rename", out boolValue))
					{
					    this.model.ExitAfterRename = boolValue;
					}
                    else if (Utility.TryGetVec2Value(lineInLower, "position", out vec2Value))
	                {
		                Rect screen = SystemParameters.WorkArea;
	                    this.Left = Utility.Clamp(vec2Value.X, screen.Left, screen.Right);
	                    this.Top = Utility.Clamp(vec2Value.Y, screen.Top, screen.Bottom);
					}
					else if (Utility.TryGetVec2Value(lineInLower, "window", out vec2Value))
					{
						this.Width = vec2Value.X;
					    this.Height = vec2Value.Y;
					}
                }
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Cannot load preferences", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SaveSettings()
        {
            try
			{
				using (StreamWriter sw = new StreamWriter(Properties.Resources.ConfigFile, false))
				{
					sw.WriteLine("Overwrite " + fileNameProcessor.ForceOverwrite);
					sw.WriteLine("Uppercase " + fileNameProcessor.StartWithUpperCase);
					sw.WriteLine("Remove brackets " + fileNameProcessor.RemoveBrackets);
					sw.WriteLine("Remove starting number " + fileNameProcessor.RemoveStartingNumber);
                    sw.WriteLine("Extension " + model.ShowExtension);
					sw.WriteLine("Full path " + model.ShowFullPath);
					sw.WriteLine("Grid lines " + model.ShowGridLines);
                    sw.WriteLine("Exit after rename " + model.ExitAfterRename);
					sw.WriteLine("Position " + Left + "x" + Top);
                    sw.WriteLine("Window " + Width + "x" + Height);
                }
            }
            catch (Exception ex)
            {
	            MessageBox.Show(ex.Message, "Cannot save preferences", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			bool renameAutomatically = false;
	        bool forceOverwrite = false;

            ProcessArgv(ref renameAutomatically, ref forceOverwrite);

			this.fileNameProcessor.ForceOverwrite = forceOverwrite;

            if (renameAutomatically)
            {
                model.RenameAll();
            }
            else
            {
	            Updater updater = new Updater(Properties.Resources.UpdateFile)
	            {
					UpdateAvailableAction = UpdateAvailable
	            };
	            updater.IsUpdateAvailableAsync();
            }
        }

	    private void UpdateAvailable()
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
                e.Effects = DragDropEffects.All;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
                model.AddFiles(files);
        }

		private void StartWithUpperCase_Click(object sender, RoutedEventArgs e)
		{
			GridRowViewModel selectedItem = model.SelectedItem;
			MenuItem menuItem = (MenuItem) sender;

			selectedItem.StartWithUpperCase = menuItem.IsChecked;
		}

		private void RemoveBrackets_Click(object sender, RoutedEventArgs e)
		{
			GridRowViewModel selectedItem = model.SelectedItem;
			MenuItem menuItem = (MenuItem)sender;

			selectedItem.RemoveBrackets = menuItem.IsChecked;
		}

		private void RemoveStartingNumber_Click(object sender, RoutedEventArgs e)
		{
			GridRowViewModel selectedItem = model.SelectedItem;
			MenuItem menuItem = (MenuItem)sender;

			selectedItem.RemoveStartingNumber = menuItem.IsChecked;
		}

        private void ShowFileExtension_Click(object sender, RoutedEventArgs e)
        {
            GridRowViewModel selectedItem = model.SelectedItem;
            MenuItem menuItem = (MenuItem)sender;

            selectedItem.ShowExtension = menuItem.IsChecked;
        }

        private void ShowFullPath_Click(object sender, RoutedEventArgs e)
        {
            GridRowViewModel selectedItem = model.SelectedItem;
            MenuItem menuItem = (MenuItem)sender;

            selectedItem.ShowFullPath = menuItem.IsChecked;
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            model.RemoveAll();
        }

	    private void Clear_Click(object sender, RoutedEventArgs e)
	    {
		    model.RemoveSelected();
	    }
    }
}
