/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using LifxStock.Core.Service;
using Android.Util;

namespace LifxStock
{
    [Activity(Label = "", MainLauncher = false, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    [IntentFilter(new string[] { "android.intent.action.SEARCH" })]
    [MetaData(("android.app.searchable"), Resource = "@xml/searchable")]
    public class SearchStock : AppCompatActivity
    {
        TextView textView;
        ListView listView;

        public string ReceivedLampId;
        public string ReceivedLampLabel;

        private const string Tag = "SearchStock";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ReceivedLampId = Intent.GetStringExtra("selectedLampId") ?? "0";
            ReceivedLampLabel = Intent.GetStringExtra("selectedLampLabel") ?? "NoLabel";

            SetContentView(Resource.Layout.SearchStock);

            FindViews();

            textView.Visibility = ViewStates.Invisible;

            SetupToolbar();

            HandleIntent(Intent);
        }

        private void FindViews()
        {
            textView = this.FindViewById<TextView>(Resource.Id.text);
            listView = this.FindViewById<ListView>(Resource.Id.list);
        }

        #region SetupToolbarFunction

        private void SetupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "";

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayShowCustomEnabled(false);
        }

        #endregion

        void HandleIntent(Intent intent)
        {
            if (Intent.ActionView.Equals(intent.Action))
            {
                var data = intent.Data;
                var selectedStockInfo = GetSelectedStock(data);
                var groupText = SettingsService.LifxMonitoring ? "lamp" : "group";

                StockInfoDataService stockInfoDataService = new StockInfoDataService();
                if (stockInfoDataService.IsStockInfoAlreadyAddedToLamp(ReceivedLampId, selectedStockInfo.Symbol))
                {
                    Toast.MakeText(this, string.Format("Stock: {0} and {1}: {2}. already added.", selectedStockInfo.Symbol, groupText, ReceivedLampLabel), ToastLength.Short).Show();
                    return;
                }

                SaveSelectedStockInfo(selectedStockInfo);

                Toast.MakeText(this, string.Format("Created monitoring on stock: {0} {1}: {2}. Hold on, fetching data!", selectedStockInfo.Symbol, groupText, ReceivedLampLabel), ToastLength.Short).Show();

                Intent mainIntent = new Intent(this, typeof(MainMonitor));
                mainIntent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(mainIntent);
                Finish();
            }
            else if (Intent.ActionSearch.Equals(intent.Action))
            {
                string query = intent.GetStringExtra(SearchManager.Query);
                textView.Visibility = ViewStates.Visible;
                ShowResults(query);
            }
        }

        private SelectedStockInfo GetSelectedStock(Android.Net.Uri uri)
        {
            var cursor = ManagedQuery(uri, null, null, null, null);

            cursor.MoveToFirst();

            int wIndex = cursor.GetColumnIndexOrThrow(DictionaryDatabase.KEY_WORD);
            int dIndex = cursor.GetColumnIndexOrThrow(DictionaryDatabase.KEY_DEFINITION);

            return new SelectedStockInfo { Symbol = cursor.GetString(wIndex), CompanyName = cursor.GetString(dIndex) };
        }

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            // Because this activity has set launchMode="singleTop", the system calls this method
            // to deliver the intent if this actvity is currently the foreground activity when
            // invoked again (when the user executes a search from this activity, we don't create
            // a new instance of this activity, so the system delivers the search intent here)
            HandleIntent(intent);
        }

        void ShowResults(string query)
        {
            var cursor = ManagedQuery(DictionaryProvider.CONTENT_URI, null, null, new string[] { query }, null);

            if (cursor == null)
            {
                // There are no results             
                textView.Text = GetString(Resource.String.no_results, query);
            }
            else
            {

                int count = cursor.Count;
                var countString = Resources.GetQuantityString(Resource.Plurals.search_results, count, count, query);
                textView.Text = countString;

                string[] from = new string[] { DictionaryDatabase.KEY_WORD,
                    DictionaryDatabase.KEY_DEFINITION };

                int[] to = new int[] { Resource.Id.word,
                                  Resource.Id.definition };

                var words = new SimpleCursorAdapter(this, Resource.Layout.result, cursor, from, to);

                listView.Adapter = words;

                listView.ItemClick += (sender, e) =>
                {
                    var data = Android.Net.Uri.WithAppendedPath(DictionaryProvider.CONTENT_URI, Java.Lang.String.ValueOf(e.Id));
                    var selectedStockInfo = GetSelectedStock(data);
                    var groupText = SettingsService.LifxMonitoring ? "lamp" : "group";

                    StockInfoDataService stockInfoDataService = new StockInfoDataService();
                    if (stockInfoDataService.IsStockInfoAlreadyAddedToLamp(ReceivedLampId, selectedStockInfo.Symbol))
                    {
                        Toast.MakeText(this, string.Format("Stock: {0} and {1}: {2}. already added.", selectedStockInfo.Symbol, groupText, ReceivedLampLabel), ToastLength.Short).Show();
                        return;
                    }

                    SaveSelectedStockInfo(selectedStockInfo);

                    Toast.MakeText(this, string.Format("Created monitoring on stock: {0} {1}: {2}. Hold on, fetching data!", selectedStockInfo.Symbol, groupText, ReceivedLampLabel), ToastLength.Short).Show();

                    var intent = new Intent(ApplicationContext, typeof(MainMonitor));
                    intent.AddFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                    Finish();
                };

            }
        }

        private void SaveSelectedStockInfo(SelectedStockInfo selectedStockInfo)
        {
            StockInfoDataService stockInfoDataService = new StockInfoDataService();
            stockInfoDataService.SaveStockInfo(
                new Core.Model.StockInfo
                {
                    CompanyName = selectedStockInfo.CompanyName,
                    LampId = ReceivedLampId,
                    LampLabel = ReceivedLampLabel,
                    Symbol = selectedStockInfo.Symbol,
                    Active = true,
                });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.search_menu, menu);

            IMenuItem item = menu.FindItem(Resource.Id.searchViewControl);

            var searchView = MenuItemCompat.GetActionView(item);
            var actualSearchView = searchView.JavaCast<Android.Support.V7.Widget.SearchView>();

            var searchManager = (SearchManager)GetSystemService(Context.SearchService);
            actualSearchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            actualSearchView.SetIconifiedByDefault(false);
            actualSearchView.MaxWidth = Int32.MaxValue;

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.searchViewControl:
                    OnSearchRequested();
                    return true;
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    return true;
                default:
                    return false;
            }
        }
    }

    public class SelectedStockInfo
    {
        public string Symbol { get; set; }

        public string CompanyName { get; set; }
    }
}