using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TweetTracker
{
    class RelayCommand : ICommand 
    {
        private Action<object> _execute;

        private Predicate<object> _canExecute;

        public RelayCommand(Action execute)
        {
            this._execute = obj => execute();
        }

        public RelayCommand(Action execute, Predicate<object> canExecute) : this(execute)
        {
            this._canExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }
            else
            {
                return this._canExecute(parameter);
            }
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this._execute(parameter);
            }
        }
    }
}
