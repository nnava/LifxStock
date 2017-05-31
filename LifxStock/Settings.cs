using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using LifxStock.Core.Service;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LifxStock
{
    [Activity(Label = "Settings", WindowSoftInputMode = SoftInput.StateVisible)]
    public class Settings : AppCompatActivity
    {
        #region Properties

        private ToggleButton lifxSupportToggleButton;
        private EditText lifxTokenEditText;
        private Button verifyLifxTokenButton;
        private Button deleteAllStockDataButton;
        private Button lifxTokenHowToLinkButton;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            FindViews();

            SetupToolbar();

            HandleEvents();

            lifxTokenEditText.SetTextIsSelectable(true);

            LoadSettings();
        }

        #region EventFunctions

        private void HandleEvents()
        {
            lifxSupportToggleButton.Click += LifxSupportToggleButton_Click;
            verifyLifxTokenButton.Click += VerifyLifxTokenButton_Click;
            deleteAllStockDataButton.Click += DeleteAllStockDataButton_Click;
            lifxTokenHowToLinkButton.Click += LifxTokenHowToLinkButton_Click;
        }

        private void LifxTokenHowToLinkButton_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://community.lifx.com/t/creating-a-lifx-http-api-token/25");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void DeleteAllStockDataButton_Click(object sender, EventArgs e)
        {
            var confirmDeleteDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            confirmDeleteDialog.SetMessage("This will delete all added stocks for monitoring, are you sure?");
            confirmDeleteDialog.SetPositiveButton("OK", (s, eventarg) => {
                var stockInfoDataService = new StockInfoDataService();
                stockInfoDataService.DeleteAllStockInfo();
                Toast.MakeText(this, "All added stocks deleted", ToastLength.Short).Show();
            });
            confirmDeleteDialog.SetNegativeButton("Cancel", (s, eventarg) => { /* do nothing */ });
            confirmDeleteDialog.Create().Show();
        }

        private async void VerifyLifxTokenButton_Click(object sender, System.EventArgs e)
        {
            var akavacheSettingsHelper = new AkavacheSettingsHelper();
            var token = await akavacheSettingsHelper.GetLifxToken();
            if (string.IsNullOrEmpty(token))
                token = lifxTokenEditText.Text;

            var lifxLampService = new LifxLampService(token);

            try
            {
                await lifxLampService.SetAvailabeLampsToList();

                var lampListCount = lifxLampService.Lamps.Count;

                if (lampListCount == 0)
                    Toast.MakeText(this, "Not found any lamps, correct token or no lamps connected?", ToastLength.Long).Show();
                else
                    Toast.MakeText(this, $"Yay! Found {lampListCount} lamps. Token seems correct.", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Failed to verify token, try again", ToastLength.Short).Show();
            }
        }

        private void LifxSupportToggleButton_Click(object sender, System.EventArgs e)
        {
            SettingsService.LifxMonitoring = lifxSupportToggleButton.Checked;

            var togglebuttonToastText = string.Format("LIFX Monitoring {0} ", lifxSupportToggleButton.Checked ? "enabled" : "disabled");
            Toast.MakeText(this, togglebuttonToastText, ToastLength.Short).Show();
        }

        #endregion

        #region SetupFunctions

        private void SetupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "Settings";

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        #endregion

        private void FindViews()
        {
            lifxSupportToggleButton = FindViewById<ToggleButton>(Resource.Id.lifxSupportToggleButton);
            lifxTokenEditText = FindViewById<EditText>(Resource.Id.lifxTokenEditText);
            verifyLifxTokenButton = FindViewById<Button>(Resource.Id.verifyLifxTokenButton);
            deleteAllStockDataButton = FindViewById<Button>(Resource.Id.deleteAllStockDataButton);
            lifxTokenHowToLinkButton = FindViewById<Button>(Resource.Id.lifxTokenHowToLinkButton);
        }

        private string GetClipboardDataToPaste()
        {
            var clipboard = (ClipboardManager)GetSystemService(ClipboardService);

            if (!(clipboard.HasPrimaryClip) ||
                !(clipboard.PrimaryClipDescription.HasMimeType(ClipDescription.MimetypeTextPlain)))
                return string.Empty;
            else
                return clipboard.PrimaryClip.GetItemAt(0).Text;            
        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                SaveSettings();

                if(IsTokenEditTextValid())
                    Toast.MakeText(this, "Settings saved", ToastLength.Short).Show();

                Finish();
            }
                
            return base.OnOptionsItemSelected(item);
        }

        private async void LoadSettings()
        {
            lifxSupportToggleButton.Checked = SettingsService.LifxMonitoring;

            var akavacheSettingsHelper = new AkavacheSettingsHelper();
            var lifxToken = await akavacheSettingsHelper.GetLifxToken();
            if (lifxToken.Length > 60)
                lifxTokenEditText.Text = "Token already set, verify?";
        }

        private async void SaveSettings()
        {
            if (IsTokenEditTextValid())
            {
                var akavacheSettingsHelper = new AkavacheSettingsHelper();
                await akavacheSettingsHelper.SaveLifxToken(lifxTokenEditText.Text);
            }
        }

        private bool IsTokenEditTextValid()
        {
            return !string.IsNullOrEmpty(lifxTokenEditText.Text) && lifxTokenEditText.Text.Length > 60;
        }        
    }
}