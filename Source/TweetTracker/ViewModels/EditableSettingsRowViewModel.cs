using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TweetTracker.ViewModels
{
    class EditableSettingsRowViewModel : BaseViewModel
    {
        public EditableSettingsRowViewModel(ObservableCollection<EditableSettingsRowViewModel> editRowList)
        {
            this.DeleteCommand = new RelayCommand(() => editRowList.Remove(this));
        }

        public string Key { get; set; }

        public string Values { get; set; }

        public ICommand DeleteCommand { get; private set; }

        public void RepairCommand(ObservableCollection<EditableSettingsRowViewModel> editRowList)
        {
            this.DeleteCommand = new RelayCommand(() => editRowList.Remove(this));
            this.OnPropertyChanged("DeleteCommand");
        }
    }
}
