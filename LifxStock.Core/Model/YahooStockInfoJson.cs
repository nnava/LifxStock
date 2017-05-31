namespace LifxStock.Core.Model
{
    public class Rootobject
    {
        public Search search { get; set; }
        public object[] users { get; set; }
        public Markettime marketTime { get; set; }
        public Quotesummary quoteSummary { get; set; }
        public object[] marketSummary { get; set; }
        public Darla darla { get; set; }
        public Desktopuh desktopUH { get; set; }
        public object[] streamAds { get; set; }
        public Follow follow { get; set; }
        public long initialNow { get; set; }
        public Streamnews streamNews { get; set; }
        public Historicalprices historicalPrices { get; set; }
        public Ui ui { get; set; }
        public Navrail navrail { get; set; }
        public Pf pf { get; set; }
        public Quotes quotes { get; set; }
        public int mailcount { get; set; }
        public object[] editorialNews { get; set; }
        public Slingstone slingstone { get; set; }
        public object[] sidekick { get; set; }
        public Spark spark { get; set; }
    }

    public class Search
    {
    }

    public class Markettime
    {
    }

    public class Quotesummary
    {
        public JNJ JNJ { get; set; }
    }

    public class JNJ
    {
        public Quotetype quoteType { get; set; }
        public Summarydetail summaryDetail { get; set; }
        public Price price { get; set; }
    }

    public class Quotetype
    {
        public string exchange { get; set; }
        public string shortName { get; set; }
        public string longName { get; set; }
        public object underlyingSymbol { get; set; }
        public string quoteType { get; set; }
        public string symbol { get; set; }
        public object underlyingExchangeSymbol { get; set; }
        public object headSymbol { get; set; }
        public string messageBoardId { get; set; }
        public string uuid { get; set; }
        public string market { get; set; }
    }

    public class Summarydetail
    {
        public float previousClose { get; set; }
        public float regularMarketOpen { get; set; }
        public float twoHundredDayAverage { get; set; }
        public float trailingAnnualDividendYield { get; set; }
        public float payoutRatio { get; set; }
        public float regularMarketDayHigh { get; set; }
        public int averageDailyVolume10Day { get; set; }
        public float regularMarketPreviousClose { get; set; }
        public float fiftyDayAverage { get; set; }
        public float trailingAnnualDividendRate { get; set; }
        public float open { get; set; }
        public int averageVolume10days { get; set; }
        public float dividendRate { get; set; }
        public int exDividendDate { get; set; }
        public float beta { get; set; }
        public float regularMarketDayLow { get; set; }
        public float trailingPE { get; set; }
        public int regularMarketVolume { get; set; }
        public long marketCap { get; set; }
        public int averageVolume { get; set; }
        public float priceToSalesTrailing12Months { get; set; }
        public float dayLow { get; set; }
        public float ask { get; set; }
        public int askSize { get; set; }
        public int volume { get; set; }
        public float fiftyTwoWeekHigh { get; set; }
        public float forwardPE { get; set; }
        public int maxAge { get; set; }
        public float fiveYearAvgDividendYield { get; set; }
        public float fiftyTwoWeekLow { get; set; }
        public float bid { get; set; }
        public float dividendYield { get; set; }
        public int bidSize { get; set; }
        public float dayHigh { get; set; }
    }

    public class Price
    {
        public float regularMarketOpen { get; set; }
        public int averageDailyVolume3Month { get; set; }
        public string exchange { get; set; }
        public int regularMarketTime { get; set; }
        public float regularMarketDayHigh { get; set; }
        public string shortName { get; set; }
        public int averageDailyVolume10Day { get; set; }
        public string longName { get; set; }
        public float regularMarketChange { get; set; }
        public string currencySymbol { get; set; }
        public float regularMarketPreviousClose { get; set; }
        public int postMarketTime { get; set; }
        public int postMarketChange { get; set; }
        public float postMarketPrice { get; set; }
        public string exchangeName { get; set; }
        public float regularMarketDayLow { get; set; }
        public string currency { get; set; }
        public float regularMarketPrice { get; set; }
        public int regularMarketVolume { get; set; }
        public string regularMarketSource { get; set; }
        public string marketState { get; set; }
        public object underlyingSymbol { get; set; }
        public string quoteType { get; set; }
        public string postMarketSource { get; set; }
        public string symbol { get; set; }
        public int postMarketChangePercent { get; set; }
        public string preMarketSource { get; set; }
        public int maxAge { get; set; }
        public float regularMarketChangePercent { get; set; }
    }

    public class Darla
    {
    }

    public class Desktopuh
    {
    }

    public class Follow
    {
    }

    public class Streamnews
    {
    }

    public class Historicalprices
    {
    }

    public class Ui
    {
        public Qsp qsp { get; set; }
        public int pageProgress { get; set; }
    }

    public class Qsp
    {
        public bool main { get; set; }
    }

    public class Navrail
    {
    }

    public class Pf
    {
        public bool pfNamesLoaded { get; set; }
        public object[] order { get; set; }
        public Portfolios portfolios { get; set; }
        public Views views { get; set; }
        public object[] viewOrder { get; set; }
        public bool viewsLoaded { get; set; }
        public Recs recs { get; set; }
    }

    public class Portfolios
    {
    }

    public class Views
    {
    }

    public class Recs
    {
    }

    public class Quotes
    {
    }

    public class Slingstone
    {
    }

    public class Spark
    {
    }

}