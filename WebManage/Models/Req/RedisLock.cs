using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using WebManage.Models.Res;

namespace WebManage
{
    public static class RedisLock
    {
        // private string con = "127.0.0.1:6379,password=199010,defaultDatabase=0";

        /// <summary>
        /// 分布式加锁
        /// </summary>
        /// <param name="key"></param>
        public static void Lock(string key,string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                while (true)
                {
                    var flag = database.LockTake(key, Thread.CurrentThread.ManagedThreadId, TimeSpan.FromSeconds(10));
                    if (flag)
                    {
                        break;
                    }
                    Thread.Sleep(200);
                }
            }
        }
        /// <summary>
        /// 分布式解锁
        /// </summary>
        /// <param name="key"></param>
        public static bool UnLock(string key, string con)
        {
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(con);
            try
            {
                IDatabase database = conn.GetDatabase();
                return database.LockRelease(key, Thread.CurrentThread.ManagedThreadId);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        /// <summary>
        /// 减量
        /// </summary>
        /// <param name="key"></param>
        public static long Decrement(string key,string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.StringDecrement(key);
            }
        }

        /// <summary>
        /// 增量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long Inc(string key, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.StringIncrement(key);
            }
        }

        /// <summary>
        /// 判断key值是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyExists(string key, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.KeyExists(key);
            }
        }

        /// <summary>
        /// 设置字符串值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="senconds">毫秒</param>
        /// <returns></returns>
        public static bool StringSetKey(string key, string value, double senconds, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.StringSet(key, value, TimeSpan.FromSeconds(senconds));
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string stringGetKey(string key, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.StringGet(key);
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="queKey"></param>
        /// <param name="queMessage"></param>
        /// <returns></returns>
        public static long EnQueue(string queKey, string queValue, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                //byte[] msg = System.Text.UTF8Encoding.UTF8.GetBytes(queValue);
                IDatabase database = conn.GetDatabase();
                return database.ListLeftPush(queKey, queValue);
            }
        }
        /// <summary>
        /// 出队
        /// </summary>
        /// <param name="queKey"></param>
        /// <returns></returns>
        public static string DeQueue(string queKey, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                string redisValue = "数据已都出队";
                IDatabase database = conn.GetDatabase();
                if (database.ListRange(queKey).Length > 0)
                {
                    redisValue = database.ListRightPop(queKey);
                    if (!string.IsNullOrEmpty(redisValue))
                        return redisValue + "已经出队";
                    else
                        return string.Empty;

                }
                return redisValue;
            }

        }
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topticName"></param>
        /// <param name="handler"></param>
        public static void SubScriper(string topticName, string con, Action<RedisChannel, RedisValue> handler = null)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {

                ISubscriber subscriber = conn.GetSubscriber();

                ChannelMessageQueue channelMessageQueue = subscriber.Subscribe(topticName);
                //消息接受
                channelMessageQueue.OnMessage(channelMessage =>
                {
                    if (handler != null)
                    {
                        string redisChannel = channelMessage.Channel;
                        string msg = channelMessage.Message;
                        handler.Invoke(redisChannel, msg);
                    }
                    else
                    {
                        string msg = channelMessage.Message;
                        Console.WriteLine($"订阅到消息: { msg},Channel={channelMessage.Channel}");
                    }
                });
            }

        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topticName"></param>
        /// <param name="message"></param>
        public static void PublishMessage(string topticName, string message, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con)) {
                ISubscriber subscriber = conn.GetSubscriber();
                long publishLong = subscriber.Publish(topticName, message);
                Console.WriteLine($"发布消息成功：{publishLong}");
            }
        }
        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetStringKey<T>(string key, string con)
         {
             try
             {
                using (var conn = ConnectionMultiplexer.Connect(con))
                {
                    IDatabase database = conn.GetDatabase();
                    return JsonConvert.DeserializeObject<T>(database.StringGet(key));
                }
   
             }
             catch (Exception ex)
             {
                 //return new T();
                 return default(T);
             }
         }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="obj"></param>
        public static bool SetStringKey<T>(string key, T obj ,double senconds, string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                string json = JsonConvert.SerializeObject(obj);
                return database.StringSet(key, json, TimeSpan.FromSeconds(senconds));
            }
        }



        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public static bool KeyDelete(string key, string con)
         {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.KeyDelete(key);
            }
         }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public static bool KeyRename(string key, string newKey, string con)
         {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.KeyRename(key, newKey);
            }
         }

        /// <summary>
        /// 删除hasekey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public static bool HaseDelete(RedisKey key, RedisValue hashField, string con)
         {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.HashDelete(key, hashField);
            }
         }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool HashRemove(string key, string dataKey, string con)
         {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.HashDelete(key, dataKey);
            }
         }


        /// <summary>
        /// 设置Hash值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="hashEntries"></param>
        /// <param name="con"></param>
        public static void HashSet(string hashKey,HashEntry[] hashEntries,string con) {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                database.HashSet(hashKey, hashEntries);
            }
        }


        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="hashEntries"></param>
        /// <param name="con"></param>
        public static HashEntry[] HashGet(string hashKey,string con)
        {
            using (var conn = ConnectionMultiplexer.Connect(con))
            {
                IDatabase database = conn.GetDatabase();
                return database.HashGetAll(hashKey);
            }
        }

        /// <summary>
        /// 把对象转HashEntry[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HashEntry[] TohashEntries(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties.TohashEntries();
        }


    }
}
