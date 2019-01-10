using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using AutoRename.Services;

namespace AutoRename
{
    /// <summary>
    /// Row in DataGrid
    /// </summary>
    public class GridRowViewModel : INotifyPropertyChanged
    {
        private static readonly SolidColorBrush normalBrush = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush errorBrush = new SolidColorBrush(Colors.Red);

        private readonly MainViewModel mainViewModel;
        private readonly FileNameProcessor fileNameProcessor;

        private bool isEditing;

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) =>
        {
            GridRowViewModel model = (GridRowViewModel) sender;

            switch (args.PropertyName)
            {
                case nameof(NewViewPath):
                    model.ValuesChanged(EditType.FileName);
                    break;

                case nameof(StartWithUpperCase):
                    model.ValuesChanged(EditType.UpperCase);
                    break;

                case nameof(RemoveBrackets):
                    model.ValuesChanged(EditType.Brackets);
                    break;

                case nameof(RemoveStartingNumber):
                    model.ValuesChanged(EditType.StartingNumber);
                    break;

                case nameof(ShowExtension):
                case nameof(ShowFullPath):
                    model.ValuesChanged(EditType.Visual);
                    break;
            }
        };

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GridRowViewModel(MainViewModel mainViewModel, string file, FileNameProcessor fileNameProcessor)
        {
            this.mainViewModel = mainViewModel;
            this.fileNameProcessor = fileNameProcessor;

            isEditing = true;

            OldFullPath = file;
            OldViewPath = fileNameProcessor.ApplyVisualRules(OldFullPath, ShowExtension, ShowFullPath);
            NewFullPath = fileNameProcessor.AutoRename(OldFullPath);
            NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath, ShowExtension, ShowFullPath);

            isEditing = false;

            startWithUpperCase = fileNameProcessor.StartWithUpperCase;
            removeBrackets = fileNameProcessor.RemoveBrackets;
            removeStartingNumber = fileNameProcessor.RemoveStartingNumber;
            ShowFullPath = mainViewModel.ShowFullPath;
            ShowExtension = mainViewModel.ShowExtension;
        }

        /// <summary>
        /// Background brush
        /// </summary>
        private SolidColorBrush brush = normalBrush;

        public SolidColorBrush Brush
        {
            get => brush;
            private set
            {
                if (brush != value)
                {
                    brush = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// File - full path (hidden)
        /// </summary>
        private string oldFullPath;

        public string OldFullPath
        {
            get => oldFullPath;
            set
            {
                if (oldFullPath != value)
                {
                    oldFullPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// File - file name shown in cell
        /// </summary>
        private string oldViewPath;

        public string OldViewPath
        {
            get => oldViewPath;
            set
            {
                if (oldViewPath != value)
                {
                    oldViewPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// New file - full path (hidden)
        /// </summary>
        private string newFullPath;

        public string NewFullPath
        {
            get => newFullPath;
            set
            {
                if (newFullPath != value)
                {
                    newFullPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// New file - file name shown in cell
        /// </summary>
        private string newViewPath;

        public string NewViewPath
        {
            get => newViewPath;
            set
            {
                if (newViewPath != value)
                {
                    newViewPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Rename file
        /// </summary>
        /// <returns>success or failure</returns>
        public bool Rename()
        {
            try
            {
                if (fileNameProcessor.Rename(OldFullPath, NewFullPath))
                {
                    return true;
                }
            }
            catch (SystemException e)
            {
                OnException(e);
                MessageBox.Show(e.Message, "Error");
            }
            catch (Exception e)
            {
                OnException(e);
            }

            return false;
        }

        private void OnException(Exception e)
        {
            Debug.WriteLine(e);
            Brush = errorBrush;
        }

        private bool startWithUpperCase;

        public bool StartWithUpperCase
        {
            get => startWithUpperCase;
            set
            {
                startWithUpperCase = value;
                OnPropertyChanged();
            }
        }

        private bool removeBrackets;

        public bool RemoveBrackets
        {
            get => removeBrackets;
            set
            {
                removeBrackets = value;
                OnPropertyChanged();
            }
        }

        private bool removeStartingNumber;

        public bool RemoveStartingNumber
        {
            get => removeStartingNumber;
            set
            {
                removeStartingNumber = value;
                OnPropertyChanged();
            }
        }

        private bool showExtension;

        public bool ShowExtension
        {
            get => showExtension;
            set
            {
                showExtension = value;
                OnPropertyChanged();
            }
        }

        private bool showFullPath;

        public bool ShowFullPath
        {
            get => showFullPath;
            set
            {
                showFullPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Value changed through GUI
        /// </summary>
        /// <param name="type">value type</param>
        private void ValuesChanged(EditType type)
        {
            if (isEditing)
            {
                return;
            }

            isEditing = true;

            switch (type)
            {
                case EditType.FileName:
                    if (mainViewModel.ShowExtension)
                    {
                        NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar +
                                      Path.GetFileName(NewViewPath);
                    }
                    else
                    {
                        NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar +
                                      Path.GetFileNameWithoutExtension(NewViewPath) + Path.GetExtension(NewFullPath);
                    }

                    break;

                case EditType.UpperCase:
                case EditType.Brackets:
                case EditType.StartingNumber:
                    NewFullPath = fileNameProcessor.AutoRename(OldFullPath, StartWithUpperCase, RemoveBrackets, RemoveStartingNumber);
                    NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath, ShowExtension, ShowFullPath);
                    break;

                case EditType.Visual:
                    OldViewPath = fileNameProcessor.ApplyVisualRules(OldFullPath, ShowExtension, ShowFullPath);
                    NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath, ShowExtension, ShowFullPath);
                    break;
            }

            isEditing = false;
        }
    }
}