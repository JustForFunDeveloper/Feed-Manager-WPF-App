using System.Collections.Generic;
using HS_Feed_Manager.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace HS_Feed_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private static MetroWindow _mainView;
        private bool _closeMe;

        public MainWindow()
        {
            InitializeComponent();
            _mainView = this;
            Mediator.Register(MediatorGlobal.CustomDialog, OnCustomDialog);
            Closing += WindowClosing;
        }

        private void OnCustomDialog(object obj)
        {
            if (obj == null)
                return;
            List<string> list = obj as List<string>;

            if (list == null)
                return;

            if (list.Count != 3)
                return;

            CustomDialog(list[0], list[1], list[2]);
        }

        private async void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;
            e.Cancel = !_closeMe;
            if (_closeMe) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowMessageAsync(
                "Exit HS-Feed-Manager?",
                "Are you sure you want to exit?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _closeMe = result == MessageDialogResult.Affirmative;

            if (_closeMe) Close();
        }

        public static MetroWindow GetInstance
        {
            get => _mainView;
        }

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Mediator.NotifyColleagues(MediatorGlobal.SliderRateValueChanged, (int) e.NewValue);
        }

        private void EpisodeSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Mediator.NotifyColleagues(MediatorGlobal.EpisodeSliderRateValueChanged, (int) e.NewValue);
        }

        private async void CustomDialog(string identifier, string title, string message)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Confirm",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync(
                title,
                message,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            Mediator.NotifyColleagues(MediatorGlobal.CustomDialogReturn, new List<object>()
            {
                identifier,
                result == MessageDialogResult.Affirmative
            });
        }
    }
}
