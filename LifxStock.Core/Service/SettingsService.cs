using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace LifxStock.Core.Service
{
    public static class SettingsService
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string LifxMonitoringKey = "lifxmonitoring_key";
        private static readonly bool LifxMonitoringDefault = false;

        private const string HasUserSeenStartupInfoKey = "hasuserseenstartupinfo_key";
        private static readonly bool HasUserSeenStartupInfoDefault = false;

        #endregion

        public static bool LifxMonitoring
        {
            get { return AppSettings.GetValueOrDefault<bool>(LifxMonitoringKey, LifxMonitoringDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(LifxMonitoringKey, value); }
        }

        public static bool HasUserSeenStartupInfo
        {
            get { return AppSettings.GetValueOrDefault<bool>(HasUserSeenStartupInfoKey, HasUserSeenStartupInfoDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(HasUserSeenStartupInfoKey, value); }
        }
    }
}