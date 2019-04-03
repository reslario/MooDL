using System;
using System.Windows.Input;

namespace MooDL.Models.Input
{
    internal class CommandHandler : ICommand
    {
        private readonly Action action;

        public CommandHandler(Action action) => this.action = action;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => action();

        public event EventHandler CanExecuteChanged;
    }
}