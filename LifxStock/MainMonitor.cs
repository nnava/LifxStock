using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using LifxStock.Adapters;
using LifxStock.Core;
using LifxStock.Core.Extensions;
using LifxStock.Core.Model;
using LifxStock.Core.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LifxStock
{
    [Activity(Label = "LIFX Stockmonitor",
        MainLauncher = false,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
        Icon = "@drawable/icon",
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainMonitor : AppCompatActivity
    {
        #region Properties

        private FloatingActionButton fabAddMonitor;
        private Toolbar toolbar;
        private StockInfoDataService stockInfoDataService;
        private Timer timerGetStockInfo;
        private object lockStockInfoList = new object();
        private ExpandableListViewAdapter mAdapter;
        private ExpandableListView lvStockMonitor;
        private SwipeRefreshLayout refresher;
        private Dictionary<string, ObservableCollection<StockInfo>> dicStockInfos = new Dictionary<string, ObservableCollection<StockInfo>>();
        private bool isRunning;
        public static int timerIntervalValue = 5000;

        private static readonly string Tag = "MainMonitor";
        private const string MenuChangeLamp = "change lamp";
        private const string MenuDeleteItem = "delete";

        public bool hasPostedLostNetwork = false;

        #endregion

        public MainMonitor()
        {
            stockInfoDataService = new StockInfoDataService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainMonitor);

            FindViews();

            SetupToolbar();

            HandleEvents();

            RegisterForContextMenu(lvStockMonitor);

            SetCurrentCultureInfo();

            DisplayStartupInfoMessage();
        }

        private async Task SetStaticLampListValues()
        {
            if (!SettingsService.LifxMonitoring) return;

            var akavacheSettingsHelper = new AkavacheSettingsHelper();
            var lifxToken = await akavacheSettingsHelper.GetLifxToken();
            var lifxLampService = new LifxLampService(lifxToken);
            await lifxLampService.SetStaticLampList();
        }

        private void DisplayStartupInfoMessage()
        {
            if (SettingsService.HasUserSeenStartupInfo) return;

            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Information");
            alert.SetMessage("Market data from Yahoo Finance. Data are provided for information purposes only, not intended for trading purposes.");
            alert.SetPositiveButton("OK, i got it!", (senderAlert, args) =>
            {
                SettingsService.HasUserSeenStartupInfo = true;
            });

            alert.Show();
        }

        private static void SetCurrentCultureInfo()
        {
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        private void HandleEvents()
        {
            fabAddMonitor.Click += FabAddMonitor_Click;
            refresher.Refresh += HandleRefresh;
        }

        async void HandleRefresh(object sender, EventArgs e)
        {
            await RunUpdateListview();
            refresher.Refreshing = false;
        }

        protected override async void OnResume()
        {
            base.OnResume();

            if (timerGetStockInfo == null)
            {
                await LoadLampsToListView(true);
                InitializeTimer();
                ExpandAllGroups();
            }
            else if (timerGetStockInfo != null && !timerGetStockInfo.Enabled)
            {
                timerGetStockInfo.Enabled = true;
                timerGetStockInfo.Start();
                await UpdateStocksInfoListAsync();
                ExpandAllGroups();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (timerGetStockInfo != null && timerGetStockInfo.Enabled)
            {
                timerGetStockInfo.Enabled = false;
                timerGetStockInfo.Stop();
            }
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.Id.lvStockMonitor)
            {
                while (isRunning)
                {
                    timerGetStockInfo.Stop();
                }
                timerGetStockInfo.Enabled = false;

                var info = (ExpandableListView.ExpandableListContextMenuInfo)menuInfo;
                var groupPosition = ExpandableListView.GetPackedPositionGroup(info.PackedPosition);
                var childPosition = ExpandableListView.GetPackedPositionChild(info.PackedPosition);
                var selectedStockInfo = mAdapter.GetStockInfo(groupPosition, childPosition);
                menu.SetHeaderTitle(selectedStockInfo.Symbol);
                var menuItems = Resources.GetStringArray(Resource.Array.menuListViewStock);
                for (var i = 0; i < menuItems.Length; i++)
                {
                    if (IsLifxMonitoringDisabled() && IsMenuItemChangeLamp(menuItems, i)) continue;

                    menu.Add(Menu.None, i, i, menuItems[i]);
                }

                timerGetStockInfo.Start();
                timerGetStockInfo.Enabled = true;
            }
        }

        private static bool IsMenuItemChangeLamp(string[] menuItems, int i)
        {
            return menuItems[i].ToLower() == MenuChangeLamp;
        }

        private static bool IsLifxMonitoringDisabled()
        {
            return !SettingsService.LifxMonitoring;
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (ExpandableListView.ExpandableListContextMenuInfo)item.MenuInfo;

            var groupPosition = ExpandableListView.GetPackedPositionGroup(info.PackedPosition);
            var childPosition = ExpandableListView.GetPackedPositionChild(info.PackedPosition);
            var menuItemIndex = item.ItemId;
            var menuItems = Resources.GetStringArray(Resource.Array.menuListViewStock);
            var menuItemName = menuItems[menuItemIndex].ToLower();
            var selectedStockInfo = mAdapter.GetStockInfo(groupPosition, childPosition);
            var listItemName = selectedStockInfo.Symbol;

            switch (menuItemName)
            {
                case MenuDeleteItem:
                    while (isRunning)
                    {
                        timerGetStockInfo.Stop();
                    }
                    timerGetStockInfo.Enabled = false;

                    DeleteStockInfoFromDb(selectedStockInfo);
                    DeleteStockInfoFromList(selectedStockInfo);
                    UpdateStocksInfoList();

                    timerGetStockInfo.Enabled = true;
                    timerGetStockInfo.Start();
                    break;
                case MenuChangeLamp:
                    var changeLampIntent = new Intent(ApplicationContext, typeof(ChooseLamp));
                    changeLampIntent.PutExtra("changeLampMode", true);
                    changeLampIntent.PutExtra("changeLampStockInfoId", selectedStockInfo.ID);
                    StartActivity(changeLampIntent);
                    break;
            }

            return true;
        }

        private void DeleteStockInfoFromDb(StockInfo selectedStockInfo)
        {
            stockInfoDataService.DeleteStockInfo(selectedStockInfo);
        }

        private void DeleteStockInfoFromList(StockInfo selectedStockInfo)
        {
            lock (lockStockInfoList)
            {
                StaticStockInfoList.stockInfoList.Remove(selectedStockInfo);
            }
        }

        private void SetupToolbar()
        {
            SetSupportActionBar(toolbar);

            SetupToolbarTitle("   LIFX Stockmonitor");
            SupportActionBar.SetIcon(Resource.Drawable.Icon_min);
        }

        private void SetupToolbarTitle(string title)
        {
            var st1 = new SpannableString(title);

            Java.Lang.ICharSequence sequence1;
            sequence1 = st1.SubSequenceFormatted(0, st1.Length());

            SupportActionBar.TitleFormatted = sequence1;
        }

        private void FabAddMonitor_Click(object sender, System.EventArgs e)
        {
            if (SettingsService.LifxMonitoring)
            {
                StartActivity(typeof(ChooseLamp));
            }
            else
            {
                var intent = new Intent();
                intent.SetClass(this, typeof(SearchStock));
                intent.PutExtra("selectedLampId", "ManualLampGroupId|");
                intent.PutExtra("selectedLampLabel", "Stocks");

                StartActivityForResult(intent, 100);
            }
        }

        private void FindViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            fabAddMonitor = FindViewById<FloatingActionButton>(Resource.Id.fabAddMonitor);
            lvStockMonitor = FindViewById<ExpandableListView>(Resource.Id.lvStockMonitor);
            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
        }

        private async Task LoadLampsToListView(bool initialLoad = false)
        {
            if (!initialLoad)
            {
                var stockInfoList = await stockInfoDataService.GetAllStockInfos();
                if (stockInfoList != null)
                    StaticStockInfoList.stockInfoList = stockInfoList;
            }

            UpdateLampsColors();

            List<string> lamps = SetDictionaryData();

            mAdapter = new ExpandableListViewAdapter(this, lamps, dicStockInfos);
            lvStockMonitor.SetAdapter(mAdapter);
        }

        private List<string> SetDictionaryData()
        {
            lock (lockStockInfoList)
            {
                var lamps = StaticStockInfoList.stockInfoList.GroupBy(e => e.LampLabel).Select(e => e.Key).ToList();
                dicStockInfos = new Dictionary<string, ObservableCollection<StockInfo>>();

                foreach (var lamp in lamps.ToList())
                {
                    var stockInfoSymbolsOnLamp = StaticStockInfoList.stockInfoList.Where(e => e.LampLabel == lamp).ToList();

                    dicStockInfos.Add(lamp, stockInfoSymbolsOnLamp.ToObservableCollection());
                }

                return lamps;
            }
        }

        private async void UpdateLampsColors()
        {
            if (!SettingsService.LifxMonitoring) return;

            var akavacheSettingsHelper = new AkavacheSettingsHelper();
            var lifxToken = await akavacheSettingsHelper.GetLifxToken();
            var lifxLampService = new LifxLampService(lifxToken);

            var lamps = StaticStockInfoList.stockInfoList.GroupBy(e => e.LampId);

            foreach (var lamp in lamps)
            {
                var procentChange = lamp.Average(e => e.ProcentChange);

                var color = LifxLampService.GetColorByProcent(procentChange);
                await lifxLampService.SetLightAsync(color, lamp.Key);
            }

            await lifxLampService.SetStaticLampList();
        }

        private void InitializeTimer()
        {
            timerGetStockInfo?.Start();

            timerGetStockInfo = new Timer();
            timerGetStockInfo.Interval = timerIntervalValue;
            timerGetStockInfo.Elapsed += new ElapsedEventHandler(timerGetStockInfo_Elapsed);
            timerGetStockInfo.AutoReset = false;
            timerGetStockInfo.Start();
        }

        private void PostLostNetworkToUser()
        {
            hasPostedLostNetwork = true;
            RunOnUiThread(() => Toast.MakeText(this, "LOST NETWORK", ToastLength.Long).Show());
        }

        private async void timerGetStockInfo_Elapsed(object sender, ElapsedEventArgs e)
        {
            await RunUpdateListview();
        }

        private async Task RunUpdateListview()
        {
            isRunning = true;

            var networkState = GetNetworkState();
            if (networkState == NetworkState.Disconnected)
            {
                timerIntervalValue = 4000;
                timerGetStockInfo.Start();

                if (!hasPostedLostNetwork) PostLostNetworkToUser();
                return;
            }

            timerIntervalValue = networkState == NetworkState.ConnectedWifi ? 3000 : 5000;
            timerGetStockInfo.Interval = timerIntervalValue;

            await UpdateStocksInfoListAsync();
            timerGetStockInfo.Start();
            isRunning = false;
            hasPostedLostNetwork = false;
        }

        private static NetworkState GetNetworkState()
        {
            var networkStatusMonitor = new NetworkStatusMonitor();
            var networkState = networkStatusMonitor.State;
            return networkState;
        }

        private void UpdateStocksInfoList()
        {
            var lamps = SetDictionaryData();

            UpdateLampsColors();
            RunOnUiThread(() => mAdapter.SetListValues(lamps, dicStockInfos));
            RunOnUiThread(() => mAdapter.NotifyDataSetChanged());
        }

        private async Task UpdateStocksInfoListAsync()
        {
            var stockInfoList = await stockInfoDataService.GetAllStockInfos();
            if (stockInfoList != null)
                StaticStockInfoList.stockInfoList = stockInfoList;

            var lamps = SetDictionaryData();

            UpdateLampsColors();
            RunOnUiThread(() => mAdapter.SetListValues(lamps, dicStockInfos));
            RunOnUiThread(() => mAdapter.NotifyDataSetChanged());
        }

        private void ExpandAllGroups()
        {
            for (int i = 0; i < mAdapter.GroupCount; i++)
            {
                lvStockMonitor.ExpandGroup(i);
            }
        }

        #region OverrideFunctions

        /// <Docs>The options menu in which you place your items.</Docs>
        /// <returns>To be added.</returns>
        /// <summary>
        /// This is the menu for the Toolbar/Action Bar to use
        /// </summary>
        /// <param name="menu">Menu.</param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            switch (item.TitleFormatted.ToString().ToLower())
            {
                case "settings":
                    Menu_Settings_Clicked();
                    break;
                case "license":
                    Menu_License_Clicked();
                    break;
                case "buy lifx-bulb":
                    Menu_Buy_Lifx_Clicked();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void Menu_Buy_Lifx_Clicked()
        {
            var uri = Android.Net.Uri.Parse("http://www.lifx.com/collections");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Menu_License_Clicked()
        {
            StartMenuItem(typeof(License));
        }

        private void Menu_Settings_Clicked()
        {
            StartMenuItem(typeof(Settings));
        }

        private void StartMenuItem(Type type)
        {
            var intent = new Intent();
            intent.SetClass(this, type);

            StartActivityForResult(intent, 100);
        }

        #endregion
    }
}