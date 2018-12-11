using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

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
			GridRowViewModel model = (GridRowViewModel)sender;

			switch (args.PropertyName)
			{
				case "NewViewPath":
					model.ValuesChanged(EditType.FileName);
					break;

				case "StartWithUpperCase":
					model.ValuesChanged(EditType.UpperCase);
					break;

				case "RemoveBrackets":
					model.ValuesChanged(EditType.Brackets);
					break;

				case "RemoveStartingNumber":
					model.ValuesChanged(EditType.StartingNumber);
					break;

			    case "ShowExtension":
			    case "ShowFullPath":
                    model.ValuesChanged(EditType.Visual);
			        break;
            }
		};

		private void OnPropertyChanged(string propertyName)
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
				brush = value;
				OnPropertyChanged("Brush");
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
				oldFullPath = value;
				OnPropertyChanged("OldFullPath");
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
				oldViewPath = value;
				OnPropertyChanged("OldViewPath");
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
				newFullPath = value;
				OnPropertyChanged("NewFullPath");
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
			    newViewPath = value;
				OnPropertyChanged("NewViewPath");
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
				OnPropertyChanged("StartWithUpperCase");
			}
		}

		private bool removeBrackets;
		public bool RemoveBrackets
		{
			get => removeBrackets;
			set
			{
				removeBrackets = value;
				OnPropertyChanged("RemoveBrackets");
			}
		}

	    private bool removeStartingNumber;
		public bool RemoveStartingNumber
		{
			get => removeStartingNumber;
			set
			{
				removeStartingNumber = value;
				OnPropertyChanged("RemoveStartingNumber");
			}
		}


        private bool showExtension;
        public bool ShowExtension
        {
            get => showExtension;
	        set
            {
                showExtension = value;
                OnPropertyChanged("ShowExtension");
            }
        }

        private bool showFullPath;
        public bool ShowFullPath
        {
            get => showFullPath;
	        set
            {
                showFullPath = value;
                OnPropertyChanged("ShowFullPath");
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
						NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileName(NewViewPath);
					}
					else
					{
						NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(NewViewPath) + Path.GetExtension(NewFullPath);
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
