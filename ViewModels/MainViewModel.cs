using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AutoRename.Commands;
using AutoRename.Services;
using Microsoft.Win32;

namespace AutoRename
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Lazy<OpenFileDialog> openFileDialog = new Lazy<OpenFileDialog>(() => new OpenFileDialog { Multiselect = true });
        private readonly FileNameProcessor fileNameProcessor;

        private ObservableCollection<GridRowViewModel> dataGridRows = new ObservableCollection<GridRowViewModel>();
        private string websiteButton = Properties.Resources.WebsiteButtonVisit;
        private GridRowViewModel selectedItem;
        private bool showExtension;
        private bool showFullPath;
        private bool showGridLines;
        private bool exitAfterRename;
        private bool renameButtonEnabled;
        private bool contextMenuEnabled;
        private Visibility rowSettingsEnabled = Visibility.Collapsed;
        private RelayCommand renameButtonCommand;
        private RelayCommand addFileCommand;
        private StartProcessCommand startProcessCommand;
        private RelayCommand removeSelectedCommand;
        private RelayCommand removeAllCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(FileNameProcessor fileNameProcessor)
        {
            this.fileNameProcessor = fileNameProcessor;

            PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(SelectedItem):
                        CheckGuiStates();
                        break;

                    case nameof(StartWithUpperCase):
                        foreach (var row in DataGridRows)
                        {
                            row.StartWithUpperCase = StartWithUpperCase;
                        }
                        break;

                    case nameof(RemoveBrackets):
                        foreach (var row in DataGridRows)
                        {
                            row.RemoveBrackets = RemoveBrackets;
                        }
                        break;

                    case nameof(RemoveStartingNumber):
                        foreach (var row in DataGridRows)
                        {
                            row.RemoveStartingNumber = RemoveStartingNumber;
                        }
                        break;

                    case nameof(ShowFullPath):
                        foreach (var row in DataGridRows)
                        {
                            row.ShowFullPath = ShowFullPath;
                        }
                        break;

                    case nameof(ShowExtension):
                        foreach (var row in DataGridRows)
                        {
                            row.ShowExtension = ShowExtension;
                        }
                        break;
                }
            };
        }

        /// <summary>
        /// DataGrid Rows
        /// </summary>
        public ObservableCollection<GridRowViewModel> DataGridRows
        {
            get => dataGridRows;
            set
            {
                if (dataGridRows != value)
                {
                    dataGridRows = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Website button caption
        /// </summary>
        public string WebsiteButton
        {
            get => websiteButton;
            set
            {
                if (websiteButton != value)
                {
                    websiteButton = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ForceOverwrite { get; set; }

        /// <summary>
        /// Selected row in DagaGrid
        /// </summary>
        public GridRowViewModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Start wit upper case checkbox
        /// </summary>
        public bool StartWithUpperCase
        {
            get => fileNameProcessor.StartWithUpperCase;
            set
            {
                if (fileNameProcessor.StartWithUpperCase != value)
                {
                    fileNameProcessor.StartWithUpperCase = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Remove brackets
        /// </summary>
        public bool RemoveBrackets
        {
            get => fileNameProcessor.RemoveBrackets;
            set
            {
                if (fileNameProcessor.RemoveBrackets != value)
                {
                    fileNameProcessor.RemoveBrackets = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RemoveStartingNumber
        {
            get => fileNameProcessor.RemoveStartingNumber;
            set
            {
                if (fileNameProcessor.RemoveStartingNumber != value)
                {
                    fileNameProcessor.RemoveStartingNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Show extension checkbox
        /// </summary>
        public bool ShowExtension
        {
            get => showExtension;
            set
            {
                if (showExtension != value)
                {
                    showExtension = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        public bool ShowFullPath
        {
            get => showFullPath;
            set
            {
                if (showFullPath != value)
                {
                    showFullPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        public bool ShowGridLines
        {
            get => showGridLines;
            set
            {
                if (showGridLines != value)
                {
                    showGridLines = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        public bool ExitAfterRename
        {
            get => exitAfterRename;
            set
            {
                exitAfterRename = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Rename button
        /// </summary>
        public ICommand RenameButtonClick => renameButtonCommand ?? (renameButtonCommand = new RelayCommand(RenameAll));

        /// <summary>
        /// Add file button
        /// </summary>
        public ICommand AddFileButtonClick => addFileCommand ?? (addFileCommand = new RelayCommand(TryAddFiles));

        /// <summary>
        /// Website button
        /// </summary>
        public ICommand WebsiteButtonClick => startProcessCommand ?? (startProcessCommand = new StartProcessCommand(Properties.Resources.HomePage));

        /// <summary>
        /// Remove selected rows
        /// </summary>
        public ICommand RemoveSelectedCommand => removeSelectedCommand ?? (removeSelectedCommand = new RelayCommand(RemoveSelected));

        /// <summary>
        /// Remove all rows
        /// </summary>
        public ICommand RemoveAllCommand => removeAllCommand ?? (removeAllCommand = new RelayCommand(RemoveAll));

        /// <summary>
        /// Rename button Enabled / Disabled
        /// </summary>
        public bool RenameButtonEnabled
        {
            get => renameButtonEnabled;
            set
            {
                renameButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ContextMenuEnabled
        {
            get => contextMenuEnabled;
            set
            {
                contextMenuEnabled = value;
                OnPropertyChanged();
            }
        }

        public Visibility RowSettingsEnabled
        {
            get => rowSettingsEnabled;
            set
            {
                rowSettingsEnabled = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Add files to table
        /// </summary>
        /// <param name="files">file collection</param>
        public void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                AddFile(file);
            }
        }

        /// <summary>
        /// Add file to table
        /// </summary>
        /// <param name="file">file path</param>
        public void AddFile(string file)
        {
            if (FileAlreadyLoaded(file))
            {
                return;
            }

            try
            {
                GridRowViewModel newRow = new GridRowViewModel(this, file, fileNameProcessor);
                DataGridRows.Add(newRow);
                CheckGuiStates();
            }
            catch
            {
                SystemSounds.Beep.Play();
            }
        }

        private void CheckGuiStates()
        {
            RenameButtonEnabled = ContextMenuEnabled = DataGridRows.Count > 0;
            RowSettingsEnabled = SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool FileAlreadyLoaded(string file)
        {
            foreach (GridRowViewModel item in DataGridRows)
            {
                if (item.OldFullPath == file)
                {
                    return true;
                }
            }

            return false;
        }

        private void TryAddFiles()
        {
            bool? showDialog = openFileDialog.Value.ShowDialog();

            if (showDialog.HasValue && showDialog.Value)
            {
                AddFiles(openFileDialog.Value.FileNames);
            }
        }

        private void RemoveSelected()
        {
            DataGridRows.Remove(SelectedItem);
            CheckGuiStates();
        }

        private void RemoveAll()
        {
            DataGridRows = new ObservableCollection<GridRowViewModel>();
            CheckGuiStates();
        }

        public void RenameAll()
        {
            if (DataGridRows.Count == 0)
            {
                return;
            }

            bool success = true;
            var renamedRows = new List<GridRowViewModel>();

            foreach (GridRowViewModel row in DataGridRows)
            {
                bool renamed = row.Rename();
                if (renamed)
                {
                    renamedRows.Add(row);
                }
                success &= renamed;
            }

            foreach (GridRowViewModel row in renamedRows)
            {
                DataGridRows.Remove(row);
            }

            if (success)
            {
                if (ExitAfterRename)
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                SystemSounds.Beep.Play();
            }
        }
    }

    /// <summary>
    /// GUI edit type
    /// </summary>
    public enum EditType
    {
        FileName,
        UpperCase,
        Brackets,
        StartingNumber,
        Visual
    }
}