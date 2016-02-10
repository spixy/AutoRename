using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace AutoRename
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

	    public MainViewModel(FileNameProcessor fileNameProcessor)
	    {
		    FileNameProcessor = fileNameProcessor;
	    }

	    /// <summary>
		/// File Name Processor
		/// </summary>
		public FileNameProcessor FileNameProcessor { get; private set; }

	    /// <summary>
	    /// DataGrid Rows
	    /// </summary>
	    private ObservableCollection<GridRowViewModel> _DataGridRows = new ObservableCollection<GridRowViewModel>();
	    public ObservableCollection<GridRowViewModel> DataGridRows
	    {
		    get
		    {
			    return _DataGridRows;
		    }
		    set
		    {
			    _DataGridRows = value;
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
	            var name = Utility.CurrentApplication.Name;
	            var version = Utility.CurrentApplication.Version;

				return name + " " + version.Major + "." + version.Minor + (version.Build > 0 ? "." + version.Build : "");
            }
        }

		private void ApplyVisualRules()
		{
			foreach (var item in DataGridRows)
			{
				item.OldViewPath = FileNameProcessor.ApplyVisualRules(item.OldFullPath);
				item.NewViewPath = FileNameProcessor.ApplyVisualRules(item.NewFullPath);
			}
		}

		private void ApplyNamingRules()
		{
			foreach (var item in DataGridRows)
			{
				item.NewFullPath = FileNameProcessor.QRename(item.OldFullPath);
				item.NewViewPath = FileNameProcessor.ApplyVisualRules(item.NewFullPath);
			}
		}

	    /// <summary>
	    /// Website button caption
	    /// </summary>
	    private string _WebsiteButton = Properties.Resources.WebsiteButtonVisit;
		public string WebsiteButton
		{
			get
			{
				return _WebsiteButton;
			}
			set
			{
				_WebsiteButton = value;
				OnPropertyChanged("WebsiteButton");
			}
		}

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
        /// Start wit upper case checkbox
        /// </summary>
        public bool StartWithUpperCase
        {
            get
            {
				return FileNameProcessor.StartWithUpperCase;
            }
            set
            {
				FileNameProcessor.StartWithUpperCase = value;
	            ApplyNamingRules();

                OnPropertyChanged("StartWithUpperCase");
            }
        }

		/// <summary>
		/// Remove brackets
		/// </summary>
	    public bool RemoveBrackets
	    {
		    get
		    {
				return FileNameProcessor.RemoveBrackets;
		    }
		    set
		    {
				FileNameProcessor.RemoveBrackets = value;
				ApplyNamingRules();

				OnPropertyChanged("RemoveBrackets");
		    }
		}

        /// <summary>
        /// Show extension checkbox
        /// </summary>
        public bool ShowExtension
        {
            get
            {
				return FileNameProcessor.ShowExtension;
            }
            set
            {
				FileNameProcessor.ShowExtension = value;
				ApplyVisualRules();

                OnPropertyChanged("ShowExtension");
            }
        }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        public bool ShowFullPath
        {
            get
            {
				return FileNameProcessor.ShowFullPath;
            }
            set
            {
				FileNameProcessor.ShowFullPath = value;
				ApplyVisualRules();

                OnPropertyChanged("ShowFullPath");
            }
        }


		/// <summary>
		/// Rename button
		/// </summary>
		private RenameCommand RenameButtonCommand;
		public ICommand RenameButtonClick
		{
			get
			{
				if (RenameButtonCommand == null)
				{
					RenameButtonCommand = new RenameCommand(this);
				}

				return RenameButtonCommand;
			}
		}

		/// <summary>
		/// Add file button
		/// </summary>
		private AddFileCommand AddFileCommand;
		public ICommand AddFileButtonClick
		{
			get
			{
				if (AddFileCommand == null)
				{
					AddFileCommand = new AddFileCommand(this);
				}

				return AddFileCommand;
			}
		}

		/// <summary>
		/// Website button
		/// </summary>
		private WebsiteCommand WebsiteCommand;
		public ICommand WebsiteButtonClick
		{
			get
			{
				if (WebsiteCommand == null)
				{
					WebsiteCommand = new WebsiteCommand();
				}

				return WebsiteCommand;
			}
		}

	    /// <summary>
		/// Rename button Enabled / Disabled
	    /// </summary>
	    private bool _RenameButtonEnabled;
	    public bool RenameButtonEnabled
	    {
		    get
		    {
			    return _RenameButtonEnabled;
		    }
			set
			{
				_RenameButtonEnabled = value;
				OnPropertyChanged("RenameButtonEnabled");
			}
	    }

	    private void CheckButtonState()
	    {
			RenameButtonEnabled = DataGridRows.Count > 0;
	    }

		/// <summary>
		/// Add files to table
		/// </summary>
		/// <param name="files"></param>
	    public void AddFiles(IEnumerable<string> files)
	    {
			foreach (string file in files)
				AddFile(file);
	    }

		/// <summary>
		/// Add file to table
		/// </summary>
		/// <param name="file"></param>
	    public void AddFile(string file)
	    {
		    if (FileAlreadyLoaded(file))
			    return;

		    try
		    {
			    GridRowViewModel newRow = new GridRowViewModel(FileNameProcessor, this, file);
			    DataGridRows.Add(newRow);

			    CheckButtonState();
		    }
		    catch
		    {
			    SystemSounds.Beep.Play();
		    }
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

	    public void RemoveSelected()
	    {
			DataGridRows.Remove(SelectedItem);
			CheckButtonState();
	    }

		public void RemoveAll()
		{
			DataGridRows = new ObservableCollection<GridRowViewModel>();
			CheckButtonState();
		}

		public void RenameAll()
		{
			var list = DataGridRows;

			if (DataGridRows.Count == 0)
				return;

			bool success = true;

			for (int i = list.Count - 1; i >= 0; i--)
			{
				GridRowViewModel row = list[i];

				bool result = row.Rename();

				DataGridRows.Remove(row);

				success &= result;
			}

			if (success)
			{
				Application.Current.Shutdown();
			}
			else
			{
				SystemSounds.Beep.Play();
			}
		}
    }
}
