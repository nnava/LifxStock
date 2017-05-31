using LifxStock.Core.Data;
using LifxStock.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LifxStock.Core.Repository
{
    public class StockInfoRepository
    {
        #region Properties

        private static List<StockInfo> stockInfoList = new List<StockInfo>();
        private StockInfoDatabase stockInfoDatabase;

        #endregion

        public StockInfoRepository()
        {
            stockInfoDatabase = new StockInfoDatabase();
        }
        
        public IList<StockInfo> GetAllStockInfos()
        {
            return stockInfoDatabase.GetAllStockInfos();
        }

        public int Save(StockInfo stockInfo)
        {
            return stockInfoDatabase.SaveStockInfo(stockInfo);
        }

        public int Delete(StockInfo stockInfo)
        {
            return stockInfoDatabase.DeleteStockInfoRow(stockInfo);
        }

        public void DeleteAllStockInfo()
        {
            stockInfoDatabase.DeleteAllStockInfo();
        }

        internal StockInfo GetStockInfo(int id)
        {
            return stockInfoDatabase.GetStockInfo(id);
        }
    }
}
