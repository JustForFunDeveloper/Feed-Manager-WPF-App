using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.ViewModels.Common;

namespace HS_Feed_Manager.ViewModels
{
    public class MainViewModel : PropertyChangedViewModel
    {
        private static MainViewModel _mainViewModel;
        private TvShow _currentTvShow;
        private Episode _currentEpisode;

        public static MainViewModel getInstance { get => _mainViewModel; }

        #region private Member
        private ICommand _openMenu;
        private ICommand _itemClick;
        private ICommand _optionItemClick;
        private ICommand _okEdit;
        private ICommand _cancelEdit;
        private ICommand _episodeOkEdit;
        private ICommand _episodeCancelEdit;

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        private bool _isPaneOpened;
        private bool _isFlyoutOpen;
        private bool _isEpisodeFlyoutOpen;
        private int _selectedIndex;

        private int _selectedStatus;
        private string _selectedValueStatus;
        private string _episodes;
        private string _episodesValue;
        private int _selectedAutoDownload;
        private string _selectedValueAutoDownload;
        private double _ratingValue;
        private double _episodeRatingValue;

        private ObservableCollection<string> _icons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        private ObservableCollection<string> _episodeIcons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        #endregion

        public MainViewModel()
        {
            CreateMenuItems();
            _mainViewModel = this;
            Mediator.Register(MediatorGlobal.UpdateFlyoutValues, UpdateFlyoutValues);
            Mediator.Register(MediatorGlobal.UpdateEpisodeFlyoutValues, UpdateEpisodeFlyoutValues);
            Mediator.Register(MediatorGlobal.SliderRateValueChanged, SliderRateValueChanged);
            Mediator.Register(MediatorGlobal.EpisodeSliderRateValueChanged, EpisodeSliderRateValueChanged);
        }

        #region Hamburger Menu

