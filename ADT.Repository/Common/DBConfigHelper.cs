using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ADT.Repository
{
    /// <summary>
    /// 获取appsettings.json配置信息
    /// </summary>
    public class DBConfigHelper
    {
        private static readonly object objLock = new object();
        /// <summary>
        /// 初始实列
        /// </summary>
        private static DBConfigHelper instance = null;
        /// <summary>
        /// 环境变量
        /// </summary>
        public static string Evn = null;

        private IConfigurationRoot Config { get; }
        /// <summary>
        /// 构造函数
        /// </summary>
        private DBConfigHelper()
        {
            //获取环境变量
            Evn = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //根据环境变量设置配置文件
            var appsettings = Evn == "Production" || string.IsNullOrEmpty(Evn) ? "appsettings.json" : $"appsettings.Development.json";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appsettings, optional: false, reloadOnChange: true);
            Config = builder.Build();
        }

        public static DBConfigHelper GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new DBConfigHelper();
                    }
                }
            }

            return instance;
        }
        /// <summary>
        /// 获取配置信息（多级目录以xxx:xxx;xxx来定位）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConfig(string name)
        {
            return GetInstance().Config.GetSection(name).Value;
        }
        /// <summary>
        /// 获取读写分离读配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<SlaveConnectionConfig> GetSlaveConnectionStrings(string name)
        {
            return GetInstance().Config.GetSection(name).GetChildren().ToList().Select(x => new SlaveConnectionConfig()
            {
                HitRate = x.GetValue<int>("HitRate"),
                ConnectionString = x.GetValue<string>("ConnectionString")
            }).ToList();
        }
    }
}
