using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetTracker.ViewModels
{
    class CaptureSettingsViewModel : BaseViewModel
    {
        private string _captureHashTag;

        private ObservableCollection<EditableSettingsRowViewModel> _settingRows;

        public CaptureSettingsViewModel()
        {
            this._settingRows = new ObservableCollection<EditableSettingsRowViewModel>();

            this.AddCommand = new RelayCommand(() => this.SettingsRow.Add(new EditableSettingsRowViewModel(this.SettingsRow)));
        }


        public string HashTag
        {
            get
            {
                return this._captureHashTag;
            }

            set
            {
                this._captureHashTag = value;
                this.OnPropertyChanged("HashTag");
            }
        }

        public ObservableCollection<EditableSettingsRowViewModel> SettingsRow
        {
            get
            {
                return this._settingRows;
            }            
        }

        public RelayCommand AddCommand
        {
            get;
            private set;
        }

    }
}
