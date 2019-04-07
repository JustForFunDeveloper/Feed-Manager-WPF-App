using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.Controls;

namespace HS_Feed_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static MetroWindow _mainView;

        public MainWindow()
        {
            InitializeComponent();
            _mainView = this;
        }

        public static MetroWindow GetInstance { get => _mainView; }

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Mediator.NotifyColleagues("SliderRateValueChanged", (int)e.NewValue);
        }
    }
}
