using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ADT.Common
{
    /// <summary>
    /// 发送验证码
    /// </summary>
   public static class SMSHelper
    {
        public static string noticeVerifyCode = "【迈慧教育】注册报名验证码：{0}，仅用于注册,5分钟以内有效，请勿告知他人";


        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SendSmsVerfiyCode(string mobile, string msg)
        {
            //var ret = PostSmS("90630", "3b4584740d9653b3020b787580fb13e7", mobile, string.Format(noticeVerifyCode, msg));
            string url = "http://www.qunfaduanxin.vip/api/send";//地址  
            int  ret =NewPostSms(url, "15618204602", "younengkao", mobile,string.Format(noticeVerifyCode,msg));
            if (ret>0)
            {
                return true;
            }
            return false;
        }

        //还未解析解析后可以拿到token,目前已固定token
        public static string GetSMSToken()
        {
            string appId = "0116667768";
            string secret = "s4P5rWL0RN";
            string jsonDate = "";
            string url = "http://openapi.gateway.paasw.com/message/tokenApi";
            try
            {
                string postString = string.Format("appId={0}&secret={1}", appId, secret);
                byte[] postData = Encoding.UTF8.GetBytes(postString);//编码  
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
                byte[] responseData = webClient.UploadData(url, "POST", postData);
                jsonDate = Encoding.UTF8.GetString(responseData);//解码
            }
            catch { }
            return "";
        }

        public class SmsModel {

            /// <summary>
            /// token
            /// </summary>
            public string token { get; set; }
            
            /// <summary>
            ///短信类型（1验证码,2业务短信） 
            /// </summary>
            public string productId { get; set; }

            /// <summary>
            /// 手机号码
            /// </summary>
            public string mobile { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string msg { get; set; }

            /// <summary>
            /// 拓展码(数字)
            /// </summary>
            public string extNo { get; set; }

            /// <summary>
            /// 定时发送时间，格式为yyyy-MM-dd HH:mm:ss
            /// </summary>
            public string time { get; set; }
        }

        public class SmsResult {
            /// <summary>
            /// 返回编码
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// 消息提示
            /// </summary>
            public string message { get; set; }

        }

        public class SmsResult2
        {
            public long success { get; set; } = 0;

            public string error { get; set; }
        }

        /// <summary>
        /// 发送业务消息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="mobile"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static SmsResult HDPostSmS(string token,string mobile,string msg)
        {
            SmsResult result = new SmsResult();
            try
            {
                SmsModel model = new SmsModel()
                {
                    extNo = "888",
                    mobile = mobile,
                    token = token,
                    productId = "2",
                    msg = msg,
                    time = ""
                };
                string postString = JsonConvert.SerializeObject(model);
                byte[] postData = Encoding.UTF8.GetBytes(postString);//编码  
                string url = "http://openapi.gateway.paasw.com/message/send";//地址  
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/json");
                byte[] responseData = webClient.UploadData(url, "POST", postData);
                result = JsonConvert.DeserializeObject< SmsResult>(Encoding.UTF8.GetString(responseData));//解码
            }
            catch{ }
            return result;
        }



        public static int NewPostSms(string url,string uid,string pwd,string mobile,string msg) {
            int result = 0;
            try
            {
                string postString = string.Format("username={0}&password={1}&gwid=lmzvxm2h&mobile={2}&message={3}", uid, Tools.MD5Encryption(pwd).ToUpper(), mobile, msg);
                byte[] postData = Encoding.UTF8.GetBytes(postString);//编码  
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
                byte[] responseData = webClient.UploadData(url, "POST", postData);
                string srcString = Encoding.UTF8.GetString(responseData);//解码
                if (!string.IsNullOrEmpty(srcString)&& srcString.Contains("success"))
                {
                    result = 1;
                }
            }
            catch{ 
            }
            return result;
        }

        public static string ToMD5(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            byte[] b = new MD5CryptoServiceProvider().ComputeHash(str.ToBytes());
            string ret = string.Empty;
            for (int i = 0; i < b.Length; i++) { ret += b[i].ToString("x").PadLeft(2, '0'); }
            return ret;
        }
    }
}
