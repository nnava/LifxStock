using LifxStock.Core.Model;
using LifxStock.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using YSQ.core.Quotes;

namespace LifxStock.Core.Service
{
    public class YahooStockEngineService
    {
        private const string Tag = "YahooStockEngineService";
        private static readonly string Canada = "CANADA";
        private static readonly string USA = "USA";
        private static readonly string Sweden = "SWEDEN";
        private static readonly string Norway = "NORWAY";
        private static readonly string Finland = "FINLAND";
        private static readonly string Germany = "GERMANY";
        private static readonly string Denmark = "DENMARK";

        public async Task<List<YahooStockInfo>> GetYahooStockInfoFromAPI(string[] symbols)
        {

            try
            {
                var listYahooStockInfo = new List<YahooStockInfo>();

                var quote_service = new QuoteService();

                var quotes = await Task.Run(() => quote_service.Quote(symbols).Return(QuoteReturnParameter.Symbol,
                                                        QuoteReturnParameter.StockExchange,
                                                        QuoteReturnParameter.ChangeAsPercent,
                                                        QuoteReturnParameter.ChangeAndPercentChange,
                                                        QuoteReturnParameter.LatestTradePrice));

                var culture = new CultureInfo("en-US");
                
                foreach (var quote in quotes)
                {
                    var stockExchangeCountryId = GetStockExchangeCountryId(quote.StockExchange, quote.Symbol);

                    double currentPrice;
                    bool boolcurrentPrice = double.TryParse(quote.LatestTradePrice.Replace(",", "."), NumberStyles.Any, culture, out currentPrice);

                    double changeAsPercent;
                    bool boolchangeAsPercent = double.TryParse(quote.ChangeAsPercent.Replace("%", "").Replace(",", "."), NumberStyles.Any, culture, out changeAsPercent);

                    var procentChangeWithPriceDynamic = (string)quote.ChangeAndPercentChange;
                    var procentChangeWithPrice = procentChangeWithPriceDynamic.GetFormattedChangeText(changeAsPercent);

                    listYahooStockInfo.Add(new YahooStockInfo { Symbol = quote.Symbol, CurrentPrice = currentPrice, ProcentChange = changeAsPercent, ProcentChangeWithPrice = procentChangeWithPrice, StockExchangeCountry = stockExchangeCountryId });
                }

                return listYahooStockInfo;
            }
            catch 
            {
                return null;
            }
        }

        private string GetStockExchangeCountryId(string stockExchangeNodeText, string symbol)
        {
            if (stockExchangeNodeText.Equals("NYQ") || stockExchangeNodeText.Equals("NMS"))
                return USA;
            else if (stockExchangeNodeText.Equals("STO"))
                return Sweden;
            else if (stockExchangeNodeText.Equals("TOR"))
                return Canada;
            else if (stockExchangeNodeText.Equals("CPH"))
                return Denmark;
            else if (stockExchangeNodeText.Equals("OSL"))
                return Norway;
            else if (stockExchangeNodeText.Equals("FRA"))
                return Germany;
            else if (stockExchangeNodeText.Equals("HEL"))
                return Finland;

            if (stockExchangeNodeText.Equals("N/A"))
            {
                if (symbol.EndsWith(".TO"))
                    return Canada;
                else if (symbol.EndsWith(".ST"))
                    return Sweden;
                else if (symbol.EndsWith(".CO"))
                    return Denmark;
                else if (symbol.EndsWith(".F"))
                    return Germany;
                else if (symbol.EndsWith(".HE"))
                    return Finland;
                else if (symbol.EndsWith(".OL"))
                    return Norway;
                else
                    return USA;
            }

            return string.Empty;
        }

        private async Task<string> HttpGetAsync(string URI)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19");
                    using (HttpResponseMessage response = await client.GetAsync(URI))
                    using (HttpContent content = response.Content)
                    {
                        return await content.ReadAsStringAsync();
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}