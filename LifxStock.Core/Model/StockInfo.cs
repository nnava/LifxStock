using SQLite;

namespace LifxStock.Core.Model
{
    public class StockInfo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Symbol { get; set; }

        public string CompanyName { get; set; }

        public double Price { get; set; }

        public string ProcentChangeWithPrice { get; set; }

        public double ProcentChange { get; set; }

        public string LampId { get; set; }

        public string LampLabel { get; set; }

        public bool Active { get; set; }

        public string Country { get; set; }
    }
}