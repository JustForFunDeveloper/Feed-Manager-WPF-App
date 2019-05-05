using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.Core.Handler;
using JetBrains.Annotations;
using PropertyChanged;

namespace HS_Feed_Manager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private TvShow _currentTvShow;
        private Episode _currentEpisode;

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

        public bool IsPaneOpened { get; set; }
        public bool IsFlyoutOpen { get; set; }
        public bool IsEpisodeFlyoutOpen { get; set; }
        public int SelectedIndex { get; set; }

        public int SelectedStatus { get; set; }
        public string SelectedValueStatus { get; set; }
        public string Episodes { get; set; }
        public string EpisodesValue { get; set; }
        public int SelectedAutoDownload { get; set; }
        public string SelectedValueAutoDownload { get; set; }
        public double RatingValue { get; set; }
        public double EpisodeRatingValue { get; set; }

        public ObservableCollection<string> Icons { get; set; }
        public ObservableCollection<string> EpisodeIcons { get; set; }
        private readonly ObservableCollection<string> _icons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };
        private readonly ObservableCollection<string> _episodeIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        #endregion

        public MainViewModel()
        {
            Icons = _icons;
            EpisodeIcons = _episodeIcons;
            CreateMenuItems();
            Mediator.Register(MediatorGlobal.UpdateFlyoutValues, UpdateFlyoutValues);
            Mediator.Register(MediatorGlobal.UpdateEpisodeFlyoutValues, UpdateEpisodeFlyoutValues);
            Mediator.Register(MediatorGlobal.SliderRateValueChanged, SliderRateValueChanged);
            Mediator.Register(MediatorGlobal.EpisodeSliderRateValueChanged, EpisodeSliderRateValueChanged);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnCustomPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    Tag = new HomeViewModel()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Settings},
                    Label = "Settings",
                    ToolTip = "The Application settings.",
                    Tag = new SettingsViewModel()
                }
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Help},
                    Label = "Help",
                    ToolTip = "Program usage.",
                    Tag = new HelpViewModel()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.InformationVariant},
                    Label = "About",
                    ToolTip = "About this Software.",
                    Tag = new AboutViewModel()
                }
            };
        }

        [DoNotNotify]
        public HamburgerMenuItemCollection MenuItems
        {
            get => _menuItems;
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                OnCustomPropertyChanged();
            }
        }

        [DoNotNotify]
        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems)) return;
                _menuOptionItems = value;
                OnCustomPropertyChanged();
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
            try
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
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("UpdateFlyoutValues: " + ex, LogLevel.Error);
            }
        }

        public IEnumerable<Status> StatusValues
        {
            get { return Enum.GetValues(typeof(Status)).Cast<Status>(); }
        }

        public IEnumerable<AutoDownload> AutoDownloadValues
        {
            get { return Enum.GetValues(typeof(AutoDownload)).Cast<AutoDownload>(); }
        }

        private void SliderRateValueChanged(object obj)
        {
            try
            {
                int length = (int) obj;
                for (int i = 0; i < length; i++)
                {
                    Icons[i] = PackIconMaterialDesignKind.Star.ToString();
                }

                for (int i = 4; i >= length; i--)
                {
                    Icons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("SliderRateValueChanged: " + ex, LogLevel.Error);
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
            try
            {
                TvShow localTvShow = _currentTvShow;
                localTvShow.Status = (Status) Enum.Parse(typeof(Status), SelectedValueStatus);
                if (int.TryParse(EpisodesValue, out int result))
                    localTvShow.EpisodeCount = result;
                localTvShow.AutoDownloadStatus =
                    (AutoDownload) Enum.Parse(typeof(AutoDownload), SelectedValueAutoDownload);
                localTvShow.Rating = (int) RatingValue;
                Mediator.NotifyColleagues(MediatorGlobal.SaveEditInfo, localTvShow);

                IsFlyoutOpen = false;
                Mediator.NotifyColleagues(MediatorGlobal.OnRefreshListView, null);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("SaveOkEditCommand: " + ex, LogLevel.Error);
            }
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

        private void EpisodeSliderRateValueChanged(object obj)
        {
            int length = (int) obj;
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
            IsEpisodeFlyoutOpen = false;
        }

        #endregion
    }
}