        private void CreateMenuItems()
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Home},
                    Label = "Home",
                    ToolTip = "The Home view.",
                    Tag = new HomeViewModel(this)
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Settings},
                    Label = "Settings",
                    ToolTip = "The Application settings.",
                    Tag = new SettingsViewModel(this)
                }
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Help},
                    Label = "About",
                    ToolTip = "About this Software.",
                    Tag = new AboutViewModel(this)
                }
            };
        }

        public HamburgerMenuItemCollection MenuItems
        {
            get => _menuItems;
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                OnPropertyChanged();
            }
        }

        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems)) return;
                _menuOptionItems = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public bool IsPaneOpened
        {
            get { return _isPaneOpened; }
            set
            {
                _isPaneOpened = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenMenu
        {
            get
            {
                if (_openMenu == null)
                {
                    _openMenu = new RelayCommand(
                        param => SaveOpenMenuCommand(),
                        param => CanSaveOpenMenuCommand()
                    );
                }
                return _openMenu;
            }
        }

        private bool CanSaveOpenMenuCommand()
        {
            return true;
        }

        private void SaveOpenMenuCommand()
        {
            IsPaneOpened = !IsPaneOpened;
        }

        public ICommand ItemClick
        {
            get
            {
                if (_itemClick == null)
                {
                    _itemClick = new RelayCommand<int>(
                        param => ItemClickCommand(),
                        param => CanItemClickCommand()
                    );
                }
                return _itemClick;
            }
        }

        private bool CanItemClickCommand()
        {
            return true;
        }

        private void ItemClickCommand()
        {
            IsPaneOpened = false;
        }

        public ICommand OptionItemClick
        {
            get
            {
                if (_optionItemClick == null)
                {
                    _optionItemClick = new RelayCommand<int>(
                        param => OptionItemClickCommand(),
                        param => CanOptionItemClickCommand()
                    );
                }
                return _optionItemClick;
            }
        }

        private bool CanOptionItemClickCommand()
        {
            return true;
        }

        private void OptionItemClickCommand()
        {
            IsPaneOpened = false;
        }
        #endregion

        #region Flyout EditLocalInfo
        private void UpdateFlyoutValues(object value)
        {
            if (value == null)
                return;

            TvShow localSeries = value as TvShow;
            if (localSeries != null)
            {
                _currentTvShow = localSeries;
                int statusIndex = Array.IndexOf(Enum.GetValues(localSeries.Status.GetType()), localSeries.Status);
                SelectedStatus = statusIndex;
                Episodes = localSeries.EpisodeCount.ToString();
                int autoDownloadIndex = Array.IndexOf(Enum.GetValues(localSeries.AutoDownloadStatus.GetType()),
                    localSeries.AutoDownloadStatus);
                SelectedAutoDownload = autoDownloadIndex;
                RatingValue = localSeries.Rating;
            }

            IsFlyoutOpen = true;
        }

        public bool IsFlyoutOpen
        {
            get { return _isFlyoutOpen; }
            set
            {
                _isFlyoutOpen = value;
                OnPropertyChanged();
            }
        }

        public int SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }

        public string SelectedValueStatus
        {
            get { return _selectedValueStatus; }
            set
            {
                _selectedValueStatus = value;
                OnPropertyChanged();
            }
        }


        public IEnumerable<Status> StatusValues
        {
            get
            {
                return Enum.GetValues(typeof(Status)).Cast<Status>();
            }
        }

        public string Episodes
        {
            get { return _episodes; }
            set
            {
                _episodes = value;
                OnPropertyChanged();
            }
        }

        public string EpisodesValue
        {
            get { return _episodesValue; }
            set
            {
                _episodesValue = value;
                OnPropertyChanged();
            }
        }

        public int SelectedAutoDownload
        {
            get { return _selectedAutoDownload; }
            set
            {
                _selectedAutoDownload = value;
                OnPropertyChanged();
            }
        }

        public string SelectedValueAutoDownload
        {
            get { return _selectedValueAutoDownload; }
            set
            {
                _selectedValueAutoDownload = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<AutoDownload> AutoDownloadValues
        {
            get
            {
                return Enum.GetValues(typeof(AutoDownload)).Cast<AutoDownload>();
            }
        }

        public ObservableCollection<string> Icons
        {
            get => _icons;
            set
            {
                _icons = value;
                OnPropertyChanged();
            }
        }

        public double RatingValue
        {
            get { return _ratingValue; }
            set
            {
                _ratingValue = value;
                OnPropertyChanged();
            }
        }

        private void SliderRateValueChanged(object obj)
        {
            int length = (int)obj;
            for (int i = 0; i < length; i++)
            {
                Icons[i] = PackIconMaterialDesignKind.Star.ToString();
            }
            for (int i = 4; i >= length; i--)
            {
                Icons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
        }

        public ICommand OkEdit
        {
            get
            {
                if (_okEdit == null)
                {
                    _okEdit = new RelayCommand(
                        param => SaveOkEditCommand(),
                        param => CanSaveOkEditCommand()
                    );
                }
                return _okEdit;
            }
        }

        private bool CanSaveOkEditCommand()
        {
            return true;
        }

        private void SaveOkEditCommand()
        {
            TvShow localTvShow = _currentTvShow;
            localTvShow.Status = (Status) Enum.Parse(typeof(Status), SelectedValueStatus);
            if (int.TryParse(EpisodesValue, out int result))
                localTvShow.EpisodeCount = result;
            localTvShow.AutoDownloadStatus = (AutoDownload)Enum.Parse(typeof(AutoDownload), SelectedValueAutoDownload);
            localTvShow.Rating = (int)RatingValue;
            Mediator.NotifyColleagues(MediatorGlobal.SaveEditInfo, localTvShow);

            IsFlyoutOpen = false;
            Mediator.NotifyColleagues(MediatorGlobal.OnRefreshListView, null);
        }

        public ICommand CancelEdit
        {
            get
            {
                if (_cancelEdit == null)
                {
                    _cancelEdit = new RelayCommand(
                        param => SaveCancelEditCommand(),
                        param => CanSaveCancelEditCommand()
                    );
                }
                return _cancelEdit;
            }
        }

        private bool CanSaveCancelEditCommand()
        {
            return true;
        }

        private void SaveCancelEditCommand()
        {
            EpisodesValue = "";
            IsFlyoutOpen = false;
        }

        #endregion

        #region Flyout EditEpisodeInfo
        private void UpdateEpisodeFlyoutValues(object value)
        {
            if (value == null)
                return;

            Episode episode = (Episode) value;
            _currentEpisode = episode;
            EpisodeRatingValue = episode.Rating;

            IsEpisodeFlyoutOpen = true;
        }

        public bool IsEpisodeFlyoutOpen
        {
            get { return _isEpisodeFlyoutOpen; }
            set
            {
                _isEpisodeFlyoutOpen = value;
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

        public double EpisodeRatingValue
        {
            get { return _episodeRatingValue; }
            set
            {
                _episodeRatingValue = value;
                OnPropertyChanged();
            }
        }

        private void EpisodeSliderRateValueChanged(object obj)
        {
            int length = (int)obj;
            for (int i = 0; i < length; i++)
            {
                EpisodeIcons[i] = PackIconMaterialDesignKind.Star.ToString();
            }
            for (int i = 4; i >= length; i--)
            {
                EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
            }
        }

        public ICommand EpisodeOkEdit
        {
            get
            {
                if (_episodeOkEdit == null)
                {
                    _episodeOkEdit = new RelayCommand(
                        param => EpisodeOkEditCommand(),
                        param => CanEpisodeOkEditCommand()
                    );
                }
                return _episodeOkEdit;
            }
        }

        private bool CanEpisodeOkEditCommand()
        {
            return true;
        }

        private void EpisodeOkEditCommand()
        {
            _currentEpisode.Rating = (int) EpisodeRatingValue;
            Mediator.NotifyColleagues(MediatorGlobal.SaveEpisodeEditInfo, _currentEpisode);

            IsEpisodeFlyoutOpen = false;
            Mediator.NotifyColleagues(MediatorGlobal.OnRefreshListView, null);
        }

        public ICommand EpisodeCancelEdit
        {
            get
            {
                if (_episodeCancelEdit == null)
                {
                    _episodeCancelEdit = new RelayCommand(
                        param => EpisodeCancelEditCommand(),
                        param => CanEpisodeCancelEditCommand()
                    );
                }
                return _episodeCancelEdit;
            }
        }

        private bool CanEpisodeCancelEditCommand()
        {
            return true;
        }

        private void EpisodeCancelEditCommand()
        {
            EpisodesValue = "";
            IsFlyoutOpen = false;
        }

        #endregion
    }
}
