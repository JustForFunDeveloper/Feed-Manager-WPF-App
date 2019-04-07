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
            "Status Finished"
        };

        private ICommand _textBoxButtonCmd;
        private ICommand _moveToFirst;
        private ICommand _editInfo;

        private int _sortModesIndex;
        private string _filterString = "";

        public ICollectionView LocalListView { get; private set; }
        public ICollectionView SortedArtistsView { get; private set; }

        private ObservableCollection<string> _icons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitialiseArtistsView();
            InitialiseLocalListView();
        }

        #region Search and Filter
        public ICommand TextBoxButtonCmd
        {
            get
            {
                if (_textBoxButtonCmd == null)
                {
                    _textBoxButtonCmd = new RelayCommand<object>(
                        param => this.ItemClickCommand(param),
                        param => this.CanItemClickCommand(param)
                    );
                }
                return _textBoxButtonCmd;
            }
        }

        private bool CanItemClickCommand(object param)
        {
            return true;
        }

        private void ItemClickCommand(object param)
        {

            if (param.GetType() == typeof(string))
            {
                _filterString = param as string;
                SortedArtistsView.Refresh();
            }
            else if (param.GetType() == typeof(TextBox))
            {
                TextBox textBox = param as TextBox;
                if (textBox.Text.Length == 0)
                {
                    _filterString = "";
                    SortedArtistsView.Refresh();
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

        private void InitialiseArtistsView()
        {
            SortedArtistsView = CollectionViewSource.GetDefaultView(SampleData.Artists);
            //SortedArtistsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            SortedArtistsView.Filter = CustomFilter;
        }

        private void InitialiseLocalListView()
        {
            LocalListView = CollectionViewSource.GetDefaultView(CurrenData.LocalList);
            LocalListView.CurrentChanged += LocalListViewCurrentChanged;
        }

        private void SortThisList(int value)
        {
            using (SortedArtistsView.DeferRefresh())
            {
                SortedArtistsView.SortDescriptions.Clear();
                if (value == 0)
                    SortedArtistsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                if (value == 1)
                    SortedArtistsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            }
        }

        private bool CustomFilter(object item)
        {
            Artist artist = item as Artist;
            return artist.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

        #region LocalList
        private void LocalListViewCurrentChanged(object sender, EventArgs e)
        {
            LocalSeries localSeries = (LocalSeries)LocalListView.CurrentItem;
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
            Artist item = (Artist)SortedArtistsView.CurrentItem;
            SampleData.Artists.Remove(item);
            SampleData.Artists.Insert(0, item);
            SortedArtistsView.Refresh();
        }

        #endregion
    }
}