using System.Windows;
using HS_Feed_Manager.DataModels;

namespace HS_Feed_Manager
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CurrentData.Seed();
        }
    }
}
