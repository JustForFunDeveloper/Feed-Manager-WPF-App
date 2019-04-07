using HS_Feed_Manager.DataModels;
using System.Windows;

namespace HS_Feed_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SampleData.Seed();
            CurrenData.Seed();
        }
    }
}
