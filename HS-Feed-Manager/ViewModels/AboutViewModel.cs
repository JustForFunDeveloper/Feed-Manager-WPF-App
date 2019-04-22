using System.Reflection;
using System.Windows.Input;
using HS_Feed_Manager.ViewModels.Common;
using HS_Feed_Manager.ViewModels.Handler;

namespace HS_Feed_Manager.ViewModels
{
    public class AboutViewModel : PropertyChangedViewModel
    {
        // TODO: Add Version, Link to Github and Homepage.Project, Add Link to Homepage Description Help
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        private string _version;
        private ICommand _newReleases;
        private ICommand _homepage;
        private ICommand _gitHubProject;
        private ICommand _license;

        public AboutViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _version = Assembly.GetEntryAssembly().GetName().Version + " Beta";
        }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewReleases
        {
            get
            {
                if (_newReleases == null)
                    _newReleases = new RelayCommand(
                        param => NewReleasesCommand(),
                        param => CanNewReleasesCommand()
                    );
                return _newReleases;
            }
        }

        private bool CanNewReleasesCommand()
        {
            return true;
        }

        private void NewReleasesCommand()
        {
            System.Diagnostics.Process.Start("https://github.com/JustForFunDeveloper/Feed-Manager-WPF-App/releases");
        }

        public ICommand Homepage
        {
            get
            {
                if (_homepage == null)
                    _homepage = new RelayCommand(
                        param => HomepageCommand(),
                        param => CanHomepageCommand()
                    );
                return _homepage;
            }
        }

        private bool CanHomepageCommand()
        {
            return true;
        }

        private void HomepageCommand()
        {
            System.Diagnostics.Process.Start("https://www.die-technik-und-ich.at/");
        }

        public ICommand GitHubProject
        {
            get
            {
                if (_gitHubProject == null)
                    _gitHubProject = new RelayCommand(
                        param => GitHubProjectCommand(),
                        param => CanGitHubProjectCommand()
                    );
                return _gitHubProject;
            }
        }

        private bool CanGitHubProjectCommand()
        {
            return true;
        }

        private void GitHubProjectCommand()
        {
            System.Diagnostics.Process.Start("https://github.com/JustForFunDeveloper/Feed-Manager-WPF-App");
        }

        public ICommand License
        {
            get
            {
                if (_license == null)
                    _license = new RelayCommand(
                        param => LicenseCommand(),
                        param => CanLicenseCommand()
                    );
                return _license;
            }
        }

        private bool CanLicenseCommand()
        {
            return true;
        }

        private void LicenseCommand()
        {
            System.Diagnostics.Process.Start("https://github.com/JustForFunDeveloper/Feed-Manager-WPF-App/blob/master/LICENSE");
        }
    }

}