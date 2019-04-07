using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HS_Feed_Manager.ViewModels
{
    public class MainViewModel : PropertyChangedViewModel
    {
        private static MainViewModel _mainViewModel;

        public static MainViewModel getInstance { get => _mainViewModel; }

        private ICommand _openMenu;
        private ICommand _itemClick;
        private ICommand _optionItemClick;
        private ICommand _okEdit;
        private ICommand _cancelEdit;

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        private bool _isPaneOpened;
        private bool _isFlyoutOpen;
        private int _selectedIndex = 0;

        private string _itemName = "";
        private string _itemNameValue;
        private int _selectedStatus;
        private string _selectedValueStatus;
        private string _episodes;
        private string _episodesValue;
        private int _selectedAutoDownload;
        private string _selectedValueAutoDownload;
        private double _ratingValue;

        private ObservableCollection<string> _icons = new ObservableCollection<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        public MainViewModel()
        {
            this.CreateMenuItems();
            _mainViewModel = this;
            Mediator.Register("UpdateFlyoutValues", UpdateFlyoutValues);
            Mediator.Register("SliderRateValueChanged", SliderRateValueChanged);
        }

        #region Hamburger Menu
        public void CreateMenuItems()
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
            get { return _menuItems; }
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
                        param => this.SaveOpenMenuCommand(),
                        param => this.CanSaveOpenMenuCommand()
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
                        param => this.ItemClickCommand(param),
                        param => this.CanItemClickCommand(param)
                    );
                }
                return _itemClick;
            }
        }

        private bool CanItemClickCommand(int param)
        {
            return true;
        }

        private void ItemClickCommand(int param)
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
                        param => this.OptionItemClickCommand(param),
                        param => this.CanOptionItemClickCommand(param)
                    );
                }
                return _optionItemClick;
            }
        }

        private bool CanOptionItemClickCommand(int param)
        {
            return true;
        }

        private void OptionItemClickCommand(int param)
        {
            IsPaneOpened = false;
        }
        #endregion

        #region Flyout
        private void UpdateFlyoutValues(object value)
        {
            LocalSeries localSeries = value as LocalSeries;
            ItemName = localSeries.Name;
            int statusIndex = Array.IndexOf(Enum.GetValues(localSeries.Status.GetType()), localSeries.Status);
            SelectedStatus = statusIndex;
            Episodes = localSeries.Episodes.ToString();
            int autoDownloadIndex = Array.IndexOf(Enum.GetValues(localSeries.AutoDownloadStatus.GetType()), localSeries.AutoDownloadStatus);
            SelectedAutoDownload = autoDownloadIndex;
            //SliderRateValueChanged(localSeries.Rating);
            RatingValue = localSeries.Rating;

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

        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                OnPropertyChanged();
            }
        }

        public string ItemNameValue
        {
            get { return _itemNameValue; }
            set
            {
                _itemNameValue = value;
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
                        param => this.SaveOkEditCommand(),
                        param => this.CanSaveOkEditCommand()
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
            LocalSeries localSeries = new LocalSeries();
            localSeries.Name = ItemNameValue;
            localSeries.Status = (Status)Enum.Parse(typeof(Status), SelectedValueStatus);
            localSeries.Episodes = int.Parse(EpisodesValue);
            localSeries.AutoDownloadStatus = (AutoDownload)Enum.Parse(typeof(AutoDownload), SelectedValueAutoDownload);
            localSeries.Rating = (int)RatingValue;
            //TODO: Save Data
            Mediator.NotifyColleagues("SaveEditInfo", localSeries);

            IsFlyoutOpen = false;
        }

        public ICommand CancelEdit
        {
            get
            {
                if (_cancelEdit == null)
                {
                    _cancelEdit = new RelayCommand(
                        param => this.SaveCancelEditCommand(),
                        param => this.CanSaveCancelEditCommand()
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
            ItemNameValue = "";
            EpisodesValue = "";
            IsFlyoutOpen = false;
        }

        #endregion
    }
}
