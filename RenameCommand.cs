using System;
using System.Windows.Input;

namespace AutoRename
{
	public class RenameCommand : ICommand
	{
		private readonly MainViewModel model;

		public RenameCommand(MainViewModel model)
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
			model.RenameAll();
		}
	}
}
