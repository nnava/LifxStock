using LifxStock.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LifxStock.Core.Data
{
    public class StockInfoDatabase
    {
        SQLiteConnection database;
        static object locker = new object();

        public StockInfoDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "stocksInfo.db");

            database = new SQLiteConnection(dbPath, false);

            database.CreateTable<StockInfo>();
        }

        public int SaveStockInfo(StockInfo stockInfo)
        {
            lock (locker)
            {
                if (stockInfo.ID != 0)
                {
                    database.Update(stockInfo);
                    return stockInfo.ID;
                }
                else
                {
                    return database.Insert(stockInfo);
                }
            }
        }

        public IList<StockInfo> GetAllStockInfos()
        {
            lock (locker)
            {
                return (from c in database.Table<StockInfo>()
                        select c).ToList();
            }
        }

        public StockInfo GetStockInfo(int id)
        {
            lock (locker)
            {
                return database.Table<StockInfo>().Where(c => c.ID == id).FirstOrDefault();
            }
        }

        public int DeleteStockInfoRow(StockInfo objectToDelete)
        {
            lock (locker)
            {
                var map = database.GetMapping(objectToDelete.GetType());
                var pk = map.PK;
                if (pk == null)
                {
                    throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
                }
                var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
                return database.Execute(q, pk.GetValue(objectToDelete));
            }
        }

        public int DeleteAllStockInfo()
        {
            lock (locker)
            {
                return database.DeleteAll<StockInfo>();
            }
        }
    }
}

