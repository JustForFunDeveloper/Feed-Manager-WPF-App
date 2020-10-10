using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.ViewModels.Handler;

namespace HS_Feed_Manager.ViewModels
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AboutViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Version { get; set; }

        private ICommand _newReleases;
        private ICommand _homepage;
        private ICommand _gitHubProject;
        private ICommand _license;

        public AboutViewModel()
        {
            try
            {
                string version = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
                if (version != null) Version = "v" + version.Remove(version.Length - 2);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("AboutViewModel: " + ex, LogLevel.Error);
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
            try
            {
                System.Diagnostics.Process.Start("https://github.com/JustForFunDeveloper/Feed-Manager-WPF-App/releases");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("NewReleasesCommand: " + ex, LogLevel.Error);
            }
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
            try
            {
                System.Diagnostics.Process.Start("https://www.die-technik-und-ich.at/?page_id=557");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("HomepageCommand: " + ex, LogLevel.Error);
            }
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
            try
            {
                System.Diagnostics.Process.Start("https://github.com/JustForFunDeveloper/Feed-Manager-WPF-App");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GitHubProjectCommand: " + ex, LogLevel.Error);
            }
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
            try
            {
                System.Diagnostics.Process.Start("https://github.com/JustForFunDeveloper/Feed-Manager-WPF-App/blob/master/LICENSE");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GitHubProjectCommand: " + ex, LogLevel.Error);
            }
        }
    }

}