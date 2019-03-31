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
    }
}
