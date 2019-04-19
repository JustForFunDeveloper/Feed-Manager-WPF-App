using HS_Feed_Manager.ViewModels.Handler;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class HomeView : UserControl
    {
        private int _currentTabIndex;

        public HomeView()
        {
            InitializeComponent();
        }

        private void TC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                TabControl tabControl = (TabControl)sender;
                if (tabControl.SelectedIndex != _currentTabIndex)
                {
                    Mediator.NotifyColleagues(MediatorGlobal.TabControlSelectionChanged, null);
                    _currentTabIndex = tabControl.SelectedIndex;
                }
            }
        }

        private void EpisodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                ListBox listBox = (ListBox)sender;
                Mediator.NotifyColleagues(MediatorGlobal.ListBoxSelectionChanged, listBox.SelectedItem);
            }
        }

        private void FeedList_Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Mediator.NotifyColleagues(MediatorGlobal.ListBoxDoubleClick, null);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            if (selector is ListBox)
            {
                (selector as ListBox).ScrollIntoView(selector.SelectedItem);
            }
        }

        private void EpisodeList_Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                ListBox listBox = (ListBox)sender;
                Mediator.NotifyColleagues(MediatorGlobal.PlayEpisode,(Episode) listBox.SelectedItem);
            }
        }

        private void Feed_Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            if (selector is ListBox)
            {
                (selector as ListBox).ScrollIntoView(selector.SelectedItem);
            }
        }
    }
}
