using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Windows;
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
		Visual
	}

    /// <summary>
    /// Main GUI
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
	{
		private static FileNameProcessor fileNameProcessor { get { return FileNameProcessor.Instance; } }

	    public event PropertyChangedEventHandler PropertyChanged = (sender, args) =>
	    {
		    MainViewModel model = (MainViewModel) sender;

		    switch (args.PropertyName)
		    {
			    case "SelectedItem":
				    model.CheckGuiStates();
				    break;

			    case "StartWithUpperCase":
					{
						bool value = model.StartWithUpperCase;

						foreach (var row in model.DataGridRows)
							row.StartWithUpperCase = value;
					}
				    break;

			    case "RemoveBrackets":
					{
						bool value = model.RemoveBrackets;

						foreach (var row in model.DataGridRows)
						    row.RemoveBrackets = value;
					}
				    break;

			    case "ShowExtension":
			    case "ShowFullPath":
				    foreach (var row in model.DataGridRows)
					    row.ValuesChanged(EditType.Visual);
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

	    /// <summary>
	    /// DataGrid Rows
	    /// </summary>
	    private ObservableCollection<GridRowViewModel> _dataGridRows = new ObservableCollection<GridRowViewModel>();
	    public ObservableCollection<GridRowViewModel> DataGridRows
	    {
		    get { return _dataGridRows; }
		    set
		    {
			    _dataGridRows = value;
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

	    /// <summary>
	    /// Website button caption
	    /// </summary>
	    private string _websiteButton = Properties.Resources.WebsiteButtonVisit;
	    public string WebsiteButton
	    {
		    get { return _websiteButton; }
			set
			{
				_websiteButton = value;
				OnPropertyChanged("WebsiteButton");
			}
	    }

	    /// <summary>
	    /// Selected row in DagaGrid
	    /// </summary>
	    private GridRowViewModel _selectedItem;
	    public GridRowViewModel SelectedItem
	    {
		    get { return _selectedItem; }
			set
			{
				_selectedItem = value;			
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

	    /// <summary>
	    /// Show extension checkbox
	    /// </summary>
	    public bool ShowExtension
	    {
		    get { return fileNameProcessor.ShowExtension; }
		    set
		    {
				fileNameProcessor.ShowExtension = value;
				OnPropertyChanged("ShowExtension");
		    }
	    }

	    /// <summary>
	    /// Show full path checkbox
	    /// </summary>
	    public bool ShowFullPath
	    {
		    get { return fileNameProcessor.ShowFullPath; }
		    set
		    {
				fileNameProcessor.ShowFullPath = value;
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
				    RenameButtonCommand = new RenameCommand(this);

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
				    AddFileCommand = new AddFileCommand(this);

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
				    WebsiteCommand = new WebsiteCommand();

			    return WebsiteCommand;
		    }
	    }

	    /// <summary>
	    /// Rename button Enabled / Disabled
	    /// </summary>
	    private bool _renameButtonEnabled;
	    public bool RenameButtonEnabled
	    {
		    get { return _renameButtonEnabled; }
		    set
		    {
			    _renameButtonEnabled = value;
			    OnPropertyChanged("RenameButtonEnabled");
		    }
	    }

	    private bool _contextMenuEnabled;
		public bool ContextMenuEnabled
	    {
			get { return _contextMenuEnabled; }
			set
			{
				_contextMenuEnabled = value;
				OnPropertyChanged("ContextMenuEnabled");
			}
		}

		private Visibility _rowSettingsEnabled = Visibility.Collapsed;
		public Visibility RowSettingsEnabled
		{
			get { return _rowSettingsEnabled; }
			set
			{
				_rowSettingsEnabled = value;
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
			    GridRowViewModel newRow = new GridRowViewModel(this, file);
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
