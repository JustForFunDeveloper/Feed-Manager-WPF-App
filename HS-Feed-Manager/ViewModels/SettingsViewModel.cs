using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.ViewModels.Handler;

namespace HS_Feed_Manager.ViewModels
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SettingsViewModel : INotifyPropertyChanged
    {
        // ReSharper disable once NotAccessedField.Local
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _defaultLocalPaths;
        private ICommand _saveLocalPaths;

        public string FileEndings { get; set; }
        public string LocalPath1 { get; set; }
        public string LocalPath2 { get; set; }
        public string LocalPath3 { get; set; }

        private ICommand _defaultFeedLink;
        private ICommand _saveFeedLink;

        public string FeedLink { get; set; }
        public string DownloadFolder { get; set; }

        public string RegexNameFront { get; set; }
        public string RegexNameBack { get; set; }
        public string RegexNumberFront { get; set; }
        public string RegexNumberBack { get; set; }

        public string RegexFileNameFront { get; set; }
        public string RegexFileNameBack { get; set; }
        public string RegexFileNumberFront { get; set; }
        public string RegexFileNumberBack { get; set; }

        public string RegexTorrentName { get; set; }

        private ICommand _logRefresh;

        public string LogText { get; set; }

        public SettingsViewModel()
        {
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
            DownloadFolder = "";

            RegexNameFront = "";
            RegexNameBack = "";
            RegexNumberFront = "";
            RegexNumberBack = "";

            RegexFileNameFront = "";
            RegexFileNameBack = "";
            RegexFileNumberFront = "";
            RegexFileNumberBack = "";

            RegexTorrentName = "";

            FileEndings = Logic.LocalConfig.FileEndings;
            LocalPath1 = Logic.LocalConfig.LocalPath1;
            LocalPath2 = Logic.LocalConfig.LocalPath2;
            LocalPath3 = Logic.LocalConfig.LocalPath3;

            FeedLink = Logic.LocalConfig.FeedUrl;
            DownloadFolder = Logic.LocalConfig.DownloadFolder;

            RegexNameFront = Logic.LocalConfig.NameFrontRegex;
            RegexNameBack = Logic.LocalConfig.NameBackRegex;
            RegexNumberFront = Logic.LocalConfig.NumberFrontRegex;
            RegexNumberBack = Logic.LocalConfig.NumberBackRegex;

            RegexFileNameFront = Logic.LocalConfig.FileNameFrontRegex;
            RegexFileNameBack = Logic.LocalConfig.FileNameBackRegex;
            RegexFileNumberFront = Logic.LocalConfig.FileNumberFrontRegex;
            RegexFileNumberBack = Logic.LocalConfig.FileNumberBackRegex;

            RegexTorrentName = Logic.LocalConfig.TorrentNameRegex;

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
    }
}
