using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoRename
{
	/// <summary>
	/// GUI edit type
	/// </summary>
	public enum EditType
	{
		FileName,
		UpperCase,
		Brackets,
		StartingNumber,
		Visual
	}

    /// <summary>
    /// Main GUI
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
	    private readonly FileNameProcessor fileNameProcessor;

		public MainViewModel(FileNameProcessor fileNameProcessor)
		{
			this.fileNameProcessor = fileNameProcessor;
		}

		public event PropertyChangedEventHandler PropertyChanged = (sender, args) =>
	    {
		    MainViewModel model = (MainViewModel) sender;

		    switch (args.PropertyName)
		    {
		        case "SelectedItem":
		        {
		            model.CheckGuiStates();
		            break;
                }
			    case "StartWithUpperCase":
				{
					bool value = model.StartWithUpperCase;
					foreach (var row in model.DataGridRows)
						row.StartWithUpperCase = value;
					break;
                }
			    case "RemoveBrackets":
				{
					bool value = model.RemoveBrackets;
					foreach (var row in model.DataGridRows)
						row.RemoveBrackets = value;
					break;
                }
				case "RemoveStartingNumber":
				{
					bool value = model.RemoveStartingNumber;
					foreach (var row in model.DataGridRows)
						row.RemoveStartingNumber = value;
					break;
		        }
		        case "ShowFullPath":
		        {
		            bool value = model.showFullPath;
		            foreach (var row in model.DataGridRows)
		                row.ShowFullPath = value;
                    break;
		        }
		        case "ShowExtension":
		        {
		            bool value = model.ShowExtension;
		            foreach (var row in model.DataGridRows)
		                row.ShowExtension = value;
                        break;
                }
		    }
	    };

	    private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	    /// <summary>
	    /// DataGrid Rows
	    /// </summary>
	    private ObservableCollection<GridRowViewModel> dataGridRows = new ObservableCollection<GridRowViewModel>();
	    public ObservableCollection<GridRowViewModel> DataGridRows
	    {
		    get { return this.dataGridRows; }
		    set
		    {
			    this.dataGridRows = value;
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
	            string name = Utility.CurrentApplication.Name;
	            Version version = Utility.CurrentApplication.Version;
	            return $"{name} {version.Major}.{version.Minor}{(version.Build > 0 ? "." + version.Build : "")}";
            }
        }

	    /// <summary>
	    /// Website button caption
	    /// </summary>
	    private string websiteButton = Properties.Resources.WebsiteButtonVisit;
	    public string WebsiteButton
	    {
		    get { return this.websiteButton; }
			set
			{
				this.websiteButton = value;
				OnPropertyChanged("WebsiteButton");
			}
	    }

	    /// <summary>
	    /// Selected row in DagaGrid
	    /// </summary>
	    private GridRowViewModel selectedItem;
	    public GridRowViewModel SelectedItem
	    {
		    get { return this.selectedItem; }
			set
			{
				this.selectedItem = value;			
				OnPropertyChanged("SelectedItem");
			}
	    }

	    /// <summary>
	    /// Start wit upper case checkbox
	    /// </summary>
	    public bool StartWithUpperCase
	    {
			get { return fileNameProcessor.StartWithUpperCase; }
		    set
		    {
				fileNameProcessor.StartWithUpperCase = value;
				OnPropertyChanged("StartWithUpperCase");
		    }
	    }

	    /// <summary>
	    /// Remove brackets
		/// </summary>
	    public bool RemoveBrackets
	    {
			get { return fileNameProcessor.RemoveBrackets; }
		    set
			{
				fileNameProcessor.RemoveBrackets = value;
			    OnPropertyChanged("RemoveBrackets");

		    }
	    }

		public bool RemoveStartingNumber
		{
			get { return fileNameProcessor.RemoveStartingNumber; }
			set
			{
				fileNameProcessor.RemoveStartingNumber = value;
				OnPropertyChanged("RemoveStartingNumber");
			}
		}

        /// <summary>
        /// Show extension checkbox
        /// </summary>
        private bool showExtension;
        public bool ShowExtension
	    {
		    get { return showExtension; }
		    set
		    {
		        showExtension = value;
				OnPropertyChanged("ShowExtension");
		    }
	    }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        private bool showFullPath;
        public bool ShowFullPath
	    {
		    get { return showFullPath; }
		    set
		    {
		        showFullPath = value;
				OnPropertyChanged("ShowFullPath");
		    }
	    }

	    /// <summary>
	    /// Show full path checkbox
	    /// </summary>
	    private bool showGridLines;
	    public bool ShowGridLines
	    {
		    get { return this.showGridLines; }
		    set
		    {
		        this.showGridLines = value;
			    OnPropertyChanged("ShowGridLines");
		        OnPropertyChanged("GridLinesVisibility");
            }
	    }

        public DataGridGridLinesVisibility GridLinesVisibility
        {
            get { return this.ShowGridLines ? DataGridGridLinesVisibility.All : DataGridGridLinesVisibility.None; }
        }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        private bool exitAfterRename;
        public bool ExitAfterRename
        {
            get { return this.exitAfterRename; }
            set
            {
                this.exitAfterRename = value;
                OnPropertyChanged("ExitAfterRename");
            }
        }

        /// <summary>
        /// Rename button
        /// </summary>
        private RenameCommand renameButtonCommand;
	    public ICommand RenameButtonClick
	    {
		    get
		    {
			    if (this.renameButtonCommand == null)
				    this.renameButtonCommand = new RenameCommand(this);

			    return this.renameButtonCommand;
		    }
	    }

	    /// <summary>
	    /// Add file button
	    /// </summary>
	    private AddFileCommand addFileCommand;
	    public ICommand AddFileButtonClick
	    {
		    get
		    {
			    if (this.addFileCommand == null)
				    this.addFileCommand = new AddFileCommand(this);

			    return this.addFileCommand;
		    }
	    }

	    /// <summary>
	    /// Website button
	    /// </summary>
	    private WebsiteCommand websiteCommand;
	    public ICommand WebsiteButtonClick
	    {
		    get
		    {
			    if (this.websiteCommand == null)
				    this.websiteCommand = new WebsiteCommand();

			    return this.websiteCommand;
		    }
	    }

	    /// <summary>
	    /// Rename button Enabled / Disabled
	    /// </summary>
	    private bool renameButtonEnabled;
	    public bool RenameButtonEnabled
	    {
		    get { return this.renameButtonEnabled; }
		    set
		    {
			    this.renameButtonEnabled = value;
			    OnPropertyChanged("RenameButtonEnabled");
		    }
	    }

	    private bool contextMenuEnabled;
		public bool ContextMenuEnabled
	    {
			get { return this.contextMenuEnabled; }
			set
			{
				this.contextMenuEnabled = value;
				OnPropertyChanged("ContextMenuEnabled");
			}
		}

		private Visibility rowSettingsEnabled = Visibility.Collapsed;
		public Visibility RowSettingsEnabled
		{
			get { return this.rowSettingsEnabled; }
			set
			{
				this.rowSettingsEnabled = value;
				OnPropertyChanged("RowSettingsEnabled");
			}
		}

	    /// <summary>
	    /// Add files to table
	    /// </summary>
	    /// <param name="files">file collection</param>
	    public void AddFiles(IEnumerable<string> files)
	    {
		    foreach (string file in files)
			    AddFile(file);
	    }

	    /// <summary>
	    /// Add file to table
	    /// </summary>
	    /// <param name="file">file path</param>
	    public void AddFile(string file)
	    {
		    if (FileAlreadyLoaded(file))
			    return;

		    try
		    {
			    GridRowViewModel newRow = new GridRowViewModel(this, file, fileNameProcessor);
			    DataGridRows.Add(newRow);
			    CheckGuiStates();
		    }
		    catch
		    {
			    SystemSounds.Beep.Play();
		    }
	    }

		private void CheckGuiStates()
		{
			RenameButtonEnabled = ContextMenuEnabled = DataGridRows.Count > 0;
			RowSettingsEnabled = SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
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
		    CheckGuiStates();
	    }

	    public void RemoveAll()
	    {
			DataGridRows = new ObservableCollection<GridRowViewModel>();
		    CheckGuiStates();
	    }

	    public void RenameAll()
	    {
		    var list = DataGridRows;
	        int listCount = list.Count;

            if (listCount == 0)
			    return;

		    bool success = true;

	        for (int i = 0; i < listCount; i++)
		    {
			    GridRowViewModel row = list[i];
			    bool renamed = row.Rename();
		        if (renamed)
		        {
		            DataGridRows.Remove(row);
                }
			    success &= renamed;
		    }

		    if (success)
		    {
		        if (ExitAfterRename)
		        {
		            Application.Current.Shutdown();
                }
		    }
		    else
		    {
			    SystemSounds.Beep.Play();
		    }
	    }
	}
}
