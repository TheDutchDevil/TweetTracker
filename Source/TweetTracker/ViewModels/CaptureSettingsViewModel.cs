using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetTracker.ViewModels
{
    class CaptureSettingsViewModel : BaseViewModel
    {
        private string _captureHashTag;

        private string _culture;

        private ObservableCollection<EditableSettingsRowViewModel> _settingRows;

        public CaptureSettingsViewModel()
        {
            this._settingRows = new ObservableCollection<EditableSettingsRowViewModel>();

            this._culture = string.Empty;

            this.AddCommand = new RelayCommand(() => this.SettingsRow.Add(new EditableSettingsRowViewModel(this.SettingsRow)));

            this.SaveCommand = new RelayCommand(this.Save);

            this.LoadCommand = new RelayCommand(this.Load);
        }

        public string Culture
        {
            get
            {
                return this._culture;
            }

            set
            {
                this._culture = value;
                this.OnPropertyChanged("Culture");

            }
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
            private set
            {
                this._settingRows = value;
                this.OnPropertyChanged("SettingsRow");
            }
        }

        public RelayCommand AddCommand
        {
            get;
            private set;
        }

        public RelayCommand SaveCommand
        {
            get;
            private set;
        }

        public RelayCommand LoadCommand
        {
            get;
            private set;
        }

        private void Save()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Settings"; // Default file name
            dlg.DefaultExt = ".set"; // Default file extension
            dlg.Filter = "Settings (.set)|*.set"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                var settings = TypeSerializer.SerializeToString(this);

                File.WriteAllLines(filename, settings.Split('\n'));
            }
        }

        private void Load()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.Filter = "Settings (*.set)|*.set";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                var contents = String.Join(string.Empty, File.ReadAllLines(filename));

                CaptureSettingsViewModel model = TypeSerializer.DeserializeFromString<CaptureSettingsViewModel>(contents);

                this.HashTag = model.HashTag;
                this.Culture = model.Culture;
                this.SettingsRow = model.SettingsRow;

                foreach(var row in this.SettingsRow)
                {
                    row.RepairCommand(this.SettingsRow);
                }
            }
        }

    }
}
