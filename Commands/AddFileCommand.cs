using System;
using System.Windows.Input;
using Microsoft.Win32;

namespace AutoRename
{
	public class AddFileCommand : ICommand
	{
		private readonly Lazy<OpenFileDialog> openFileDialog = new Lazy<OpenFileDialog>(() =>
		{
			var _openFileDialog = new OpenFileDialog { Multiselect = true };
			return _openFileDialog;
		});

		private readonly MainViewModel model;

		public AddFileCommand(MainViewModel model)
		{
			this.model = model;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			bool? showDialog = openFileDialog.Value.ShowDialog();

			if (showDialog != null && showDialog.Value)
			{
				model.AddFiles(openFileDialog.Value.FileNames);
			}
		}
	}
}
