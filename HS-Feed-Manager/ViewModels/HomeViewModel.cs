using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HS_Feed_Manager.ViewModels
{
    public class HomeViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private List<string> _sortModes = new List<string>
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

        private ICommand _textBoxButtonCmd;
        private ICommand _moveToFirst;
        private ICommand _editInfo;

        private int _sortModesIndex;
        private string _filterString = "";

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
            SortModesIndex = 1;
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
                    LocalListView.SortDescriptions.Clear();
                    FeedListView.SortDescriptions.Clear();
                    switch (value)
                    {
                        case 0:
                            LocalListView.Filter = null;
                            FeedListView.Filter = null;
                            break;
                        case 1:
                            LocalListView.Filter = null;
                            LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload", ListSortDirection.Ascending));
                            FeedListView.Filter = null;
                            FeedListView.SortDescriptions.Add(new SortDescription("LatestDownload", ListSortDirection.Ascending));
                            break;
                        case 2:
                            LocalListView.Filter = null;
                            LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload", ListSortDirection.Descending));
                            FeedListView.Filter = null;
                            FeedListView.SortDescriptions.Add(new SortDescription("LatestDownload", ListSortDirection.Descending));
                            break;
                        case 3:
                            LocalListView.Filter = AutowDownloadFilterOn;
                            FeedListView.Filter = AutowDownloadFilterOn;
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

        private bool AutowDownloadFilterOn(object item)
        {
            LocalSeries localSeries = item as LocalSeries;
            return localSeries.AutoDownloadStatus.Equals(AutoDownload.On);
        }

        private bool AutowDownloadFilterOff(object item)
        {
            LocalSeries localSeries = item as LocalSeries;
            return localSeries.AutoDownloadStatus.Equals(AutoDownload.Off);
        }

        private bool StatusFilterOngoing(object item)
        {
            LocalSeries localSeries = item as LocalSeries;
            return localSeries.Status.Equals(Status.Ongoing);
        }

        private bool StatusFilterNew(object item)
        {
            LocalSeries localSeries = item as LocalSeries;
            return localSeries.Status.Equals(Status.New);
        }

        private bool StatusFilterFinished(object item)
        {
            LocalSeries localSeries = item as LocalSeries;
            return localSeries.Status.Equals(Status.Finished);
        }

        private bool SearchFilter(object item)
        {
            LocalSeries localSeries = item as LocalSeries;
            return localSeries.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        #endregion

        #region LocalList
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
            LocalSeries localSeries = (LocalSeries)FeedListView.CurrentItem;

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