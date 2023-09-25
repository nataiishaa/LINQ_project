using NewVariant.Interfaces;
using NewVariant.Models;
using NewVariant.Exceptions;
using System;
using System.Security.Cryptography;

namespace DataLib
{
    public class DataAccessLayer : IDataAccessLayer
    {
        public DataAccessLayer() { }
        /// <summary>
        /// Метод возвращает список всех товаров, купленных покупателем с самым
        /// длинным именем.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public IEnumerable<Good> GetAllGoodsOfLongestNameBuyer(IDataBase dataBase)
        {
            var result = from good in dataBase.GetTable<Good>()
                         where ( from sale in dataBase.GetTable<Sale>()
                         where sale.BuyerId == ( from  buyers in dataBase.GetTable<Buyer>()
                                                 orderby buyers.Name.Length,buyers.Name
                                                 select buyers.Id).LastOrDefault()
                                 select sale.GoodId).Contains(good.Id)
                                 select good;
            return result;
        }
        /// <summary>
        /// Метод возвращает название категории самого дорого товара.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public string GetMostExpensiveGoodCategory(IDataBase dataBase)
        {
            var goods = dataBase.GetTable<Good>()
                .OrderByDescending(g => g.Price)
                .ToList();
            if (goods.Count == 0)
            {
                return null;
            }
            return goods
                .First()
                .Category;
        }
        /// <summary>
        /// Метод получает название города, в котором было потрачено меньше всего денег.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public string GetMinimumSalesCity(IDataBase dataBase)
        {
            var shops = dataBase.GetTable<Shop>().OrderBy(s => s.City).ToList();
            List<Sale> sales = dataBase.GetTable<Sale>().ToList();
            Dictionary<string, long> cities = new Dictionary<string, long>();
            foreach (var shop in shops)
            {
                long sum = sales
                    .Where(sale => shop.Id == sale.ShopId)
                    .Sum(sale => sale.GoodCount);
                cities[shop.City] = sum;
            }

            if (cities.Count == 0)
            {
                return null;
            }
            return cities
                .OrderBy(x => x.Value)
                .First()
                .Key;
        }
        /// <summary>
        /// Метод возвращает список покупателей, которые купили самый популярный товар.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public IEnumerable<Buyer> GetMostPopularGoodBuyers(IDataBase dataBase)
        {
            var goodsb = new Dictionary<Good, long>();
            foreach (var good in dataBase.GetTable<Good>())
            {
                long count = dataBase.GetTable<Sale>()
                    .Where(sale => sale.GoodId == good.Id)
                    .Sum(sale => sale.GoodCount);
                goodsb[good] = count;
            }
            var gooods = goodsb.OrderByDescending(g => g.Value);
            if (gooods.Count() == 0)
            {
                return new List<Buyer>();
            }
            var g = gooods
                .First()
                .Key;
            var buyersbb = (from sale in dataBase.GetTable<Sale>()
                            where sale.GoodId == g.Id
                            from b in dataBase.GetTable<Buyer>()
                            where sale.BuyerId == b.Id
                            select b).ToList();
            return buyersbb;
        }
        /// <summary>
        /// Для каждой страны метод определяет количество ее магазинов. 
        /// Возвращает наименьшее из полученных значений.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public int GetMinimumNumberOfShopsInCountry(IDataBase dataBase)
        {
            var answer = (from s in (from s in dataBase.GetTable<Shop>()
                                     group s by s.Country)
                          orderby s.Count()
                          select s.Count());
            if (dataBase.GetTable<Shop>().Count() == 0)
            {
                return 0;
            }
            return answer.FirstOrDefault();
        }
        /// <summary>
        ///  Метод возвращает список покупок, совершенных покупателями во всех городах, кроме города проживания. 
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public IEnumerable<Sale> GetOtherCitySales(IDataBase dataBase)
        {
            var sales = dataBase.GetTable<Sale>();
            var shops = dataBase.GetTable<Shop>();
            var buyers = dataBase.GetTable<Buyer>();
            var answer = (from sale in sales
                          join buy in buyers on sale.BuyerId equals buy.Id
                          join shop in shops on sale.ShopId equals shop.Id
                          where shop.City != buy.City
                          select sale);
            return answer;
        }
        /// <summary>
        ///  Метод возвращает общую стоимость покупок.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public long GetTotalSalesValue(IDataBase dataBase)
        {
            var sales = dataBase.GetTable<Sale>();
            var goods = dataBase.GetTable<Good>();
            return (from sale in sales
                    from good in goods
                    where sale.GoodId == good.Id
                    select good.Price * sale.GoodCount)
                .Sum();
        }
    }
}
