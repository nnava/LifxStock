using LifxStock.Core.Model;
using LifxStock.Core.Repository;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LifxStock.Core.Service
{
    public class StockInfoDataService
    {
        private static StockInfoRepository stockInfoRepository = new StockInfoRepository();

        public async Task<ObservableCollection<StockInfo>> GetAllStockInfos()
        {
            var stockInfoList = GetAllStockInfo();

            var yahooStockEngineService = new YahooStockEngineService();
            var yahooStockInfoList = await yahooStockEngineService.GetYahooStockInfoFromAPI(stockInfoList.Select(e => e.Symbol).ToArray());

            if (yahooStockInfoList == null) return null;

            var observableCollectionStockInfo = new ObservableCollection<StockInfo>();
            foreach (var stockInfo in stockInfoList.OrderBy(e => e.LampLabel).ThenBy(e => e.Symbol))
            {
                var yahooStockInfo = yahooStockInfoList.Where(e => e.Symbol == stockInfo.Symbol).FirstOrDefault();
                if (yahooStockInfo == null) continue;

                stockInfo.Price = yahooStockInfo.CurrentPrice;
                stockInfo.ProcentChange = yahooStockInfo.ProcentChange;
                stockInfo.ProcentChangeWithPrice = yahooStockInfo.ProcentChangeWithPrice;
                stockInfo.Country = yahooStockInfo.StockExchangeCountry;

                observableCollectionStockInfo.Add(stockInfo);
            }

            return observableCollectionStockInfo;
        }

        public int SaveStockInfo(StockInfo stockInfo)
        {
            return stockInfoRepository.Save(stockInfo);
        }

        public void DeleteStockInfo(StockInfo stockInfo)
        {
            stockInfoRepository.Delete(stockInfo);
        }

        public void DeleteAllStockInfo()
        {
            stockInfoRepository.DeleteAllStockInfo();
        }

        public IList<StockInfo> GetAllStockInfo()
        {
            return stockInfoRepository.GetAllStockInfos();
        }

        public StockInfo GetStockInfo(int id)
        {
            return stockInfoRepository.GetStockInfo(id);
        }

        public bool IsStockInfoAlreadyAddedToLamp(string lampId, string symbol)
        {
            return stockInfoRepository.GetAllStockInfos().Any(e => e.LampId == lampId && e.Symbol == symbol);
        }
    }
}
