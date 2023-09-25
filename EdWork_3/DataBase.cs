using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using NewVariant.Exceptions;
using NewVariant.Interfaces;
using NewVariant.Models;


    public class DataBase : IDataBase
    {
        private Dictionary<Type, List<IEntity>> dk = new Dictionary<Type, List<IEntity>>();
        public DataBase() { }
    /// <summary>
    /// Метод создает новую таблицу типа Т.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="DataBaseException"> Исключение, выбрасываемое если уже таблица сущесвует.</exception>

    public void CreateTable<T>() where T : IEntity
        {
        try
        {
            if (dk.ContainsKey(typeof(T)))
            {
                throw new DataBaseException();
            }
            else
            {
                dk.Add(typeof(T), new List<IEntity>());
            }

        }
        catch
        {

            throw new DataBaseException();
        }

     }
    /// <summary>
    ///  Метод возвращает ссылку на таблицу типа Т. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="DataBaseException"> Исключение, выбрасываемое если таблица уже существует. </exception>

    public IEnumerable<T> GetTable<T>() where T : IEntity
        {    
            if (!dk.ContainsKey(typeof(T)))
            {
                throw new DataBaseException();
            }
            if (typeof(T) == typeof(Buyer))
            {
                List<Buyer> bBuyers = new List<Buyer>();
                foreach (var b in dk[typeof(T)])
                {
                    bBuyers.Add((Buyer)b);
                }
                return (IEnumerable<T>)bBuyers;
            }
            if (typeof(T) == typeof(Shop))
            {
                List<Shop> sShops = new List<Shop>();
                foreach (var b in dk[typeof(T)])
                {
                    sShops.Add((Shop)b);
                }
                return (IEnumerable<T>)sShops;
            }
            if (typeof(T) == typeof(Sale))
            {
                List<Sale> sSales = new List<Sale>();
                foreach (var b in dk[typeof(T)])
                {
                    sSales.Add((Sale)b);
                }
                return (IEnumerable<T>)sSales;
            }
            if (typeof(T) == typeof(Good))
            {
                List<Good> gGoods = new List<Good>();
                foreach (var b in dk[typeof(T)])
                {
                    gGoods.Add((Good)b);
                }
                return (IEnumerable<T>)gGoods;
            }

            throw new DataBaseException();
        }
    /// <summary>
    /// Метод добавляет новую строку в таблицу типа Т.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="getEntity">Возвращает результат соответсвующего типа.</param>
    /// <exception cref="DataBaseException"></exception>

    public void InsertInto<T>(Func<T> getEntity) where T : IEntity
        {
        try
        {
            if (!dk.ContainsKey(typeof(T)))
            {
                throw new DataBaseException();
            }
            dk[typeof(T)].Add(getEntity.Invoke());

        }
        catch
        {
            throw new DataBaseException();
        }        
     }
    /// <summary>
    /// Метод сериализует таблицу типа Т.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">Путь файла.</param>
    /// <exception cref="DataBaseException"></exception>
    public void Serialize<T>(string path) where T : IEntity
     {
        try
        { 
            if (!dk.ContainsKey(typeof(T)))
            {
                throw new DataBaseException();
            }
            else
            {
                List<T> newList = new List<T>();
                if (dk[typeof(T)] != null)
                {
                    foreach (IEntity obj in dk[typeof(T)])
                    {
                        newList.Add((T)obj);
                    }
                }
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    JsonSerializer.Serialize(fs,newList);
                }
            }

        }
        catch 
        {

            throw new DataBaseException();
        }         
     }
    /// <summary>
    /// Метод десериализует и сохраняет таблицу типа Т из файла.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">Путь файла.</param>
    public void Deserialize<T>(string path) where T : IEntity
    {
        try
        { 
            if (File.Exists(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                List<T> inputList = JsonSerializer.Deserialize(fs, typeof(List<T>)) as List<T>;
                List<IEntity> newList = new List<IEntity>();
                if (inputList.Count > 0)
                {
                    foreach (T obj in inputList)
                    { 
                        newList.Add(obj);
                    }
                    if (dk.ContainsKey(typeof(T)))
                    {
                        dk[typeof(T)] = newList;
                    }
                    else
                    {
                        dk.Add(typeof(T), newList);
                    }
                }
            }
        }

        }
        catch
        {
            throw new DataBaseException();
        }      
    }
}
       

