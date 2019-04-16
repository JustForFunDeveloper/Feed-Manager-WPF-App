using HS_Feed_Manager.ViewModels.Common;

namespace HS_Feed_Manager.ViewModels
{
    public class AboutViewModel : PropertyChangedViewModel
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        public AboutViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }

}