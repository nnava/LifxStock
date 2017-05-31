using LifxStock.Core.Model;
using System.Collections.ObjectModel;

namespace LifxStock.Core
{
    public static class StaticLampStatusList
    {
        private static object lockInfoList = new object();
        private static ObservableCollection<Lamp> _lampStatusList;

        public static ObservableCollection<Lamp> lampStatusList
        {
            get
            {
                if (_lampStatusList == null)
                    _lampStatusList = new ObservableCollection<Lamp>();

                return _lampStatusList;
            }
            
            set
            {
                lock(lockInfoList)
                {
                    _lampStatusList = value;
                }
            }
        }
    }
}