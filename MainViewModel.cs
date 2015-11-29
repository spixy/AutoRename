using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace QuickRename
{
    /// <summary>
    /// Main GUI
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// DataGrid Source
        /// </summary>
        public ICollectionView DataGridRows { get; set; }

        /// <summary>
        /// DataGrid Source as List
        /// </summary>
        public List<GridRowViewModel> DataGridRowsList
        {
            get
            {
                List<GridRowViewModel> list = new List<GridRowViewModel>();

                if (DataGridRows != null)
                {
                    foreach (GridRowViewModel row in DataGridRows.SourceCollection)
                        list.Add(row);
                }

                return list;
            }
            set
            {
                DataGridRows = CollectionViewSource.GetDefaultView(value);
                OnPropertyChanged("DataGridRows");
            }
        }

        /// <summary>
        /// Window title
        /// </summary>
        public string Title
        {
            get
            {
                return "Quick Rename " + Utility.AppVersion.Major + "." + Utility.AppVersion.Minor + "." + Utility.AppVersion.Revision;
            }
        }

        /// <summary>
        /// Website button caption
        /// </summary>
        public string WebsiteButton { get; set; }

        /// <summary>
        /// Selected row in DagaGrid
        /// </summary>
        private GridRowViewModel _SelectedItem;
        public GridRowViewModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
       }

        /// <summary>
        /// Overwrite checkbox
        /// </summary>
        private bool _Overwrite;
        public bool Overwrite
        {
            get
            {
                return _Overwrite;
            }
            set
            {
                _Overwrite = value;
                OnPropertyChanged("Overwrite");
            }
        }

        /// <summary>
        /// Start wit upper case checkbox
        /// </summary>
        private bool _StartWithUpperCase;
        public bool StartWithUpperCase
        {
            get
            {
                return _StartWithUpperCase;
            }
            set
            {
                _StartWithUpperCase = value;
                OnPropertyChanged("StartWithUpperCase");
            }
        }

        /// <summary>
        /// Show extension checkbox
        /// </summary>
        private bool _ShowExtension;
        public bool ShowExtension
        {
            get
            {
                return _ShowExtension;
            }
            set
            {
                _ShowExtension = value;
                OnPropertyChanged("ShowExtension");
            }
        }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        private bool _ShowFullPath;
        public bool ShowFullPath
        {
            get
            {
                return _ShowFullPath;
            }
            set
            {
                _ShowFullPath = value;
                OnPropertyChanged("ShowFullPath");
            }
        }

    }
}
