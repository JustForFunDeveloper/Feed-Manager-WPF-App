using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HS_Feed_Manager.ViewModels
{
    public class HomeViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private List<string> _feedSortModes = new List<string>
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

        private List<string> _localSortModes = new List<string>
        {
            "Not sorted",
            "Sort Date Newest",
            "Sort Date Oldest",
            "Auto Download On",
            "Auto Download Off",
            "Status Ongoing",
            "Status New",
            "Status Finished",
            "Sort By Name Asc.",
            "Sort By Name Desc."
        };

        private List<string> _sortModes;

        private int _lastFeedSortMode = 0;
        private int _lastLocalSortMode = 0;

        private ICommand _textBoxButtonCmd;
        private ICommand _moveToFirst;
        private ICommand _editInfo;

        private int _sortModesIndex;
        private string _filterString = "";
        private int _tabIndex;
        private string _clearSearch;

        Visibility _downloadListVisibility = Visibility.Visible;
        Visibility _episodeListVisibility = Visibility.Hidden;

        Visibility _feedInfoVisibility = Visibility.Visible;
        private string _feedInfoName;
        private string _feedInfoEpisode;
        private string _feedInfoAutoDownload;
        private string _feedInfoLocalEpisodeCount;
        private ObservableCollection<string> _feedIcons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        Visibility _localInfoVisibility = Visibility.Hidden;
        private string _localInfoName;
        private string _localInfoStatus;
        private string _localInfoEpisodes;
        private string _localInfoAutoDownload;
        private string _localInfoLocalEpisodeCount;
        private ObservableCollection<string> _localIcons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        Visibility _episodeInfoVisibility = Visibility.Hidden;
        private string _episodeInfoName;
        private string _episodeInfoStatus;
        private string _episodeInfoEpisode;
        private string _episodeInfoAutoDownload;
        private string _episodeInfoLocalEpisodeCount;
        private string _episodeInfoDownloadDate;
        private ObservableCollection<string> _episodeIcons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };


        public ICollectionView LocalListView { get; private set; }
        public ICollectionView FeedListView { get; private set; }
        private ObservableCollection<Episode> _episodeList;

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitialiseListViews();
            SortModes = _feedSortModes;
            Mediator.Register("TabControlSelectionChanged", TabControlSelectionChanged);
            Mediator.Register("ListBoxSelectionChanged", ListBoxSelectionChanged);
        }

        #region Search and Filter

        #region SearchBox and ComboBox
        public ICommand TextBoxButtonCmd
        {
            get
            {
                if (_textBoxButtonCmd == null)
                {
                    _textBoxButtonCmd = new RelayCommand<object>(
                        param => this.TextBoxButtonCommand(param),
                        param => this.CanTextBoxButtonCommand(param)
                    );
                }
                return _textBoxButtonCmd;
            }
        }

        private bool CanTextBoxButtonCommand(object param)
        {
            return true;
        }

        private void TextBoxButtonCommand(object param)
        {

            if (param.GetType() == typeof(string))
            {
                _filterString = param as string;
                LocalListView.Filter = SearchFilter;
                FeedListView.Filter = SearchFilter;
            }
            else if (param.GetType() == typeof(TextBox))
            {
                TextBox textBox = param as TextBox;
                if (textBox.Text.Length == 0)
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
                if (TabIndex == 0)
                    _lastFeedSortMode = value;
                else if (TabIndex == 1)
                    _lastLocalSortMode = value;
                SortThisList(value);
                OnPropertyChanged();
            }
        }
        #endregion

        #region Views, Filter and SOrting
        private void InitialiseListViews()
        {
            FeedListView = CollectionViewSource.GetDefaultView(CurrenData.FeedList);
            FeedListView.CurrentChanged += FeedListViewCurrentChanged;

            LocalListView = CollectionViewSource.GetDefaultView(CurrenData.LocalList);
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
                                FeedListView.Filter = StatusFilterOngoing;
                                break;
                            case 6:
                                FeedListView.Filter = StatusFilterNew;
                                break;
                            case 7:
                                FeedListView.Filter = StatusFilterFinished;
                                break;
                            case 8:
                                FeedListView.Filter = null;
                                FeedListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                                break;
                            case 9:
                                FeedListView.Filter = null;
                                FeedListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
                                break;
                            default:
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
                                LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload", ListSortDirection.Ascending));
                                break;
                            case 2:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload", ListSortDirection.Descending));
                                break;
                            case 3:
                                LocalListView.Filter = AutowDownloadFilterOn;
                                break;
                            case 4:
                                LocalListView.Filter = AutowDownloadFilterOff;
                                break;
                            case 5:
                                LocalListView.Filter = StatusFilterOngoing;
                                break;
                            case 6:
                                LocalListView.Filter = StatusFilterNew;
                                break;
                            case 7:
                                LocalListView.Filter = StatusFilterFinished;
                                break;
                            case 8:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                                break;
                            case 9:
                                LocalListView.Filter = null;
                                LocalListView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private bool AutowDownloadFilterOn(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                // TODO: Replace with proper DB method
                return CurrenData.LocalList.Any(x => (x.AutoDownloadStatus == AutoDownload.On) && (x.Name == ((Episode)item).Name));

            }
            else if (item.GetType() == typeof(LocalSeries))
            {
                LocalSeries localSeries = item as LocalSeries;
                return localSeries.AutoDownloadStatus.Equals(AutoDownload.On);
            }
            return false;
        }

        private bool AutowDownloadFilterOff(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                // TODO: Replace with proper DB method
                return CurrenData.LocalList.Any(x => (x.AutoDownloadStatus == AutoDownload.Off) && (x.Name == ((Episode)item).Name));
            }
            else if (item.GetType() == typeof(LocalSeries))
            {
                LocalSeries localSeries = item as LocalSeries;
                return localSeries.AutoDownloadStatus.Equals(AutoDownload.Off);
            }
            return false;
        }

        private bool ExistsLocally(object item)
        {
            return CurrenData.LocalList.Any(x => x.Name == ((Episode)item).Name);
        }

        private bool DoesntExistsLocally(object item)
        {
            return !CurrenData.LocalList.Any(x => x.Name == ((Episode)item).Name);
        }

        private bool StatusFilterOngoing(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                // TODO: Replace with proper DB method
                return CurrenData.LocalList.Any(x => (x.Status == Status.Ongoing) && (x.Name == ((Episode)item).Name));
            }
            else if (item.GetType() == typeof(LocalSeries))
            {
                LocalSeries localSeries = item as LocalSeries;
                return localSeries.Status.Equals(Status.Ongoing);
            }
            return false;
        }

        private bool StatusFilterNew(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                // TODO: Replace with proper DB method
                return CurrenData.LocalList.Any(x => (x.Status == Status.New) && (x.Name == ((Episode)item).Name));
            }
            else if (item.GetType() == typeof(LocalSeries))
            {
                LocalSeries localSeries = item as LocalSeries;
                return localSeries.Status.Equals(Status.New);
            }
            return false;
        }

        private bool StatusFilterFinished(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                // TODO: Replace with proper DB method
                return CurrenData.LocalList.Any(x => (x.Status == Status.Finished) && (x.Name == ((Episode)item).Name));
            }
            else if (item.GetType() == typeof(LocalSeries))
            {
                LocalSeries localSeries = item as LocalSeries;
                return localSeries.Status.Equals(Status.Finished);
            }
            return false;
        }

        private bool SearchFilter(object item)
        {
            if (item.GetType() == typeof(Episode))
            {
                Episode episode = item as Episode;
                return episode.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            else if (item.GetType() == typeof(LocalSeries))
            {
                LocalSeries localSeries = item as LocalSeries;
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

            Episode feedEpisode = (Episode)FeedListView.CurrentItem;

            if (feedEpisode == null)
                return;

            FeedInfoName = feedEpisode.Name;
            FeedInfoEpisode = feedEpisode.EpisodeNumber.ToString();

            LocalSeries localSeries = (LocalSeries)CurrenData.LocalList.Where(x => (x.Name == feedEpisode.Name)).SingleOrDefault();

            if (localSeries != null)
            {
                FeedInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                FeedInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();

                for (int i = 0; i < localSeries.Rating; i++)
                {
                    FeedIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                }
                for (int i = 4; i >= localSeries.Rating; i--)
                {
                    FeedIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
            else
            {
                FeedInfoAutoDownload = "None";
                FeedInfoLocalEpisodeCount = "None";

                for (int i = 4; i >= 0; i--)
                {
                    FeedIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
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

            LocalSeries localSeries = (LocalSeries)LocalListView.CurrentItem;

            if (localSeries != null)
            {
                EpisodeList = new ObservableCollection<Episode>(localSeries.EpisodeList);

                LocalInfoName = localSeries.Name;
                LocalInfoStatus = localSeries.Status.ToString();
                LocalInfoEpisodes = localSeries.Episodes.ToString();
                LocalInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                LocalInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();

                for (int i = 0; i < localSeries.Rating; i++)
                {
                    LocalIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                }
                for (int i = 4; i >= localSeries.Rating; i--)
                {
                    LocalIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
            else
            {
                for (int i = 4; i >= 0; i--)
                {
                    LocalIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
        }

        private void ListBoxSelectionChanged(object obj)
        {
            if (obj == null)
                return;

            if (TabIndex == 1)
            {
                FeedInfoVisibility = Visibility.Hidden;
                LocalInfoVisibility = Visibility.Hidden;
                EpisodeInfoVisibility = Visibility.Visible;
            }

            Episode listEpisode = (Episode)obj;

            if (listEpisode == null)
                return;

            EpisodeInfoName = listEpisode.Name;
            EpisodeInfoEpisode = listEpisode.EpisodeNumber.ToString();
            EpisodeInfoDownloadDate = listEpisode.DownloadDate.ToString("dd.MM.yyyy HH:mm:ss");

            LocalSeries localSeries = (LocalSeries)CurrenData.LocalList.Where(x => (x.Name == listEpisode.Name)).SingleOrDefault();

            if (localSeries != null)
            {
                EpisodeInfoStatus = localSeries.Status.ToString();
                EpisodeInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                EpisodeInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();

                for (int i = 0; i < localSeries.Rating; i++)
                {
                    EpisodeIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                }
                for (int i = 4; i >= localSeries.Rating; i--)
                {
                    EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
            else
            {
                for (int i = 4; i >= 0; i--)
                {
                    EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
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

        public ICommand EditInfo
        {
            get
            {
                if (_editInfo == null)
                {
                    _editInfo = new RelayCommand(
                        param => this.EditInfoCommand(),
                        param => this.CanEditInfoCommand()
                    );
                }
                return _editInfo;
            }
        }

        private bool CanEditInfoCommand()
        {
            return true;
        }

        private void EditInfoCommand()
        {
            LocalSeries localSeries = (LocalSeries)LocalListView.CurrentItem;
            Mediator.NotifyColleagues("UpdateFlyoutValues", localSeries);
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

        #region Left List

        public ObservableCollection<Episode> EpisodeList
        {
            get => _episodeList;
            set
            {
                _episodeList = value;
                OnPropertyChanged();
            }
        }

        public Visibility DownloadListVisibility
        {
            get => _downloadListVisibility;
            set
            {
                _downloadListVisibility = value;
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

        public ICommand MoveToFirst
        {
            get
            {
                if (_moveToFirst == null)
                {
                    _moveToFirst = new RelayCommand(
                        param => this.MoveToFirstCommand(),
                        param => this.CanMoveToFirstCommand()
                    );
                }
                return _moveToFirst;
            }
        }

        private bool CanMoveToFirstCommand()
        {
            return true;
        }

        private void MoveToFirstCommand()
        {
            //Artist item = (Artist)SortedArtistsView.CurrentItem;
            //SampleData.Artists.Remove(item);
            //SampleData.Artists.Insert(0, item);
            //SortedArtistsView.Refresh();
        }

        #endregion
    }
}