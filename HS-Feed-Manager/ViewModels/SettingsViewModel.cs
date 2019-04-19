using System;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.ViewModels.Common;


namespace HS_Feed_Manager.ViewModels
{
    public class SettingsViewModel : PropertyChangedViewModel
    {
        // TODO: Add LocalPath 1-3, Regex from FileHandler
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        private string _logText;

        public SettingsViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            foreach (var localTvShow in Logic.LocalTvShows)
            {
                _logText += DateTime.Now.ToString() + ": ";
                _logText += localTvShow.Name + " | " + localTvShow.Status.ToString() + "\n";
            }
        }

        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                OnPropertyChanged();
            }
        }
    }
}
