using System;

namespace HS_Feed_Manager.ViewModels.Interfaces
{
    interface ISettingsView
    {
        event EventHandler SaveConfig;
        event EventHandler RestoreLocalPathSettings;
        event EventHandler RestoreFeedLinkSettings;
        event EventHandler LogRefresh;

        void RefreshSettingsView();
    }
}
