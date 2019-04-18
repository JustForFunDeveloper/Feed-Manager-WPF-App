using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.ViewModels.Common;
using HS_Feed_Manager.ViewModels.Handler;
using JetBrains.Annotations;
using MahApps.Metro.IconPacks;

namespace HS_Feed_Manager.ViewModels
{
    public class HomeViewModel : PropertyChangedViewModel
    {
        // TODO: Logging and Exception-handling!
        // TODO: ToggleAutoDownload has to check for duplicates in download list before!
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        #region private Member

        private readonly List<string> _feedSortModes = new List<string>
        {
            "Not sorted",
            "Auto Download On",
            "Auto Download Off",
            "Exists locally",
            "Don't exists locally",
            "Status Ongoing",
            "Status New",
            "Status Finished",
            "Sort By Name Asc.",
            "Sort By Name Desc."
        };

        private readonly List<string> _localSortModes = new List<string>
        {
            "Not sorted",
            "Sort Date Newest",
            "Sort Date Oldest",
            "Auto Download On",
            "Auto Download Off",
            "Status Undefined",
            "Status Ongoing",
            "Status New",
            "Status Finished",
            "Sort By Name Asc.",
            "Sort By Name Desc."
        };

        private List<string> _episodeSortModes = new List<string>
        {
            "Not sorted",
            "By Number Asc.",
            "By Number Desc.",
            "By Rating Asc.",
            "By Rating Desc.",
            "By Date Asc.",
            "By Date Desc."
        };

        private List<string> _sortModes;

        private ICommand _downloadFeed;
        private ICommand _searchLocalFolder;

        private ICommand _textBoxButtonCmd;
        private ICommand _editLocalInfo;
        private ICommand _toggleAutoDownload;
        private ICommand _deleteFeedFromList;

        private ICommand _episodeTextBoxButtonCmd;
        private ICommand _playEpisode;
        private ICommand _openFolder;
        private ICommand _editEpisodeInfo;
        private ICommand _deleteEpisode;

        private ICommand _moveToFirst;
        private ICommand _moveToLast;
        private ICommand _moveUp;
        private ICommand _moveDown;

        private ICommand _addToList;
        private ICommand _deleteFromList;
        private ICommand _downloadThis;
        private ICommand _downloadAll;
        private ICommand _deleteTvShow;

        private int _sortModesIndex;
        private string _filterString = "";
        private int _tabIndex;
        private string _clearSearch;
        private int _episodeSortModesIndex;
        private string _episodeClearSearch;

        private ObservableCollection<Episode> _downloadList = new ObservableCollection<Episode>();
        private Visibility _downloadListVisibility = Visibility.Visible;
        private int _selectedDownloadIndex;

        private ObservableCollection<Episode> _episodeList;
        private Visibility _episodeListVisibility = Visibility.Hidden;
        private object _selectedEpisode;

        private bool _isProgressActive;
        private string _progressToolTip;
        private bool _isSearchLocalEnabled = true;

        private Visibility _feedInfoVisibility = Visibility.Visible;
        public ICollectionView FeedListView { get; private set; }

        private string _feedInfoName;
        private string _feedInfoEpisode;
        private string _feedInfoAutoDownload;
        private string _feedInfoLocalEpisodeCount;

        private ObservableCollection<string> _feedIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        private Visibility _localInfoVisibility = Visibility.Hidden;
        public ICollectionView LocalListView { get; private set; }

        private string _localInfoName;
        private string _localInfoStatus;
        private string _localInfoEpisodes;
        private string _localInfoAutoDownload;
        private string _localInfoLocalEpisodeCount;

        private ObservableCollection<string> _localIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        private Visibility _episodeInfoVisibility = Visibility.Hidden;
        private string _episodeInfoName;
        private string _episodeInfoStatus;
        private string _episodeInfoEpisode;
        private string _episodeInfoAutoDownload;
        private string _episodeInfoLocalEpisodeCount;
        private string _episodeInfoDownloadDate;

        private ObservableCollection<string> _episodeIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        #endregion

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitialiseListViews();

            SortModes = _feedSortModes;
            Mediator.Register(MediatorGlobal.TabControlSelectionChanged, TabControlSelectionChanged);
            Mediator.Register(MediatorGlobal.ListBoxSelectionChanged, ListBoxSelectionChanged);
            Mediator.Register(MediatorGlobal.OnRefreshListView, OnRefreshListViews);
            Mediator.Register(MediatorGlobal.FinishedSearchLocalFolder, OnFinishedSearchLocalFolder);
            Mediator.Register(MediatorGlobal.UpdateDownloadList, OnUpdateDownloadList);
        }

