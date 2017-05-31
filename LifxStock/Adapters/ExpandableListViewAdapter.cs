using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Hanks.Htextview;
using LifxStock.Core;
using LifxStock.Core.Model;
using LifxStock.Core.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LifxStock.Adapters
{
    public class ExpandableListViewAdapter : BaseExpandableListAdapter
    {
        private Context context;
        private List<string> listGroupLamps;
        private Dictionary<string, ObservableCollection<StockInfo>> listStocks;
        private bool startFlag = true;

        public ExpandableListViewAdapter(Context context, List<string> listGroupLamps, Dictionary<string, ObservableCollection<StockInfo>> listStocks)
        {
            this.context = context;
            this.listGroupLamps = listGroupLamps;
            this.listStocks = listStocks;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }

        public void SetListValues(List<string> listGroup, Dictionary<string, ObservableCollection<StockInfo>> listChild)
        {
            this.listGroupLamps = listGroup;
            this.listStocks = listChild;
        }

        public override int GroupCount
        {
            get
            {
                return listGroupLamps.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public StockInfo GetStockInfo(int groupPosition, int childPosition)
        {
            var result = new ObservableCollection<StockInfo>();
            var groupList = listGroupLamps[groupPosition];
            listStocks.TryGetValue(groupList, out result);
            return result[childPosition];
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            var result = new ObservableCollection<StockInfo>();
            listStocks.TryGetValue(listGroupLamps[groupPosition], out result);
            return "";
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new ObservableCollection<StockInfo>();
            listStocks.TryGetValue(listGroupLamps[groupPosition], out result);

            if (result == null || result.Count == 0) return 0;

            return result.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.Item_layout, null);
            }

            var item = GetStockInfo(groupPosition, childPosition);

            var textColor = GetColorFromProcentChange(item.ProcentChange);

            convertView.FindViewById<TextView>(Resource.Id.stockSymbolTextView).Text = item.Symbol;
            convertView.FindViewById<TextView>(Resource.Id.stockNameTextView).Text = item.CompanyName;

            var hProcentChangeTextView = (HTextView)convertView.FindViewById(Resource.Id.procentChangeTextView);
            
            if (item.ProcentChangeWithPrice != null && HasStringValueChanged(item.ProcentChangeWithPrice.ToString(), hProcentChangeTextView))
            {
                hProcentChangeTextView.SetTextColor(textColor);
                hProcentChangeTextView.SetAnimateType(HTextViewType.Evaporate);
                hProcentChangeTextView.AnimateText(item.ProcentChangeWithPrice.ToString());
            }                

            var hPriceTextView = (HTextView)convertView.FindViewById(Resource.Id.priceTextView);

            if (HasStringValueChanged(item.Price.ToString("0.00"), hPriceTextView))
            {
                hPriceTextView.SetAnimateType(HTextViewType.Evaporate);
                hPriceTextView.AnimateText(item.Price.ToString("0.00"));
            }                

            var countryImage = GetCountryImage(item.Country);
            if (countryImage != 0)
                convertView.FindViewById<ImageView>(Resource.Id.countryImageView).SetImageResource(countryImage);

            return convertView;
        }

        private static bool HasStringValueChanged(string value, HTextView hTextView)
        {
            return hTextView.Text != value;
        }

        private int GetCountryImage(string country)
        {
            if (country == "USA")
                return Resource.Drawable.usa;
            else if (country == "SWEDEN")
                return Resource.Drawable.sweden;
            else if (country == "CANADA")
                return Resource.Drawable.canada;
            else if (country == "DENMARK")
                return Resource.Drawable.denmark;
            else if (country == "NORWAY")
                return Resource.Drawable.norway;
            else if (country == "GERMANY")
                return Resource.Drawable.germany;
            else if (country == "FINLAND")
                return Resource.Drawable.finland;

            return 0;
        }

        private static Color GetColorFromProcentChange(double procentChange)
        {
            if (procentChange > 0)
                return Color.DarkGreen;
            else if (procentChange == 0)
                return Color.Black;

            return Color.Red;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return listGroupLamps[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.Group_item, null);
            }

            var lamp = (string)GetGroup(groupPosition);

            var builder = new SpannableStringBuilder();

            var avgValue = GetGroupAvgValueForStocks(lamp);
            var minusOrPlus = avgValue > 0 ? "+" : "";
            var procentChangeText = " (" + minusOrPlus + avgValue.ToString("N") + "%)";

            var color = "#EE0000"; // Red 
            if (avgValue == 0)
                color = "#000000"; // Black
            else if(avgValue > 0)
                color = "#006622"; // Green

            var lampStatusText = string.Empty;
            if (SettingsService.LifxMonitoring && !startFlag && lamp != "Stocks")
            {
                var lampStatus = StaticLampStatusList.lampStatusList.Where(e => e.Name == lamp).FirstOrDefault();

                if (lampStatus == null || (lampStatus != null && !lampStatus.Online))
                    lampStatusText = "<font color ='#EE0000'> - LAMP OFFLINE</font>";
            }

            var groupText = "<font color='" + color + "'>" + lamp + procentChangeText + "</font>" + lampStatusText;
            convertView.FindViewById<TextView>(Resource.Id.groupText).SetText(Html.FromHtml(groupText), TextView.BufferType.Spannable);

            startFlag = false;
            return convertView; 
        }

        private double GetGroupAvgValueForStocks(string group)
        {
            if (listStocks == null || listStocks.Count == 0) return 0;

            var observableStocks = new ObservableCollection<StockInfo>();
            listStocks.TryGetValue(group, out observableStocks);
            if (observableStocks == null || observableStocks.Count == 0) return 0;

            return listStocks[group].Average(e => e.ProcentChange);
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}