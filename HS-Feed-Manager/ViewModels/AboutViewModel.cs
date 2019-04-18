using HS_Feed_Manager.ViewModels.Common;

namespace HS_Feed_Manager.ViewModels
{
    public class AboutViewModel : PropertyChangedViewModel
    {
        // TODO: Add Version, Link to Github and Homepage.Project, Add Link to Homepage Description Help
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        public AboutViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }

}