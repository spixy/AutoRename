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
		private static FileNameProcessor fileNameProcessor { get { return FileNameProcessor.Instance; } }

		private readonly MainViewModel model;
		private bool renameAutomatically = false;

        public MainWindow()
        {
            InitializeComponent();

			model = new MainViewModel();
	        // bind to UI
			DataContext = model;

			LoadSettings();
        }

        private void LoadArgv()
        {
            string[] argv = Environment.GetCommandLineArgs();

            for (int i = 1; i < argv.Length; i++)
            {
	            string arg = argv[i];

	            switch (arg.ToLower())
				{
					case "-b":
						model.RemoveBrackets = true;
						continue;
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
					string lineInLower = line.ToLowerInvariant();

					if (lineInLower.StartsWith("overwrite "))
					{
						bool val;
						if (bool.TryParse(lineInLower.Replace("overwrite ", ""), out val))
							fileNameProcessor.ForceOverwrite = val;
					}

					if (lineInLower.StartsWith("uppercase "))
                    {
                        bool val;
						if (bool.TryParse(lineInLower.Replace("uppercase ", ""), out val))
                            model.StartWithUpperCase = val;
                    }

					if (lineInLower.StartsWith("extension "))
                    {
                        bool val;
						if (bool.TryParse(lineInLower.Replace("extension ", ""), out val))
                            model.ShowExtension = val;
                    }

					if (lineInLower.StartsWith("full path "))
                    {
                        bool val;
                        if (bool.TryParse(lineInLower.Replace("full path ", ""), out val))
                            model.ShowFullPath = val;
                    }

					if (lineInLower.StartsWith("position "))
					{
						string[] vals = lineInLower.Replace("position ", "").Split('x');

						if (vals.Length == 2)
						{
							double x, y;

							if (double.TryParse(vals[0], out x) && double.TryParse(vals[1], out y))
							{
								Rect screen = SystemParameters.WorkArea;
								Left = Utility.Clamp(x, screen.Left, screen.Right);
								Top = Utility.Clamp(y, screen.Top, screen.Bottom);
							}
						}
					}

					if (lineInLower.StartsWith("window "))
					{
						string[] vals = lineInLower.Replace("window ", "").Split('x');

						if (vals.Length == 2)
	                    {
							double w, h;

							if (double.TryParse(vals[0], out w) && double.TryParse(vals[1], out h))
							{
								Width = w;
								Height = h;
							}
	                    }
                    }

					if (lineInLower.StartsWith("uppercaseexceptions "))
					{
						string values = line.Substring("uppercaseexceptions ".Length);
						fileNameProcessor.UpperCaseExceptions = values.Split('|');
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
					sw.WriteLine("Overwrite " + fileNameProcessor.ForceOverwrite);
					sw.WriteLine("Uppercase " + fileNameProcessor.StartWithUpperCase);
                    sw.WriteLine("Extension " + fileNameProcessor.ShowExtension);
					sw.WriteLine("Full path " + fileNameProcessor.ShowFullPath);
					sw.WriteLine("Position " + Left + "x" + Top);
                    sw.WriteLine("Window " + Width + "x" + Height);
                    sw.WriteLine("UpperCaseExceptions " + string.Join("|", fileNameProcessor.UpperCaseExceptions));
                }
            }
            catch
            {
                // ignored
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadArgv();

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
