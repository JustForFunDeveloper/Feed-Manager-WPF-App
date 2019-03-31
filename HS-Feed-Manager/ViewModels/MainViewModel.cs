using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Input;

namespace HS_Feed_Manager.ViewModels
{
    public class MainViewModel : PropertyChangedViewModel
    {
        private static MainViewModel _mainViewModel;

        private Visibility _hideWindowCommands = Visibility.Hidden;

        private ICommand _logIn;
        private ICommand _closeWindow;
        private ICommand _itemClick;
        private ICommand _optionItemClick;

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        private bool _isPaneOpened;
        private int _selectedIndex = 0;

        private MetroWindow _loginWindow;

        public MainViewModel()
        {
            this.CreateMenuItems();
            _mainViewModel = this;
            Mediator.Register("OnViewChange", OnViewChange);
        }

        public static MainViewModel getInstance { get => _mainViewModel; }

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

        public Visibility HideWindowCommands
        {
            get { return _hideWindowCommands; }
            set
            {
                _hideWindowCommands = value;
                OnPropertyChanged();
            }
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

        public bool IsPaneOpened
        {
            get { return _isPaneOpened; }
            set
            {
                _isPaneOpened = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseWindow
        {
            get
            {
                if (_closeWindow == null)
                {
                    _closeWindow = new RelayCommand(
                        param => this.SaveCloseWindowCommand(),
                        param => this.CanSaveCloseWindowCommand()
                    );
                }
                return _closeWindow;
            }
        }

        private bool CanSaveCloseWindowCommand()
        {
            return true;
        }

        private void SaveCloseWindowCommand()
        {
            if (_loginWindow != null)
                _loginWindow.Close();
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
            switch (param)
            {
                case 0:
                    Mediator.NotifyColleagues("ReloadHomeView", true);
                    break;
                case 1:
                    Mediator.NotifyColleagues("ReloadSettingsView", true);
                    break;
                default:
                    break;
            }
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
            //Reload AboutView not needed at the moment

            //HamburgerMenu hamburgerMenu = (HamburgerMenu)sender;
            //switch (hamburgerMenu.SelectedOptionsIndex)
            //{
            //    case 0:
            //        Mediator.NotifyColleagues("ReloadAboutView", true);
            //        break;
            //    default:
            //        break;
            //}
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        private void OnViewChange(object value)
        {
            //KeyValuePair<int, LogLevel> keyValuePair = (KeyValuePair<int, LogLevel>)value;
            //SelectedIndex = keyValuePair.Key;
            //Mediator.NotifyColleagues("OnLogLevel", keyValuePair.Value);
        }
    }
}
