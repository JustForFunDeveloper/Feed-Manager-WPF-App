using System.Windows.Input;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.ViewModels.Common;
using HS_Feed_Manager.ViewModels.Handler;

namespace HS_Feed_Manager.ViewModels
{
    public class SettingsViewModel : PropertyChangedViewModel
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        private ICommand _defaultLocalPaths;
        private ICommand _saveLocalPaths;
        private string _fileEndings = "";
        private string _fileEndingsWaterMark;
        private string _localPath1 = "";
        private string _localPath1WaterMark;
        private string _localPath2 = "";
        private string _localPath2WaterMark;
        private string _localPath3 = "";
        private string _localPath3WaterMark;

        private ICommand _defaultFeedLink;
        private ICommand _saveFeedLink;
        private string _feedLink = "";
        private string _feedLinkWaterMark;
        private string _regexNameFront = "";
        private string _regexNameFrontWaterMark;
        private string _regexNameBack = "";
        private string _regexNameBackWaterMark;
        private string _regexNumberFront = "";
        private string _regexNumberFrontWaterMark;
        private string _regexNumberBack = "";
        private string _regexNumberBackWaterMark;

        private ICommand _logRefresh;
        private string _logText;


        public SettingsViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register(MediatorGlobal.RefreshSettingsView, RefreshView);
            RefreshView();
        }

        private void RefreshView(object e = null)
        {
            FileEndings = "";
            LocalPath1 = "";
            LocalPath2 = "";
            LocalPath3 = "";
            FeedLink = "";
            RegexNameFront = "";
            RegexNameBack = "";
            RegexNumberFront = "";
            RegexNumberBack = "";

            FileEndingsWaterMark = Logic.LocalConfig.FileEndings;
            LocalPath1WaterMark = Logic.LocalConfig.LocalPath1;
            LocalPath2WaterMark = Logic.LocalConfig.LocalPath2;
            LocalPath3WaterMark = Logic.LocalConfig.LocalPath3;
            FeedLinkWaterMark = Logic.LocalConfig.FeedUrl;
            RegexNameFrontWaterMark = Logic.LocalConfig.NameFrontRegex;
            RegexNameBackWaterMark = Logic.LocalConfig.NameBackRegex;
            RegexNumberFrontWaterMark = Logic.LocalConfig.NumberFrontRegex;
            RegexNumberBackWaterMark = Logic.LocalConfig.NumberBackRegex;

            LogText = Logic.Log;
        }

        #region Local Path Settings

        public ICommand DefaultLocalPaths
        {
            get
            {
                if (_defaultLocalPaths == null)
                    _defaultLocalPaths = new RelayCommand(
                        param => DefaultLocalPathsCommand(),
                        param => CanDefaultLocalPathsCommand()
                    );
                return _defaultLocalPaths;
            }
        }

        private bool CanDefaultLocalPathsCommand()
        {
            return true;
        }

        private void DefaultLocalPathsCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.RestoreLocalPathSettings, null);
        }

        public ICommand SaveLocalPaths
        {
            get
            {
                if (_saveLocalPaths == null)
                    _saveLocalPaths = new RelayCommand(
                        param => SaveLocalPathsCommand(),
                        param => CanSaveLocalPathsCommand()
                    );
                return _saveLocalPaths;
            }
        }

        private bool CanSaveLocalPathsCommand()
        {
            return true;
        }

        private void SaveLocalPathsCommand()
        {
            if (!FileEndings.Equals(""))
                Logic.LocalConfig.FileEndings = FileEndings;
            if (!LocalPath1.Equals(""))
                Logic.LocalConfig.LocalPath1 = LocalPath1;
            if (!LocalPath2.Equals(""))
                Logic.LocalConfig.LocalPath2 = LocalPath2;
            if (!LocalPath3.Equals(""))
                Logic.LocalConfig.LocalPath3 = LocalPath3;

            Mediator.NotifyColleagues(MediatorGlobal.SaveConfig, null);
        }

        public string FileEndings
        {
            get => _fileEndings;
            set
            {
                _fileEndings = value;
                OnPropertyChanged();
            }
        }

        public string FileEndingsWaterMark
        {
            get => _fileEndingsWaterMark;
            set
            {
                _fileEndingsWaterMark = value;
                OnPropertyChanged();
            }
        }

        public string LocalPath1
        {
            get => _localPath1;
            set
            {
                _localPath1 = value;
                OnPropertyChanged();
            }
        }

        public string LocalPath1WaterMark
        {
            get => _localPath1WaterMark;
            set
            {
                _localPath1WaterMark = value;
                OnPropertyChanged();
            }
        }

        public string LocalPath2
        {
            get => _localPath2;
            set
            {
                _localPath2 = value;
                OnPropertyChanged();
            }
        }

        public string LocalPath2WaterMark
        {
            get => _localPath2WaterMark;
            set
            {
                _localPath2WaterMark = value;
                OnPropertyChanged();
            }
        }

        public string LocalPath3
        {
            get => _localPath3;
            set
            {
                _localPath3 = value;
                OnPropertyChanged();
            }
        }

        public string LocalPath3WaterMark
        {
            get => _localPath3WaterMark;
            set
            {
                _localPath3WaterMark = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Feed Link and Regex Settings

        public ICommand DefaultFeedLink
        {
            get
            {
                if (_defaultFeedLink == null)
                    _defaultFeedLink = new RelayCommand(
                        param => DefaultFeedLinkCommand(),
                        param => CanDefaultFeedLinkCommand()
                    );
                return _defaultFeedLink;
            }
        }

        private bool CanDefaultFeedLinkCommand()
        {
            return true;
        }

        private void DefaultFeedLinkCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.RestoreFeedLinkSettings, null);
        }

        public ICommand SaveFeedLink
        {
            get
            {
                if (_saveFeedLink == null)
                    _saveFeedLink = new RelayCommand(
                        param => SaveFeedLinkCommand(),
                        param => CanSaveFeedLinkCommand()
                    );
                return _saveFeedLink;
            }
        }

        private bool CanSaveFeedLinkCommand()
        {
            return true;
        }

        private void SaveFeedLinkCommand()
        {
            if (!FeedLink.Equals(""))
                Logic.LocalConfig.FeedUrl = FeedLink;
            if (!RegexNameFront.Equals(""))
                Logic.LocalConfig.NameFrontRegex = RegexNameFront;
            if (!RegexNameBack.Equals(""))
                Logic.LocalConfig.NameBackRegex = RegexNameBack;
            if (!RegexNumberFront.Equals(""))
                Logic.LocalConfig.NumberFrontRegex = RegexNumberFront;
            if (!RegexNumberBack.Equals(""))
                Logic.LocalConfig.NumberBackRegex = RegexNumberBack;

            Mediator.NotifyColleagues(MediatorGlobal.SaveConfig, null);
        }

        public string FeedLink
        {
            get => _feedLink;
            set
            {
                _feedLink = value;
                OnPropertyChanged();
            }
        }

        public string FeedLinkWaterMark
        {
            get => _feedLinkWaterMark;
            set
            {
                _feedLinkWaterMark = value;
                OnPropertyChanged();
            }
        }

        public string RegexNameFront
        {
            get => _regexNameFront;
            set
            {
                _regexNameFront = value;
                OnPropertyChanged();
            }
        }

        public string RegexNameFrontWaterMark
        {
            get => _regexNameFrontWaterMark;
            set
            {
                _regexNameFrontWaterMark = value;
                OnPropertyChanged();
            }
        }

        public string RegexNameBack
        {
            get => _regexNameBack;
            set
            {
                _regexNameBack = value;
                OnPropertyChanged();
            }
        }

        public string RegexNameBackWaterMark
        {
            get => _regexNameBackWaterMark;
            set
            {
                _regexNameBackWaterMark = value;
                OnPropertyChanged();
            }
        }

        public string RegexNumberFront
        {
            get => _regexNumberFront;
            set
            {
                _regexNumberFront = value;
                OnPropertyChanged();
            }
        }

        public string RegexNumberFrontWaterMark
        {
            get => _regexNumberFrontWaterMark;
            set
            {
                _regexNumberFrontWaterMark = value;
                OnPropertyChanged();
            }
        }

        public string RegexNumberBack
        {
            get => _regexNumberBack;
            set
            {
                _regexNumberBack = value;
                OnPropertyChanged();
            }
        }

        public string RegexNumberBackWaterMark
        {
            get => _regexNumberBackWaterMark;
            set
            {
                _regexNumberBackWaterMark = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ICommand LogRefresh
        {
            get
            {
                if (_logRefresh == null)
                    _logRefresh = new RelayCommand(
                        param => LogRefreshCommand(),
                        param => CanLogRefreshCommand()
                    );
                return _logRefresh;
            }
        }

        private bool CanLogRefreshCommand()
        {
            return true;
        }

        private void LogRefreshCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.LogRefresh, null);
            RefreshView();
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
