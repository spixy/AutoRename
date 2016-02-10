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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		public GridRowViewModel(FileNameProcessor fileNameProcessor, MainViewModel mainViewModel, string file)
		{
			FileNameProcessor = fileNameProcessor;
		    MainViewModel = mainViewModel;

			OldFullPath = file;
			OldViewPath = FileNameProcessor.ApplyVisualRules(OldFullPath);

			NewFullPath = FileNameProcessor.QRename(file);
			NewViewPath = FileNameProcessor.ApplyVisualRules(NewFullPath);
	    }

		/// <summary>
		/// File Name Processor
		/// </summary>
		public FileNameProcessor FileNameProcessor { get; private set; }

		public MainViewModel MainViewModel { get; private set; }

	    /// <summary>
	    /// Background brush
	    /// </summary>
	    private SolidColorBrush _Brush = normalBrush;
        public SolidColorBrush Brush
        {
            get
            {
				return _Brush;
            }
	        private set
	        {
		        _Brush = value;
				OnPropertyChanged("Brush");
	        }
        }

        /// <summary>
        /// Error flag (hidden)
		/// </summary>
		private bool _Error { get; set; }
		public bool Error
		{
			get
			{
				return _Error;
			}
			private set
			{
				_Error = value;
				Brush = value ? errorBrush : normalBrush;
				OnPropertyChanged("Error");
			}
		}

        /// <summary>
        /// File - full path (hidden)
        /// </summary>
        private string _OldFullPath;
        public string OldFullPath
        {
            get
            {
                return _OldFullPath;
            }
            set
            {
                _OldFullPath = value;
                OnPropertyChanged("OldFullPath");
            }
        }

        /// <summary>
        /// File - file name shown in cell
        /// </summary>
        private string _OldViewPath;
        public string OldViewPath
        {
            get
            {
                return _OldViewPath;
            }
            set
            {
                _OldViewPath = value;
                OnPropertyChanged("OldViewPath");
            }
        }

        /// <summary>
        /// New file - full path (hidden)
        /// </summary>
        private string _NewFullPath;
        public string NewFullPath
        {
            get
            {
                return _NewFullPath;
            }
            set
            {
                _NewFullPath = value;
                OnPropertyChanged("NewFullPath");
            }
        }

        /// <summary>
        /// New file - file name shown in cell
        /// </summary>
        private string _NewViewPath;
        public string NewViewPath
        {
            get
            {
                return _NewViewPath;
            }
            set
            {
                _NewViewPath = value;
	            FileNameChanged();
                OnPropertyChanged("NewViewPath");
            }
        }

		private void FileNameChanged()
		{
			if (MainViewModel.ShowExtension)
			{
				NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + NewViewPath;
			}
			else
			{
				NewFullPath = Path.GetDirectoryName(NewFullPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(NewViewPath) + Path.GetExtension(NewFullPath);
			}
		}

		public bool Rename()
		{
			if (FileNameProcessor.Rename(OldFullPath, NewFullPath))
			{
				return true;
			}
			else
			{
				Error = true;
				return false;
			}
		}
    }
}
