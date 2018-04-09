using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AutoRename
{
	class WebsiteCommand : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			Process.Start(Properties.Resources.HomePage);
		}
	}
}
