using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using AutoRename.Services;

namespace AutoRename
{
    /// <summary>
    /// Row in DataGrid
    /// </summary>
    public class GridRowViewModel : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, EditType> propertyMap = new Dictionary<string, EditType>
        {
            {nameof(NewViewPath), EditType.FileName},
            {nameof(StartWithUpperCase), EditType.UpperCase},
            {nameof(RemoveBrackets), EditType.Brackets},
            {nameof(RemoveStartingNumber), EditType.StartingNumber},
            {nameof(ShowExtension), EditType.Visual},
            {nameof(ShowFullPath), EditType.Visual}
        };

        private readonly MainViewModel mainViewModel;
        private readonly FileNameProcessor fileNameProcessor;

        private string oldViewPath;
        private string newFullPath;
        private string oldFullPath;
        private string newViewPath;
        private bool startWithUpperCase;
        private bool removeBrackets;
        private bool removeStartingNumber;
        private bool showExtension;
        private bool showFullPath;
        private bool isEditing;
        private RowState state = RowState.Ready;

        public event PropertyChangedEventHandler PropertyChanged;

        public GridRowViewModel(MainViewModel mainViewModel, string file, FileNameProcessor fileNameProcessor)
        {
            this.mainViewModel = mainViewModel;
            this.fileNameProcessor = fileNameProcessor;

            StartWithUpperCase = mainViewModel.StartWithUpperCase;
            RemoveBrackets = mainViewModel.RemoveBrackets;
            RemoveStartingNumber = mainViewModel.RemoveStartingNumber;
            ShowFullPath = mainViewModel.ShowFullPath;
            ShowExtension = mainViewModel.ShowExtension;

            OldFullPath = file;
            OldViewPath = fileNameProcessor.ApplyVisualRules(OldFullPath, ShowExtension, ShowFullPath);
            NewFullPath = fileNameProcessor.GetNewRename(OldFullPath, StartWithUpperCase, RemoveBrackets, RemoveStartingNumber);
            NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath, ShowExtension, ShowFullPath);

            PropertyChanged += (sender, args) =>
            {
                if (propertyMap.TryGetValue(args.PropertyName, out EditType editType))
                {
                    ValuesChanged(editType);
                }
            };
        }

        /// <summary>
        /// Row state
        /// </summary>
        public RowState State
        {
            get => state;
            private set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// File - full path (hidden)
        /// </summary>
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
        public string NewViewPath
        {
            get => newViewPath;
            set
            {
                if (newViewPath != value)
                {
                    newViewPath = value;
                    OnPropertyChanged();
                    State = RowState.Ready;
                }
            }
        }

        public bool StartWithUpperCase
        {
            get => startWithUpperCase;
            set
            {
                startWithUpperCase = value;
                OnPropertyChanged();
            }
        }

        public bool RemoveBrackets
        {
            get => removeBrackets;
            set
            {
                removeBrackets = value;
                OnPropertyChanged();
            }
        }
        public bool RemoveStartingNumber
        {
            get => removeStartingNumber;
            set
            {
                removeStartingNumber = value;
                OnPropertyChanged();
            }
        }

        public bool ShowExtension
        {
            get => showExtension;
            set
            {
                showExtension = value;
                OnPropertyChanged();
            }
        }

        public bool ShowFullPath
        {
            get => showFullPath;
            set
            {
                showFullPath = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Rename file
        /// </summary>
        /// <returns>success or failure</returns>
        public bool Rename()
        {
            try
            {
                if (fileNameProcessor.Rename(OldFullPath, NewFullPath, mainViewModel.ForceOverwrite))
                {
                    State = RowState.Renamed;
                    return true;
                }
            }
            catch (SystemException e)
            {
                MessageBox.Show(e.Message, "Error");
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            State = RowState.Error;
            return false;
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
                        NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileName(NewViewPath);
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
                    NewFullPath = fileNameProcessor.GetNewRename(OldFullPath, StartWithUpperCase, RemoveBrackets, RemoveStartingNumber);
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

    /// <summary>
    /// GridRowState
    /// </summary>
    public enum RowState
    {
        Ready,
        Error,
        Renamed
    }
}