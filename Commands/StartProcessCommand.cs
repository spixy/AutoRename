using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AutoRename.Commands
{
    public class StartProcessCommand : ICommand
    {
        private readonly string fileName;

        public StartProcessCommand(string fileName)
        {
            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Process.Start(fileName);
        }
    }
}