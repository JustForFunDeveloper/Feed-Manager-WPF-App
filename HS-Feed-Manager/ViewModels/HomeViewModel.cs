using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public ICollectionView LocalListView { get; private set; }
        public ICollectionView FeedListView { get; private set; }

        private ObservableCollection<string> _icons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitialiseListViews();
            SortModes = _feedSortModes;
            Mediator.Register("TabControlSelectionChanged", TabControlSelectionChanged);
        }

        #region Search and Filter
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

        private void InitialiseListViews()
        {
            LocalListView = CollectionViewSource.GetDefaultView(CurrenData.LocalList);
            LocalListView.CurrentChanged += LocalListViewCurrentChanged;

            FeedListView = CollectionViewSource.GetDefaultView(CurrenData.FeedList);
            FeedListView.CurrentChanged += FeedListViewCurrentChanged;
        }

        private void SortThisList(int value)
        {
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
                        LocalListView.Filter = null;
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

        private void LocalListViewCurrentChanged(object sender, EventArgs e)
        {
            LocalSeries localSeries = (LocalSeries)LocalListView.CurrentItem;

            if (localSeries == null)
                localSeries = new LocalSeries() { Rating = 0 };

            for (int i = 0; i < localSeries.Rating; i++)
            {
                Icons[i] = PackIconMaterialDesignKind.Star.ToString();
            }
            for (int i = 4; i >= localSeries.Rating; i--)
            {
                Icons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
        }

        private void FeedListViewCurrentChanged(object sender, EventArgs e)
        {
            Episode feedEpisode = (Episode)FeedListView.CurrentItem;

            for (int i = 4; i >= 0; i--)
            {
                Icons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
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
        public ObservableCollection<string> Icons
        {
            get => _icons;
            set
            {
                _icons = value;
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

        #region Download List
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