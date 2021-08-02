using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ADT.Common
{
    /// <summary>
    /// 获取appsettings.json配置信息
    /// </summary>
    public class ConfigHelper
    {
        private static readonly object objLock = new object();
        private static ConfigHelper instance = null;

        private IConfigurationRoot Config { get; }

        /// <summary>
        /// 构造函数（）
        /// </summary>
        private ConfigHelper()
        {
            //获取环境变量
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //根据环境变量设置配置文件
            var appsettings = env == "Production" || string.IsNullOrEmpty(env) ? "appsettings.json" : $"appsettings.Development.json";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appsettings, optional: false, reloadOnChange: true);
            Config = builder.Build();
        }

        public static ConfigHelper GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new ConfigHelper();
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
    }
}
