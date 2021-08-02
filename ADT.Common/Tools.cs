using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ADT.Common
{
    public class Tools
    {
        /// <summary>
        /// 将多个实体组合成为一个 字典类型
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public static Dictionary<string, object> EntityToDictionary(Dictionary<string, object> di)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            foreach (var item in di)
            {
                Console.WriteLine(item.Value.GetType());
                Console.WriteLine(item.Value.GetType().Namespace);
                if (item.Value.GetType().Namespace == "ADT.Models")
                {
                    ReflexHelper.GetPropertyInfos(item.Value.GetType()).ToList().ForEach(pi =>
                    {
                        if (pi.GetValue(item.Value, null) == null)
                            r.Add(pi.Name, null);
                        else
                        {
                            if (pi.PropertyType == typeof(DateTime))
                                r.Add(pi.Name, pi.GetValue(item.Value, null).ToDateTimeFormat("yyyy-MM-dd HH:mm:ss"));
                            else
                                r.Add(pi.Name, pi.GetValue(item.Value, null));
                        }
                    });
                }
                else
                {
                    r.Add(item.Key, item.Value);
                }
            }
            return r;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="inputValue">需要加密的字符串</param>
        /// <returns></returns>
        public static string MD5Encryption(string inputValue)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
        /// <summary>
        /// 获取真实IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp(HttpContext httpContext)
        {
            string ip = "";
            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                ip = httpContext.Request.Headers["X-Forwarded-For"].ToString();

            if (string.IsNullOrEmpty(ip) && httpContext.Request.Headers.ContainsKey("X-Real-IP"))
                ip = httpContext.Request.Headers["X-Real-IP"].ToString();

            if (string.IsNullOrEmpty(ip))
                ip = httpContext.Connection.RemoteIpAddress.ToString();

            return ip;
        }
        /// <summary>
        /// 获取真实IP组（已|隔开）
        /// </summary>
        /// <returns></returns>
        public static string GetIps(HttpContext httpContext)
        {
            string ip = "";

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                ip += "|" + httpContext.Request.Headers["X-Forwarded-For"].ToString();

            if (string.IsNullOrEmpty(ip) && httpContext.Request.Headers.ContainsKey("X-Real-IP"))
                ip += "|" + httpContext.Request.Headers["X-Real-IP"].ToString();

            if (string.IsNullOrEmpty(ip))
                ip += "|" + httpContext.Connection.RemoteIpAddress.ToString();

            return ip;
        }
    }
}
