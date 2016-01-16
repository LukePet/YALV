using System;
using System.Diagnostics;
using YALV.Common.Interfaces;

namespace YALV.Common
{
    public class CommandRelay
        : ICommandAncestor
    {
        readonly protected Func<object, object> _execute;
        readonly protected Predicate<object> _canExecute;

        public CommandRelay(Func<object, object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;
      
        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            if (null != CanExecuteChanged)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
