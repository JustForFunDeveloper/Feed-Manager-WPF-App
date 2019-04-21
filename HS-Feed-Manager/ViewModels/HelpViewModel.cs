using HS_Feed_Manager.ViewModels.Common;

namespace HS_Feed_Manager.ViewModels
{
    public class HelpViewModel : PropertyChangedViewModel
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        public HelpViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
