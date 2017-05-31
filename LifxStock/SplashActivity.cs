using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using LifxStock.Core;
using LifxStock.Core.Service;
using System.Threading.Tasks;

namespace LifxStock
{
    [Activity(Theme = "@style/Theme.LifxStockStartup", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            var startupWork = new Task(() =>
            {
                SetStaticListValues();
            });

            startupWork.ContinueWith(t =>
                                     {
                                         StartActivity(new Intent(Application.Context, typeof(MainMonitor)));
                                     }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }

        private void SetStaticListValues()
        {
            var stockInfoDataService = new StockInfoDataService();
            var stockInfoList = AsyncHelpers.RunSync(stockInfoDataService.GetAllStockInfos);
            if (stockInfoList != null)
                StaticStockInfoList.stockInfoList = stockInfoList;

            if (!SettingsService.LifxMonitoring) return;

            var akavacheSettingsHelper = new AkavacheSettingsHelper();
            var lifxToken = AsyncHelpers.RunSync<string>(akavacheSettingsHelper.GetLifxToken);
            var lifxLampService = new LifxLampService(lifxToken);
            AsyncHelpers.RunSync(lifxLampService.SetStaticLampList);
        }
    }
}