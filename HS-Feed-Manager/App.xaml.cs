using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HS_Feed_Manager.Core;
using Serilog;

namespace HS_Feed_Manager
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App
    {
        private MainWindow _mainWindow;
        private Logic _logic;
        
        public App()
        {
            // ReSharper disable once ObjectCreationAsStatement
            SetupUnhandledExceptionHandling();
            _logic = new Logic();
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.SQLite("local.db")
                    .CreateLogger();

                _mainWindow = new MainWindow();
                Log.Information("Started Application!");
            }
            catch (Exception exception)
            {
                Log.Error(exception, "OnStartup");
                OnContextClose(null, null);
            }

            base.OnStartup(e);
        }
        
        private void OnContextClose(object sender, EventArgs e)
        {
            Log.Information("Closing application");
            Log.CloseAndFlush();
            Thread.Sleep(1000);
            Shutdown();
        }
        
        private void SetupUnhandledExceptionHandling()
        {
            // Catch exceptions from all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");

            // Catch exceptions from a single specific UI dispatcher thread.
            Dispatcher.UnhandledException += (sender, args) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if (!Debugger.IsAttached)
                {
                    args.Handled = true;
                    ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException");
                }
            };
        }

        void ShowUnhandledException(Exception e, string unhandledExceptionType)
        {
            Log.Logger.Error(e, "Unexpected Error Occurred!");
            OnContextClose(null, null);
        }
    }
}
