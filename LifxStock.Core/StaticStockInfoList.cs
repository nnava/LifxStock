using LifxStock.Core.Model;
using System.Collections.ObjectModel;

namespace LifxStock.Core
{
    public static class StaticStockInfoList
    {
        private static object lockStockInfoList = new object();
        private static ObservableCollection<StockInfo> _stockInfoList;

        public static ObservableCollection<StockInfo> stockInfoList
        {
            get
            {
                if (_stockInfoList == null)
                    _stockInfoList = new ObservableCollection<StockInfo>();

                return _stockInfoList;
            }
            
            set
            {
                lock(lockStockInfoList)
                {
                    _stockInfoList = value;
                }
            }
        }
    }
}