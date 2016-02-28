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

		private static FileNameProcessor fileNameProcessor { get { return FileNameProcessor.Instance; } }

		private readonly MainViewModel mainViewModel;

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
			}
		};

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public GridRowViewModel(MainViewModel mainViewModel, string file)
		{
		    this.mainViewModel = mainViewModel;

			isEditing = true;

			OldFullPath = file;
			OldViewPath = fileNameProcessor.ApplyVisualRules(OldFullPath);
			NewFullPath = fileNameProcessor.QRename(file);
			NewViewPath = fileNameProcessor.ApplyVisualRules(NewFullPath);

			isEditing = false;
	    }

	    /// <summary>
	    /// Background brush
	    /// </summary>
	    private SolidColorBrush _brush = normalBrush;
	    public SolidColorBrush Brush
	    {
		    get { return _brush; }
			private set
			{
				_brush = value;
				OnPropertyChanged("Brush");
			}
	    }

	    /// <summary>
        /// File - full path (hidden)
        /// </summary>
        private string _oldFullPath;
	    public string OldFullPath
	    {
		    get { return _oldFullPath; }
			set
			{
				_oldFullPath = value;
				OnPropertyChanged("OldFullPath");
			}
	    }

	    /// <summary>
        /// File - file name shown in cell
        /// </summary>
        private string _oldViewPath;
	    public string OldViewPath
	    {
		    get { return _oldViewPath; }
			set
			{
				_oldViewPath = value;
				OnPropertyChanged("OldViewPath");
			}
	    }

	    /// <summary>
        /// New file - full path (hidden)
        /// </summary>
        private string _newFullPath;
	    public string NewFullPath
	    {
		    get { return _newFullPath; }
			set
			{
				_newFullPath = value;
				OnPropertyChanged("NewFullPath");
			}
	    }

	    /// <summary>
        /// New file - file name shown in cell
        /// </summary>
        private string _newViewPath;
	    public string NewViewPath
	    {
		    get { return _newViewPath; }
		    set
		    {
			    _newViewPath = value;
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


	    private bool _startWithUpperCase = FileNameProcessor.Instance.StartWithUpperCase;
		public bool StartWithUpperCase
		{
			get { return _startWithUpperCase; }
			set
			{
				_startWithUpperCase = value;
				OnPropertyChanged("StartWithUpperCase");
			}
		}

		private bool _removeBrackets = FileNameProcessor.Instance.RemoveBrackets;
		public bool RemoveBrackets
		{
			get { return _removeBrackets; }
			set
			{
				_removeBrackets = value;
				OnPropertyChanged("RemoveBrackets");
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
					NewFullPath = fileNameProcessor.QRename(OldFullPath, StartWithUpperCase, RemoveBrackets);
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
