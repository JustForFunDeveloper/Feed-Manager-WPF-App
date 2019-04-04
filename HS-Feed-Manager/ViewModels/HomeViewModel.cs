using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HS_Feed_Manager.ViewModels
{
    public class HomeViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private ICommand _textBoxButtonCmd;
        private int _comboBoxIndex;

        private List<string> _icons = new List<string> {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString() };

        private string _filterString = "";

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Icons[0] = PackIconMaterialDesignKind.Star.ToString();
            InitialiseViews();
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

        public int ComboBoxIndex
        {
            get => _comboBoxIndex;
            set
            {
                _comboBoxIndex = value;
                SortThisList(value);
                OnPropertyChanged();
            }
        }

        private void SortThisList(int value)
        {
            using (SortedArtistsView.DeferRefresh())
            {
                SortedArtistsView.SortDescriptions.Clear();
                if (value == 0)
                    SortedArtistsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                if (value == 1)
                    SortedArtistsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            }
        }

        public ICollectionView SortedArtistsView { get; private set; }

        private void InitialiseViews()
        {
            InitialiseArtistsView();
        }

        private void InitialiseArtistsView()
        {
            SortedArtistsView = CollectionViewSource.GetDefaultView(SampleData.Artists);
            SortedArtistsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            SortedArtistsView.Filter = CustomFilter;
        }

        private bool CustomFilter(object item)
        {
            Artist artist = item as Artist;
            return artist.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public ICommand TextBoxButtonCmd
        {
            get
            {
                if (_textBoxButtonCmd == null)
                {
                    _textBoxButtonCmd = new RelayCommand<object>(
                        param => this.ItemClickCommand(param),
                        param => this.CanItemClickCommand(param)
                    );
                }
                return _textBoxButtonCmd;
            }
        }

        private bool CanItemClickCommand(object param)
        {
            return true;
        }

        private void ItemClickCommand(object param)
        {

            if (param.GetType() == typeof(string))
            {
                _filterString = param as string;
                SortedArtistsView.Refresh();
            } else if (param.GetType() == typeof(TextBox))
            {
                TextBox textBox = param as TextBox;
                if (textBox.Text.Length == 0)
                {
                    _filterString = "";
                    SortedArtistsView.Refresh();
                }
            }
        }
    }
}