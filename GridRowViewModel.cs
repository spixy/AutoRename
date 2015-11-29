using System.ComponentModel;
using System.Windows.Media;

namespace QuickRename
{
    /// <summary>
    /// Row in DataGrid
    /// </summary>
    public class GridRowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static readonly SolidColorBrush normalBrush = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush errorBrush = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Background brush
        /// </summary>
        public SolidColorBrush Brush
        {
            get
            {
                return (Error) ? errorBrush : normalBrush;
            }
        }

        /// <summary>
        /// Error flag (hidden)
        /// </summary>
        public bool Error { get; set; }

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
                OnPropertyChanged("NewViewPath");
            }
        }
    }
}
