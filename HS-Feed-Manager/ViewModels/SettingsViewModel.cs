﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS_Feed_Manager.ViewModels
{
    public class SettingsViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        public SettingsViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}