using HS_Feed_Manager.ViewModels.Handler;
using System.Windows.Controls;

namespace HS_Feed_Manager.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private int _currentTabIndex = 0;

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
                    Mediator.NotifyColleagues("TabControlSelectionChanged", null);
                    _currentTabIndex = tabControl.SelectedIndex;
                }
            }
        }
    }
}
