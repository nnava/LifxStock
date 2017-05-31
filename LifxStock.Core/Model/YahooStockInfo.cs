namespace LifxStock.Core.Model
{
    public class YahooStockInfo
    {
        public string Symbol { get; set; }

        public string LampId { get; set; }

        public string ProcentChangeWithPrice { get; set; }

        public double ProcentChange { get; set; }

        public double CurrentPrice { get; set; }

        public string StockExchangeCountry { get; set; }
    }
}