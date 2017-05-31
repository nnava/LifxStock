using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using LifxStock.Core.Model;
using System.Collections.Generic;

namespace LifxStock.Adapters
{
    public class LampListAdapter : BaseAdapter<Lamp>
    {
        List<Lamp> items;
        AppCompatActivity context;

        public LampListAdapter (AppCompatActivity context, List<Lamp> items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Lamp this[int position]
        {
            get
            {
                return items[position];
            }
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            if (convertView == null)
            {
                convertView = context.LayoutInflater.Inflate(Resource.Layout.LampRowView, null);
            }
            
            convertView.FindViewById<TextView>(Resource.Id.nameTextView).Text = item.Name;
                        
            return convertView;
        }

    }
}