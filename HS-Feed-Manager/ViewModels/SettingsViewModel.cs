using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HS_Feed_Manager.ViewModels.Common;

namespace HS_Feed_Manager.ViewModels
{
    public class SettingsViewModel : PropertyChangedViewModel
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly PropertyChangedViewModel _mainViewModel;

        public SettingsViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
