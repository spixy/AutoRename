using System.ComponentModel;
using System.IO;
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
			OldViewPath = fileNameProcessor.ApplyVisualRules(OldFullPath);
			NewFullPath = fileNameProcessor.AutoRename(OldFullPath);
			NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath);

			isEditing = false;

			this.startWithUpperCase = fileNameProcessor.StartWithUpperCase;
			this.removeBrackets = fileNameProcessor.RemoveBrackets;
			this.removeStartingNumber = fileNameProcessor.RemoveStartingNumber;
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
			if (fileNameProcessor.Rename(OldFullPath, NewFullPath))
			{
				return true;
			}
			else
			{
				Brush = errorBrush;
				return false;
			}
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

		/// <summary>
		/// Value changed through GUI
		/// </summary>
		/// <param name="type">value type</param>
		public void ValuesChanged(EditType type)
		{
			if (isEditing)
				return;

			isEditing = true;

			switch (type)
			{
				case EditType.FileName:
					if (mainViewModel.ShowExtension)
						NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + NewViewPath;
					else
						NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(NewViewPath) + Path.GetExtension(NewFullPath);
					break;

				case EditType.UpperCase:
				case EditType.Brackets:
				case EditType.StartingNumber:
					NewFullPath = fileNameProcessor.AutoRename(OldFullPath, StartWithUpperCase, RemoveBrackets, RemoveStartingNumber);
					NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath);			
					break;

				case EditType.Visual:
					OldViewPath = fileNameProcessor.ApplyVisualRules(OldFullPath);
					NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath);
					break;
			}

			isEditing = false;
		}
    }
}
