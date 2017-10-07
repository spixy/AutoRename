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
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
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

			this.startWithUpperCase = fileNameProcessor.StartWithUpperCase;
			this.removeBrackets = fileNameProcessor.RemoveBrackets;
			this.removeStartingNumber = fileNameProcessor.RemoveStartingNumber;
		    this.ShowFullPath = mainViewModel.ShowFullPath;
		    this.ShowExtension = mainViewModel.ShowExtension;
        }

	    /// <summary>
	    /// Background brush
	    /// </summary>
	    private SolidColorBrush brush = normalBrush;
	    public SolidColorBrush Brush
	    {
		    get { return this.brush; }
			private set
			{
				this.brush = value;
				OnPropertyChanged("Brush");
			}
	    }

	    /// <summary>
        /// File - full path (hidden)
        /// </summary>
        private string oldFullPath;
	    public string OldFullPath
	    {
		    get { return this.oldFullPath; }
			set
			{
				this.oldFullPath = value;
				OnPropertyChanged("OldFullPath");
			}
	    }

	    /// <summary>
        /// File - file name shown in cell
        /// </summary>
        private string oldViewPath;
	    public string OldViewPath
	    {
		    get { return this.oldViewPath; }
			set
			{
				this.oldViewPath = value;
				OnPropertyChanged("OldViewPath");
			}
	    }

	    /// <summary>
        /// New file - full path (hidden)
        /// </summary>
        private string newFullPath;
	    public string NewFullPath
	    {
		    get { return this.newFullPath; }
			set
			{
				this.newFullPath = value;
				OnPropertyChanged("NewFullPath");
			}
	    }

	    /// <summary>
        /// New file - file name shown in cell
        /// </summary>
        private string newViewPath;
	    public string NewViewPath
	    {
		    get { return this.newViewPath; }
		    set
		    {
			    this.newViewPath = value;
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
			get { return this.startWithUpperCase; }
			set
			{
				this.startWithUpperCase = value;
				OnPropertyChanged("StartWithUpperCase");
			}
		}

		private bool removeBrackets;
		public bool RemoveBrackets
		{
			get { return this.removeBrackets; }
			set
			{
				this.removeBrackets = value;
				OnPropertyChanged("RemoveBrackets");
			}
		}

	    private bool removeStartingNumber;
		public bool RemoveStartingNumber
		{
			get { return this.removeStartingNumber; }
			set
			{
				this.removeStartingNumber = value;
				OnPropertyChanged("RemoveStartingNumber");
			}
		}


        private bool showExtension;
        public bool ShowExtension
        {
            get { return this.showExtension; }
            set
            {
                this.showExtension = value;
                OnPropertyChanged("ShowExtension");
            }
        }

        private bool showFullPath;
        public bool ShowFullPath
        {
            get { return this.showFullPath; }
            set
            {
                this.showFullPath = value;
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
				return;

			isEditing = true;

			switch (type)
			{
				case EditType.FileName:
					if (mainViewModel.ShowExtension)
						NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileName(NewViewPath);
					else
						NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(NewViewPath) + Path.GetExtension(NewFullPath);
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
