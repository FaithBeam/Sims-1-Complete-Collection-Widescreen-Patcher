using System;
using System.Windows.Input;

namespace Sims1WidescreenPatcher.UI.WPF
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Func<bool> _func;

        public DelegateCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
        }
        
        public DelegateCommand(Action<object> executeAction, Func<bool> func)
        {
            _executeAction = executeAction;
            _func = func;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this,EventArgs.Empty);
        }

        public void Execute(object parameter) => _executeAction(parameter);

        public bool CanExecute(object parameter)
        {
            return _func == null || _func();
        }

        public event EventHandler CanExecuteChanged;
    }
}
