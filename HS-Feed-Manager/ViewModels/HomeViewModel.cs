using MahApps.Metro.IconPacks;
using System.Collections.Generic;

namespace HS_Feed_Manager.ViewModels
{
    public class HomeViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private List<string> _icons = new List<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Icons[0] = PackIconMaterialDesignKind.Star.ToString();
        }

        public List<string> Icons
        {
            get => _icons;
            set
            {
                _icons = value;
                OnPropertyChanged();
            }
        }
    }
}