        #region IconBar

        public ICommand DownloadFeed
        {
            get
            {
                if (_downloadFeed == null)
                    _downloadFeed = new RelayCommand(
                        param => DownloadFeedCommand(),
                        param => CanDownloadFeedCommand()
                    );
                return _downloadFeed;
            }
        }

        private bool CanDownloadFeedCommand()
        {
            return true;
        }

        private void DownloadFeedCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.DownloadFeed, null);
        }

        public ICommand SearchLocalFolder
        {
            get
            {
                if (_searchLocalFolder == null)
                    _searchLocalFolder = new RelayCommand(
                        param => SearchLocalFolderCommand(),
                        param => CanSearchLocalFolderCommand()
                    );
                return _searchLocalFolder;
            }
        }

        private bool CanSearchLocalFolderCommand()
        {
            return true;
        }

        private void SearchLocalFolderCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.SearchLocalFolder, null);
            IsProgressActive = true;
            IsSearchLocalEnabled = false;
            ProgressToolTip = "Searching local folders!";
        }

        private void OnFinishedSearchLocalFolder(object obj)
        {
            IsProgressActive = false;
            IsSearchLocalEnabled = true;
            ProgressToolTip = "";
        }

        public bool IsProgressActive
        {
            get => _isProgressActive;
            set
            {
                _isProgressActive = value;
                OnPropertyChanged();
            }
        }

        public string ProgressToolTip
        {
            get => _progressToolTip;
            set
            {
                _progressToolTip = value;
                OnPropertyChanged();
            }
        }

        public bool IsSearchLocalEnabled
        {
            get => _isSearchLocalEnabled;
            set
            {
                _isSearchLocalEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Search and Filter

        #region SearchBox and ComboBox

        public ICommand TextBoxButtonCmd
        {
            get
            {
                if (_textBoxButtonCmd == null)
                    _textBoxButtonCmd = new RelayCommand<object>(
                        param => TextBoxButtonCommand(param),
                        param => CanTextBoxButtonCommand()
                    );
                return _textBoxButtonCmd;
            }
        }

        private bool CanTextBoxButtonCommand()
        {
            return true;
        }

        private void TextBoxButtonCommand(object param)
        {
            if (param == null)
                return;

            if (param is string)
            {
                _filterString = param as string;
                LocalListView.Filter = SearchFilter;
                FeedListView.Filter = SearchFilter;
            }
            else if (param.GetType() == typeof(TextBox))
            {
                var textBox = param as TextBox;
                if (textBox != null && textBox.Text.Length == 0)
                {
                    _filterString = "";
                    LocalListView.Filter = SearchFilter;
                    FeedListView.Filter = SearchFilter;
                }
            }
        }

        public List<string> SortModes
        {
            get => _sortModes;
            set
            {
                _sortModes = value;
                OnPropertyChanged();
            }
        }

        public int SortModesIndex
        {
            get => _sortModesIndex;
            set
            {
                _sortModesIndex = value;
                SortThisList(value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region Views, Filter and Sorting

        private void OnRefreshListViews(object obj)
        {
            FeedListView.Refresh();
            LocalListView.Refresh();
        }

        private void InitialiseListViews()
        {
            FeedListView = CollectionViewSource.GetDefaultView(Logic.FeedEpisodes);
            FeedListView.CurrentChanged += FeedListViewCurrentChanged;

            LocalListView = CollectionViewSource.GetDefaultView(Logic.LocalTvShows);
            LocalListView.CurrentChanged += LocalListViewCurrentChanged;
        }

        private void SortThisList(int value)
        {
            if (value == -1)
                return;
            using (LocalListView.DeferRefresh())
            {
                using (FeedListView.DeferRefresh())
                {
                    if (TabIndex == 0)
                    {
                        FeedListView.SortDescriptions.Clear();
                        switch (value)
                        {
                            case 0:
                                FeedListView.Filter = null;
                                break;
                            case 1:
                                FeedListView.Filter = AutowDownloadFilterOn;
                                break;
                            case 2:
                                FeedListView.Filter = AutowDownloadFilterOff;
                                break;
                            case 3:
                                FeedListView.Filter = ExistsLocally;
                                break;
                            case 4:
                                FeedListView.Filter = DoesntExistsLocally;
                                break;
                            case 5:
                                FeedListView.Filter = StatusFilterUndefined;
                                break;
                            case 6:
                                FeedListView.Filter = StatusFilterOngoing;
                                break;
                            case 7:
                                FeedListView.Filter = StatusFilterNew;
                                break;
                            case 8:
                                FeedListView.Filter = StatusFilterFinished;
                                break;
                            case 9:
                                FeedListView.Filter = null;
                                FeedListView.SortDescriptions.Add(new SortDescription("Name",
                                    ListSortDirection.Ascending));
                                break;
                            case 10:
                                FeedListView.Filter = null;
                                FeedListView.SortDescriptions.Add(new SortDescription("Name",
                                    ListSortDirection.Descending));
                                break;
                        }
                    }
                    else if (TabIndex == 1)
                    {
                        LocalListView.SortDescriptions.Clear();
                        switch (value)
                        {
                            case 0:
                                LocalListView.Filter = null;
                                break;
                            case 1:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload",
                                    ListSortDirection.Ascending));
                                break;
                            case 2:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload",
                                    ListSortDirection.Descending));
                                break;
                            case 3:
                                LocalListView.Filter = AutowDownloadFilterOn;
                                break;
                            case 4:
                                LocalListView.Filter = AutowDownloadFilterOff;
                                break;
                            case 5:
                                LocalListView.Filter = StatusFilterUndefined;
                                break;
                            case 6:
                                LocalListView.Filter = StatusFilterOngoing;
                                break;
                            case 7:
                                LocalListView.Filter = StatusFilterNew;
                                break;
                            case 8:
                                LocalListView.Filter = StatusFilterFinished;
                                break;
                            case 9:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("Name",
                                    ListSortDirection.Ascending));
                                break;
                            case 10:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("Name",
                                    ListSortDirection.Descending));
                                break;
                        }
                    }
                }
            }
        }

        private bool AutowDownloadFilterOn(object item)
        {
            if (item.GetType() == typeof(Episode))
                return Logic.LocalTvShows.Any(x =>
                    x.AutoDownloadStatus == AutoDownload.On && x.Name == ((Episode) item).Name);

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.AutoDownloadStatus.Equals(AutoDownload.On);
            }

            return false;
        }

        private bool AutowDownloadFilterOff(object item)
        {
            if (item.GetType() == typeof(Episode))
                return Logic.LocalTvShows.Any(x =>
                    x.AutoDownloadStatus == AutoDownload.Off && x.Name == ((Episode) item).Name);

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.AutoDownloadStatus.Equals(AutoDownload.Off);
            }

            return false;
        }

        private bool ExistsLocally(object item)
        {
            return Logic.LocalTvShows.Any(x => x.Name == ((Episode) item).Name);
        }

        private bool DoesntExistsLocally(object item)
        {
            return !Logic.LocalTvShows.Any(x => x.Name == ((Episode) item).Name);
        }

        private bool StatusFilterUndefined(object item)
        {
            if (item.GetType() == typeof(Episode))
                return Logic.LocalTvShows.Any(x => x.Status == Status.Undefined && x.Name == ((Episode) item).Name);

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.Status.Equals(Status.Undefined);
            }

            return false;
        }

        private bool StatusFilterOngoing(object item)
        {
            if (item.GetType() == typeof(Episode))
                return Logic.LocalTvShows.Any(x => x.Status == Status.Ongoing && x.Name == ((Episode) item).Name);

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.Status.Equals(Status.Ongoing);
            }

            return false;
        }

        private bool StatusFilterNew(object item)
        {
            if (item.GetType() == typeof(Episode))
                return Logic.LocalTvShows.Any(x => x.Status == Status.New && x.Name == ((Episode) item).Name);

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.Status.Equals(Status.New);
            }

            return false;
        }

        private bool StatusFilterFinished(object item)
        {
            if (item.GetType() == typeof(Episode))
                return Logic.LocalTvShows.Any(x => x.Status == Status.Finished && x.Name == ((Episode) item).Name);

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.Status.Equals(Status.Finished);
            }

            return false;
        }

        private bool SearchFilter(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                var episode = item as Episode;
                if (episode == null) throw new ArgumentNullException(nameof(episode));
                return episode.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            if (item.GetType() == typeof(TvShow))
            {
                var localSeries = item as TvShow;
                if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                return localSeries.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return false;
        }

        #endregion

        #region CurrentChangedEvents

        private void FeedListViewCurrentChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            if (TabIndex == 0)
            {
                FeedInfoVisibility = Visibility.Visible;
                LocalInfoVisibility = Visibility.Hidden;
                EpisodeInfoVisibility = Visibility.Hidden;
                DownloadListVisibility = Visibility.Visible;
                EpisodeListVisibility = Visibility.Hidden;
            }

            var feedEpisode = (Episode) FeedListView.CurrentItem;

            if (feedEpisode == null)
                return;

            FeedInfoName = feedEpisode.Name;
            FeedInfoEpisode = feedEpisode.EpisodeNumber.ToString();

            var localSeries = Logic.LocalTvShows.SingleOrDefault(x => x.Name == feedEpisode.Name);

            if (localSeries != null)
            {
                FeedInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                FeedInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();

                for (var i = 0; i < localSeries.Rating; i++) FeedIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                for (var i = 4; i >= localSeries.Rating; i--)
                    FeedIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
            else
            {
                FeedInfoAutoDownload = "None";
                FeedInfoLocalEpisodeCount = "None";

                for (var i = 4; i >= 0; i--) FeedIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
        }

        private void LocalListViewCurrentChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            if (TabIndex == 1)
            {
                FeedInfoVisibility = Visibility.Hidden;
                LocalInfoVisibility = Visibility.Visible;
                EpisodeInfoVisibility = Visibility.Hidden;
                DownloadListVisibility = Visibility.Hidden;
                EpisodeListVisibility = Visibility.Visible;
            }

            var localSeries = (TvShow) LocalListView.CurrentItem;

            if (localSeries != null)
            {
                EpisodeSortModesIndex = 2;
                EpisodeClearSearch = "";

                LocalInfoName = localSeries.Name;
                LocalInfoStatus = localSeries.Status.ToString();
                LocalInfoEpisodes = localSeries.EpisodeCount.ToString();
                LocalInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                LocalInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();

                for (var i = 0; i < localSeries.Rating; i++) LocalIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                for (var i = 4; i >= localSeries.Rating; i--)
                    LocalIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
            else
            {
                for (var i = 4; i >= 0; i--) LocalIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
        }

        private void ListBoxSelectionChanged(object obj)
        {
            if (TabIndex == 1)
            {
                FeedInfoVisibility = Visibility.Hidden;
                LocalInfoVisibility = Visibility.Hidden;
                EpisodeInfoVisibility = Visibility.Visible;
            }

            var listEpisode = (Episode) obj;

            if (listEpisode == null)
            {
                for (var i = 4; i >= 0; i--) EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                return;
            }


            EpisodeInfoName = listEpisode.Name;
            EpisodeInfoEpisode = listEpisode.EpisodeNumber.ToString();
            EpisodeInfoDownloadDate = listEpisode.DownloadDate.ToString("dd.MM.yyyy HH:mm:ss");

            for (var i = 0; i < listEpisode.Rating; i++) EpisodeIcons[i] = PackIconMaterialDesignKind.Star.ToString();
            for (var i = 4; i >= listEpisode.Rating; i--)
                EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();

            var localSeries = Logic.LocalTvShows.Where(x => x.Name == listEpisode.Name).SingleOrDefault();

            if (localSeries != null)
            {
                EpisodeInfoStatus = localSeries.Status.ToString();
                EpisodeInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                EpisodeInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();
            }
        }

        #endregion

        #endregion

        #region Tab

        public string ClearSearch
        {
            get => _clearSearch;
            set
            {
                _clearSearch = value;
                OnPropertyChanged();
            }
        }

        public int TabIndex
        {
            get => _tabIndex;
            set
            {
                _tabIndex = value;
                OnPropertyChanged();
            }
        }

        private void TabControlSelectionChanged(object obj)
        {
            if (TabIndex == 0)
            {
                SortModes = _feedSortModes;
                SortModesIndex = 0;
                ClearSearch = "";
            }
            else if (TabIndex == 1)
            {
                SortModes = _localSortModes;
                SortModesIndex = 1;
                ClearSearch = "";
            }
        }

        #endregion

        #region Feed Info

        public Visibility FeedInfoVisibility
        {
            get => _feedInfoVisibility;
            set
            {
                _feedInfoVisibility = value;
                OnPropertyChanged();
            }
        }

        public string FeedInfoName
        {
            get => _feedInfoName;
            set
            {
                _feedInfoName = value;
                OnPropertyChanged();
            }
        }

        public string FeedInfoEpisode
        {
            get => _feedInfoEpisode;
            set
            {
                _feedInfoEpisode = value;
                OnPropertyChanged();
            }
        }

        public string FeedInfoAutoDownload
        {
            get => _feedInfoAutoDownload;
            set
            {
                _feedInfoAutoDownload = value;
                OnPropertyChanged();
            }
        }

        public string FeedInfoLocalEpisodeCount
        {
            get => _feedInfoLocalEpisodeCount;
            set
            {
                _feedInfoLocalEpisodeCount = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> FeedIcons
        {
            get => _feedIcons;
            set
            {
                _feedIcons = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Local Info

        public Visibility LocalInfoVisibility
        {
            get => _localInfoVisibility;
            set
            {
                _localInfoVisibility = value;
                OnPropertyChanged();
            }
        }

        public string LocalInfoName
        {
            get => _localInfoName;
            set
            {
                _localInfoName = value;
                OnPropertyChanged();
            }
        }

        public string LocalInfoStatus
        {
            get => _localInfoStatus;
            set
            {
                _localInfoStatus = value;
                OnPropertyChanged();
            }
        }

        public string LocalInfoEpisodes
        {
            get => _localInfoEpisodes;
            set
            {
                _localInfoEpisodes = value;
                OnPropertyChanged();
            }
        }

        public string LocalInfoAutoDownload
        {
            get => _localInfoAutoDownload;
            set
            {
                _localInfoAutoDownload = value;
                OnPropertyChanged();
            }
        }

        public string LocalInfoLocalEpisodeCount
        {
            get => _localInfoLocalEpisodeCount;
            set
            {
                _localInfoLocalEpisodeCount = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> LocalIcons
        {
            get => _localIcons;
            set
            {
                _localIcons = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Episode Info

        public Visibility EpisodeInfoVisibility
        {
            get => _episodeInfoVisibility;
            set
            {
                _episodeInfoVisibility = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeInfoName
        {
            get => _episodeInfoName;
            set
            {
                _episodeInfoName = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeInfoStatus
        {
            get => _episodeInfoStatus;
            set
            {
                _episodeInfoStatus = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeInfoEpisode
        {
            get => _episodeInfoEpisode;
            set
            {
                _episodeInfoEpisode = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeInfoAutoDownload
        {
            get => _episodeInfoAutoDownload;
            set
            {
                _episodeInfoAutoDownload = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeInfoLocalEpisodeCount
        {
            get => _episodeInfoLocalEpisodeCount;
            set
            {
                _episodeInfoLocalEpisodeCount = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeInfoDownloadDate
        {
            get => _episodeInfoDownloadDate;
            set
            {
                _episodeInfoDownloadDate = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> EpisodeIcons
        {
            get => _episodeIcons;
            set
            {
                _episodeIcons = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Feed List

        public ICommand ToggleAutoDownload
        {
            get
            {
                if (_toggleAutoDownload == null)
                    _toggleAutoDownload = new RelayCommand<object>(
                        param => ToggleAutoDownloadCommand(param),
                        param => CanToggleAutoDownloadCommand()
                    );
                return _toggleAutoDownload;
            }
        }

        private bool CanToggleAutoDownloadCommand()
        {
            return true;
        }

        private void ToggleAutoDownloadCommand(object param)
        {
            TvShow currentTvShow = null;
            string listName = param as string;
            if (listName != null && listName.Equals("LocalList"))
            {
                currentTvShow = (TvShow) LocalListView.CurrentItem;
                currentTvShow.AutoDownloadStatus = currentTvShow.AutoDownloadStatus.Equals(AutoDownload.On)
                    ? AutoDownload.Off
                    : AutoDownload.On;
                Mediator.NotifyColleagues(MediatorGlobal.ToggleAutoDownload, currentTvShow);
            }
            else if (listName != null && listName.Equals("FeedList"))
            {
                Episode currentEpisode = (Episode) FeedListView.CurrentItem;
                currentTvShow = Logic.LocalTvShows.SingleOrDefault(tvShow => tvShow.Name.Equals(currentEpisode.Name));
                if (currentTvShow == null)
                {
                    currentTvShow = new TvShow()
                    {
                        AutoDownloadStatus = AutoDownload.On,
                        Name = currentEpisode.Name
                    };
                    DownloadList.Add(currentEpisode);
                }
                else
                {
                    currentTvShow.AutoDownloadStatus = currentTvShow.AutoDownloadStatus.Equals(AutoDownload.On)
                        ? AutoDownload.Off
                        : AutoDownload.On;
                    if (currentTvShow.AutoDownloadStatus.Equals(AutoDownload.On))
                        DownloadList.Add(currentEpisode);
                }
            }

            if (currentTvShow != null)
                Mediator.NotifyColleagues(MediatorGlobal.ToggleAutoDownload, currentTvShow);

            OnRefreshListViews(null);
        }

        public ICommand DeleteFeedFromList
        {
            get
            {
                if (_deleteFeedFromList == null)
                    _deleteFeedFromList = new RelayCommand(
                        param => DeleteFeedFromListCommand(),
                        param => CanDeleteFeedFromListCommand()
                    );
                return _deleteFeedFromList;
            }
        }

        private bool CanDeleteFeedFromListCommand()
        {
            return true;
        }

        private void DeleteFeedFromListCommand()
        {
            if (FeedListView.CurrentItem == null)
                return;

            DownloadList.Remove((Episode) FeedListView.CurrentItem);
        }

        #endregion

        #region Local List

        public ICommand EditLocalInfo
        {
            get
            {
                if (_editLocalInfo == null)
                    _editLocalInfo = new RelayCommand(
                        param => EditLocalInfoCommand(),
                        param => CanEditLocalInfoCommand()
                    );
                return _editLocalInfo;
            }
        }

        private bool CanEditLocalInfoCommand()
        {
            return true;
        }

        private void EditLocalInfoCommand()
        {
            if (LocalListView.CurrentItem == null)
                return;

            var localSeries = (TvShow) LocalListView.CurrentItem;
            Mediator.NotifyColleagues(MediatorGlobal.UpdateFlyoutValues, localSeries);
        }

        public ICommand DeleteTvShow
        {
            get
            {
                if (_deleteTvShow == null)
                    _deleteTvShow = new RelayCommand(
                        param => DeleteTvShowCommand(),
                        param => CanDeleteTvShowCommand()
                    );
                return _deleteTvShow;
            }
        }

        private bool CanDeleteTvShowCommand()
        {
            return true;
        }

        private void DeleteTvShowCommand()
        {
            if (LocalListView.CurrentItem == null)
                return;

            var localTvShow = (TvShow) LocalListView.CurrentItem;
            Mediator.NotifyColleagues(MediatorGlobal.DeleteTvShow, localTvShow);
        }

        #endregion

        #region Download List

        public Visibility DownloadListVisibility
        {
            get => _downloadListVisibility;
            set
            {
                _downloadListVisibility = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Episode> DownloadList
        {
            get => _downloadList;
            set
            {
                _downloadList = value;
                OnPropertyChanged();
            }
        }

        public int SelectedDownloadIndex
        {
            get => _selectedDownloadIndex;
            set
            {
                _selectedDownloadIndex = value;
                OnPropertyChanged();
            }
        }

        public ICommand MoveToFirst
        {
            get
            {
                if (_moveToFirst == null)
                    _moveToFirst = new RelayCommand(
                        param => MoveToFirstCommand(),
                        param => CanMoveToFirstCommand()
                    );
                return _moveToFirst;
            }
        }

        private bool CanMoveToFirstCommand()
        {
            return true;
        }

        private void MoveToFirstCommand()
        {
            if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                return;

            var episode = DownloadList[SelectedDownloadIndex];
            DownloadList.RemoveAt(SelectedDownloadIndex);
            DownloadList.Insert(0, episode);
            SelectedDownloadIndex = 0;
        }

        public ICommand MoveToLast
        {
            get
            {
                if (_moveToLast == null)
                    _moveToLast = new RelayCommand(
                        param => MoveToLastCommand(),
                        param => CanMoveToLastCommand()
                    );
                return _moveToLast;
            }
        }

        private bool CanMoveToLastCommand()
        {
            return true;
        }

        private void MoveToLastCommand()
        {
            if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                return;

            var episode = DownloadList[SelectedDownloadIndex];
            DownloadList.RemoveAt(SelectedDownloadIndex);
            DownloadList.Add(episode);
            SelectedDownloadIndex = DownloadList.Count - 1;
        }

        public ICommand MoveUp
        {
            get
            {
                if (_moveUp == null)
                    _moveUp = new RelayCommand(
                        param => MoveUpCommand(),
                        param => CanMoveUpCommand()
                    );
                return _moveUp;
            }
        }

        private bool CanMoveUpCommand()
        {
            return true;
        }

        private void MoveUpCommand()
        {
            if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                return;

            if (SelectedDownloadIndex - 1 >= 0)
            {
                var episode = DownloadList[SelectedDownloadIndex];
                if (SelectedDownloadIndex == DownloadList.Count - 1)
                {
                    DownloadList.RemoveAt(SelectedDownloadIndex);
                    DownloadList.Insert(SelectedDownloadIndex, episode);
                    SelectedDownloadIndex = SelectedDownloadIndex - 1;
                }
                else
                {
                    DownloadList.RemoveAt(SelectedDownloadIndex);
                    DownloadList.Insert(SelectedDownloadIndex - 1, episode);
                    SelectedDownloadIndex = SelectedDownloadIndex - 2;
                }
            }
        }

        public ICommand MoveDown
        {
            get
            {
                if (_moveDown == null)
                    _moveDown = new RelayCommand(
                        param => MoveDownCommand(),
                        param => CanMoveDownCommand()
                    );
                return _moveDown;
            }
        }

        private bool CanMoveDownCommand()
        {
            return true;
        }

        private void MoveDownCommand()
        {
            if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                return;

            if (SelectedDownloadIndex + 1 < DownloadList.Count)
            {
                var episode = DownloadList[SelectedDownloadIndex];
                DownloadList.RemoveAt(SelectedDownloadIndex);
                DownloadList.Insert(SelectedDownloadIndex + 1, episode);
                SelectedDownloadIndex = SelectedDownloadIndex + 1;
            }
        }

        public ICommand AddToList
        {
            get
            {
                if (_addToList == null)
                    _addToList = new RelayCommand(
                        param => AddToListCommand(),
                        param => CanAddToListCommand()
                    );
                return _addToList;
            }
        }

        private bool CanAddToListCommand()
        {
            return true;
        }

        private void AddToListCommand()
        {
            if (FeedListView.CurrentItem == null)
                return;

            if (!DownloadList.Any(x => x.Name == ((Episode) FeedListView.CurrentItem).Name))
            {
                var episode = (Episode) FeedListView.CurrentItem;
                if (episode != null)
                    DownloadList.Add(episode);
            }
        }

        public ICommand DeleteFromList
        {
            get
            {
                if (_deleteFromList == null)
                    _deleteFromList = new RelayCommand(
                        param => DeleteFromListCommand(),
                        param => CanDeleteFromListCommand()
                    );
                return _deleteFromList;
            }
        }

        private bool CanDeleteFromListCommand()
        {
            return true;
        }

        private void DeleteFromListCommand()
        {
            if (SelectedDownloadIndex >= 0)
                DownloadList.RemoveAt(SelectedDownloadIndex);
        }

        public ICommand DownloadThis
        {
            get
            {
                if (_downloadThis == null)
                    _downloadThis = new RelayCommand(
                        param => DownloadThisCommand(),
                        param => CanDownloadThisCommand()
                    );
                return _downloadThis;
            }
        }

        private bool CanDownloadThisCommand()
        {
            return true;
        }

        private void DownloadThisCommand()
        {
            if (SelectedDownloadIndex < 0)
                return;

            Mediator.NotifyColleagues(MediatorGlobal.StartDownloadEpisodes,
                new List<object> {DownloadList[SelectedDownloadIndex]});
            DownloadList.Remove(DownloadList[SelectedDownloadIndex]);
        }

        public ICommand DownloadAll
        {
            get
            {
                if (_downloadAll == null)
                    _downloadAll = new RelayCommand(
                        param => DownloadAllCommand(),
                        param => CanDownloadAllCommand()
                    );
                return _downloadAll;
            }
        }

        private bool CanDownloadAllCommand()
        {
            return true;
        }

        private void DownloadAllCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.StartDownloadEpisodes,
                new List<object>(DownloadList.ToList()));
            DownloadList.Clear();
        }

        private void OnUpdateDownloadList(object obj)
        {
            List<object> autoEpisodes = obj as List<object>;
            DownloadList.Clear();
            foreach (var autoEpisode in autoEpisodes)
            {
                DownloadList.Add((Episode)autoEpisode);
            }
        }

        #endregion

        #region Episode List

        public ObservableCollection<Episode> EpisodeList
        {
            get => _episodeList;
            set
            {
                _episodeList = value;
                OnPropertyChanged();
            }
        }

        public object SelectedEpisode
        {
            get => _selectedEpisode;
            set
            {
                _selectedEpisode = value;
                OnPropertyChanged();
            }
        }

        public Visibility EpisodeListVisibility
        {
            get => _episodeListVisibility;
            set
            {
                _episodeListVisibility = value;
                OnPropertyChanged();
            }
        }

        public string EpisodeClearSearch
        {
            get => _episodeClearSearch;
            set
            {
                _episodeClearSearch = value;
                OnPropertyChanged();
            }
        }

        public ICommand EpisodeTextBoxButtonCmd
        {
            get
            {
                if (_episodeTextBoxButtonCmd == null)
                    _episodeTextBoxButtonCmd = new RelayCommand<object>(
                        param => EpisodeTextBoxButtonCommand(param),
                        param => CanEpisodeTextBoxButtonCommand()
                    );
                return _episodeTextBoxButtonCmd;
            }
        }

        private bool CanEpisodeTextBoxButtonCommand()
        {
            return true;
        }

        private void EpisodeTextBoxButtonCommand(object param)
        {
            if (param == null)
                return;

            var value = param as string;
            var localSeries = (TvShow) LocalListView.CurrentItem;
            var episodes = new List<Episode>(localSeries.Episodes);

            if (value != null && value.Equals(""))
            {
                EpisodeList = new ObservableCollection<Episode>(episodes);
                return;
            }

            if (param is string)
            {
                var episodeFiltered = episodes.Where(item => item.EpisodeNumber.ToString() == value).ToList();
                EpisodeList = new ObservableCollection<Episode>(episodeFiltered);
            }
        }

        public ICommand PlayEpisode
        {
            get
            {
                if (_playEpisode == null)
                    _playEpisode = new RelayCommand<object>(
                        param => PlayEpisodeCommand(param),
                        param => CanPlayEpisodenCommand()
                    );
                return _playEpisode;
            }
        }

        private bool CanPlayEpisodenCommand()
        {
            return true;
        }

        private void PlayEpisodeCommand([NotNull] object param)
        {
            Mediator.NotifyColleagues(MediatorGlobal.PlayEpisode, SelectedEpisode);
        }

        public ICommand OpenFolder
        {
            get
            {
                if (_openFolder == null)
                    _openFolder = new RelayCommand<object>(
                        param => OpenFolderCommand(),
                        param => CanOpenFolderCommand()
                    );
                return _openFolder;
            }
        }

        private bool CanOpenFolderCommand()
        {
            return true;
        }

        private void OpenFolderCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.OpenFolder, SelectedEpisode);
        }

        public ICommand EditEpisodeInfo
        {
            get
            {
                if (_editEpisodeInfo == null)
                    _editEpisodeInfo = new RelayCommand(
                        param => EditEpisodeInfoCommand(),
                        param => CanEditEpisodeInfoCommand()
                    );
                return _editEpisodeInfo;
            }
        }

        private bool CanEditEpisodeInfoCommand()
        {
            return true;
        }

        private void EditEpisodeInfoCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.UpdateEpisodeFlyoutValues, SelectedEpisode);
        }

        public ICommand DeleteEpisode
        {
            get
            {
                if (_deleteEpisode == null)
                    _deleteEpisode = new RelayCommand<object>(
                        param => DeleteEpisodeCommand(param),
                        param => CanDeleteEpisodeCommand()
                    );
                return _deleteEpisode;
            }
        }

        private bool CanDeleteEpisodeCommand()
        {
            return true;
        }

        private void DeleteEpisodeCommand(object param)
        {
            Mediator.NotifyColleagues(MediatorGlobal.DeleteEpisode, SelectedEpisode);
        }

        public List<string> EpisodeSortModes
        {
            get => _episodeSortModes;
            set
            {
                _episodeSortModes = value;
                OnPropertyChanged();
            }
        }

        public int EpisodeSortModesIndex
        {
            get => _episodeSortModesIndex;
            set
            {
                _episodeSortModesIndex = value;
                SortEpisodeList(value);
                OnPropertyChanged();
            }
        }

        private void SortEpisodeList(int value)
        {
            var localSeries = (TvShow) LocalListView.CurrentItem;
            var episodes = new List<Episode>(localSeries.Episodes);
            switch (value)
            {
                case 0:
                    EpisodeList = new ObservableCollection<Episode>(localSeries.Episodes);
                    break;
                case 1:
                    episodes.Sort((ep1, ep2) => ep1.EpisodeNumber.CompareTo(ep2.EpisodeNumber));
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    break;
                case 2:
                    episodes.Sort((ep1, ep2) => ep2.EpisodeNumber.CompareTo(ep1.EpisodeNumber));
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    break;
                case 3:
                    episodes.Sort((ep1, ep2) => ep1.Rating.CompareTo(ep2.Rating));
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    break;
                case 4:
                    episodes.Sort((ep1, ep2) => ep2.Rating.CompareTo(ep1.Rating));
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    break;
                case 5:
                    episodes.Sort((ep1, ep2) => ep1.DownloadDate.CompareTo(ep2.DownloadDate));
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    break;
                case 6:
                    episodes.Sort((ep1, ep2) => ep2.DownloadDate.CompareTo(ep1.DownloadDate));
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    break;
            }
        }

        #endregion
    }
}