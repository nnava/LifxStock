using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using LifxStock.Adapters;
using LifxStock.Core.Model;
using LifxStock.Core.Service;
using System;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LifxStock
{
    [Activity(Label = "ChooseLamp")]
    public class ChooseLamp : AppCompatActivity
    {

        #region Properties

        private ListView lampListView;
        private SwipeRefreshLayout refresher;
        private List<Lamp> lamps;
        private bool changeLampMode;
        private int changeLampStockInfoId;
        private const string Tag = "ChooseLamp";

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            changeLampMode = Intent.GetBooleanExtra("changeLampMode", false);
            changeLampStockInfoId = Intent.GetIntExtra("changeLampStockInfoId", 0);
            
            SetContentView(Resource.Layout.ChooseLamp);

            FindViews();

            SetupToolbar();

            HandleEvents();

            LoadLampsToListView();
        }

        #region EventFunctions

        private void HandleEvents()
        {
            refresher.Refresh += HandleRefresh;
            lampListView.ItemClick += LampListView_ItemClick;
        }

        private void LampListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedLamp = lamps[e.Position];

            if(changeLampMode)
            {
                UpdateChangeLampSelectedStock(selectedLamp.LampId, selectedLamp.Name);

                var intentMainMonitor = new Intent(ApplicationContext, typeof(MainMonitor));
                intentMainMonitor.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intentMainMonitor);
                return;
            }

            var intent = new Intent();
            intent.SetClass(this, typeof(SearchStock));
            intent.PutExtra("selectedLampId", selectedLamp.LampId);
            intent.PutExtra("selectedLampLabel", selectedLamp.Name);

            Log.Debug(Tag, "Lampid" + selectedLamp.LampId);
            Log.Debug(Tag, "Lampname" + selectedLamp.Name);

            StartActivityForResult(intent, 100);
        }

        private void UpdateChangeLampSelectedStock(string lampId, string lampLabel)
        {
            StockInfoDataService stockInfoDataService = new StockInfoDataService();
            var stockInfo = stockInfoDataService.GetStockInfo(changeLampStockInfoId);
            stockInfo.LampId = lampId;
            stockInfo.LampLabel = lampLabel;

            stockInfoDataService.SaveStockInfo(stockInfo);
        }

        #endregion

        #region SetupFunctions

        private void SetupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "Choose lamp";

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        #endregion

        void HandleRefresh(object sender, EventArgs e)
        {
            LoadLampsToListView();
            refresher.Refreshing = false;
        }

        private async void LoadLampsToListView()
        {
            lamps = new List<Lamp>();

            var akavacheSettingsHelper = new AkavacheSettingsHelper();
            var lifxToken = await akavacheSettingsHelper.GetLifxToken();
            var lifxLampService = new LifxLampService(lifxToken);

            try
            {
                await lifxLampService.SetAvailabeLampsToList();

                foreach (var light in lifxLampService.Lamps)
                {
                    lamps.Add(new Lamp { Name = light.Label, LampId = light.UUID });
                }

                if(lamps.Count > 0)
                    lampListView.Adapter = new LampListAdapter(this, lamps);                  
                else
                    Toast.MakeText(this, "Found 0 Lifx-lamps, have you tried turning it off and then on again? /Tech-support", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Oh no, error when getting Lifx-lamps. Go to settings and enter a token!", ToastLength.Long).Show();
            }            
        }

        private void FindViews()
        {
            lampListView = FindViewById<ListView>(Resource.Id.lampListView);
            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }
    }
}