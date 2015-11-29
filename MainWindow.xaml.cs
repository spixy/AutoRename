using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;

namespace QuickRename
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string websiteButtonVisit = "Visit\r\nwebsite";
        private const string websiteButtonUpdate = "Update to\r\nnew version";

        private MainViewModel model;
        private FileNameProcessor fileNameProcessor;

        private Lazy<OpenFileDialog> openFileDialog = new Lazy<OpenFileDialog>(() =>
        {
            var _openFileDialog = new OpenFileDialog();
            _openFileDialog.Multiselect = true;
            return _openFileDialog;
         });

        public MainWindow()
        {
            InitializeComponent();

            model = new MainViewModel();
            model.PropertyChanged += PropertyChanged;
            model.WebsiteButton = websiteButtonVisit;

            fileNameProcessor = new FileNameProcessor(model);

            // bind to UI
            DataContext = model;
        }

        private void LoadArgv()
        {
            string[] argv = Environment.GetCommandLineArgs();

            for (int i = 1; i < argv.Length; i++)
            {
                switch (argv[i].ToLower())
                {
                    case "-s":
                        model.StartWithUpperCase = true;
                        continue;
                    case "-f":
                        model.Overwrite = true;
                        continue;
                }

                if (Utility.ItemExists(argv[i]))
                {
                    AddRow(argv[i]);
                }
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists(QuickRename.Properties.Resources.ConfigFile))
                return;

            try
            {
                string[] lines = File.ReadAllLines(QuickRename.Properties.Resources.ConfigFile);

                foreach (string line in lines)
                {
                    if (line.Contains("overwrite "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("overwrite ", ""), out val))
                            model.Overwrite = val;
                    }

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
                        string res = line.Substring(7);
                        Width = Convert.ToInt32(res.Substring(0, res.IndexOf('x')));
                        Height = Convert.ToInt32(res.Substring(res.IndexOf('x') + 1));
                    }

                    if (line.Contains("UpperCaseExceptions ") && line.Length > 20)
                    {
                        fileNameProcessor.UpperCaseExceptions = line.Substring(20).Split('|');
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
                using (StreamWriter sw = new StreamWriter(QuickRename.Properties.Resources.ConfigFile, false))
                {
                    sw.WriteLine("overwrite " + model.Overwrite);
                    sw.WriteLine("uppercase " + model.StartWithUpperCase);
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

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowExtension":
                    ApplyRules();
                    break;

                case "ShowFullPath":
                    ApplyRules();
                    break;

                case "StartWithUpperCase":
                    ChangeToUpperCase();
                    break;

                case "NewViewPath":
                    FileNameChanged((GridRowViewModel)sender);
                    break;
            }
        }

        private void FileNameChanged(GridRowViewModel row)
        {
            if (model.ShowExtension)
            {
                row.NewFullPath = Path.GetDirectoryName(row.NewFullPath) + Path.DirectorySeparatorChar + row.NewViewPath;
            }
            else
            {
                row.NewFullPath = Path.GetDirectoryName(row.NewFullPath) + Path.DirectorySeparatorChar
                                + Path.GetFileNameWithoutExtension(row.NewViewPath) + Path.GetExtension(row.NewFullPath);
            }
        }

        private void ApplyRules()
        {
            var list = model.DataGridRowsList;
            foreach (var item in list)
            {
                item.OldViewPath = fileNameProcessor.ApplyRules(item.OldFullPath);
                item.NewViewPath = fileNameProcessor.ApplyRules(item.NewFullPath);
            }
        }

        private void ChangeToUpperCase()
        {
            var list = model.DataGridRowsList;
            foreach (var item in list)
            {
                item.NewFullPath = fileNameProcessor.QRename(item.OldFullPath);
                item.NewViewPath = fileNameProcessor.ApplyRules(item.NewFullPath);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            LoadArgv();

            Updater updater = new Updater(QuickRename.Properties.Resources.UpdateFile);
            updater.UpdateAvailableAction = () => model.WebsiteButton = websiteButtonUpdate;
            updater.IsUpdateAvailableAsync();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.Value.ShowDialog(this).Value)
            {
                foreach (string file in openFileDialog.Value.FileNames)
                {
                    AddRow(file);
                }
            }
        }

        private void ButtonWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(QuickRename.Properties.Resources.HomePage);
        }

        private void ButtonRename_Click(object sender, RoutedEventArgs e)
        {
            var list = model.DataGridRowsList;

            if (list.Count == 0)
                return;

            bool success = true;

            var newList = new List<GridRowViewModel>();

            foreach (GridRowViewModel row in list)
            {
                bool result = Rename(row);

                if (!result)
                {
                    row.Error = true;
                    newList.Add(row);
                }

                success &= result;
            }

            if (success)
            {
                Application.Current.Shutdown();
            }
            else
            {
                SystemSounds.Beep.Play();

                // refresh UI
                model.DataGridRowsList = newList;
            }
        }

        private void AddRow(string file)
        {
            GridRowViewModel newRow = new GridRowViewModel();
            newRow.PropertyChanged += PropertyChanged;

            newRow.OldFullPath = file;
            newRow.OldViewPath = fileNameProcessor.ApplyRules(newRow.OldFullPath);

            newRow.NewFullPath = fileNameProcessor.QRename(file);
            newRow.NewViewPath = fileNameProcessor.ApplyRules(newRow.NewFullPath);

            var list = model.DataGridRowsList;

            if (!ContainsOldFullPath(list, newRow))
            {
                list.Add(newRow);
                model.DataGridRowsList = list;
            }
        }

        private bool ContainsOldFullPath(List<GridRowViewModel> list, GridRowViewModel newItem)
        {
            foreach (GridRowViewModel item in list)
            {
                if (item.OldFullPath == newItem.OldFullPath)
                {
                    return true;
                }
            }

            return false;
        }

        private bool Rename(GridRowViewModel row)
        {
            if (!model.Overwrite && File.Exists(row.NewFullPath))
                return false;

            if (Utility.ItemExists(row.OldFullPath))
            {
                try
                {
                    Directory.Move(row.OldFullPath, row.NewFullPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
                foreach (string file in files)
                    AddRow(file);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            model.DataGridRowsList = null;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            GridRowViewModel curent = model.SelectedItem;

            var list = model.DataGridRowsList;

            if (list != null)
            {
                list.Remove(curent);
                model.DataGridRowsList = list;
            }
        }
    }
}
