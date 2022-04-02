using ADT.Common;
using ADT.Service.IService;
using CrmApi.Models;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrmApi.Controllers
{
    public class WeiChatController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private ICurrencyService _currencyService;
        private const string token = "younengkao";
        private WXSetting _wxConfig;
        private IMemoryCache _cache;
        private WXAcceSSToken wxtokenModel = new WXAcceSSToken();
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeiChatController));
        public WeiChatController(ICurrencyService currencyService, IOptions<WXSetting> wxConfig, IMemoryCache memoryCache, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _currencyService = currencyService;
            _wxConfig = wxConfig.Value;
            _cache = memoryCache;
            var cache = _cache.Get("accessToken");
            if (cache == null)//如果没有该缓存
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                _cache.Set("accessToken", tokenCotent, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(wxtokenModel.Expires_in)));
            }
            else
            {
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(cache.ToString());
            }
        }


        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        /// <returns></returns>
        [HttpGet]
        public ActionResult WeChatCheck(string signature, string timestamp, string nonce, string echostr)
        {
            string[] ArrTmp = { token, timestamp, nonce };
            //字典排序
            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);
            //字符加密
            var sha1 = HmacSha1Sign(tmpStr);
            if (sha1.Equals(signature))
            {
                return Content(echostr);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取用户token
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryToken() {
            var cache = _cache.Get("accessToken");
            if (cache == null)//如果没有该缓存
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                _cache.Set("accessToken", tokenCotent, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(wxtokenModel.Expires_in)));
            }
            else
            {
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(cache.ToString());
            }
            return Content(wxtokenModel.Access_Token);
        }


        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="jsonMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMenu() {
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string menu = "";
            using (FileStream fs= new FileStream(webRootPath + "\\JsonConfig\\wxMenu.json", FileMode.Open)) {
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                menu = sr.ReadToEnd();
                sr.Close();
            }
            var api = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + wxtokenModel.Access_Token;
            var jsonStr = JsonConvert.SerializeObject(menu, Formatting.None);
            var response = await HttpHelper.HttpPostAsync(api, menu, "application/json");
            return Content(response);
        }


        /// <summary>
        /// 获取所有关注的openId
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QueryOpenList() {
            var api =string.Format("https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid=", wxtokenModel.Access_Token);
            string opentIdCotent = await HttpHelper.HttpGetAsync(api);
            return Content(opentIdCotent);
        }

        /// <summary>
        /// 通过前端网页请求发送的code获取openId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryOpenID(string code) {
          var api= string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&grant_type=authorization_code&code={2}", _wxConfig.AppId, _wxConfig.AppSecret,code);
          string opentId =await HttpHelper.HttpGetAsync(api);
            return Content(opentId);
        }
        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="postJson"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendTemplateMsg() {
 
            Dictionary<string,object> jsonObject = new Dictionary<string, object>();
            jsonObject.Add("touser", "oUiF66MdA5-prq9ORmmCzveI9pUk");   // openid
            jsonObject.Add("template_id", "SaMmNnE6AO9VZGWZqx2o6N43eMrIiTzUt2rL1AC7c7k");
            Dictionary<string, object> data = new  Dictionary<string, object>();
            Dictionary<string, string> first = new Dictionary<string, string>();
            first.Add("value", "hello");
            first.Add("color", "#173177");
            Dictionary<string, string> keyword1 = new Dictionary<string, string>();
            keyword1.Add("value", "hello");
            keyword1.Add("color", "#173177");
            Dictionary<string, string> keyword2 = new Dictionary<string, string>();
            keyword2.Add("value", "hello");
            keyword2.Add("color", "#173177");
            Dictionary<string, string> keyword3 = new Dictionary<string, string>();
            keyword3.Add("value", "hello");
            keyword3.Add("color", "#173177");
            Dictionary<string, string> remark = new Dictionary<string, string>();
            remark.Add("value", "hello");
            remark.Add("color", "#173177");

            data.Add("first", first);
            data.Add("keyword1", keyword1);
            data.Add("keyword2", keyword2);
            data.Add("keyword3", keyword3);
            data.Add("remark", remark);
            jsonObject.Add("data", data);
            var jsonStr = JsonConvert.SerializeObject(jsonObject, Formatting.None);
            var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token="+wxtokenModel.Access_Token;
            string content =HttpHelper.HttpPost(api, jsonStr, "application/json");
            return Content(content);
        }

        /// <summary>
        /// 获取tacket
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string getTicket(String accessToken)
        {
            // 网页授权接口
            var api = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + accessToken + "&type=jsapi";
            string ticket = "";
            try
            {
                var cache = _cache.Get("ticket");
                if (cache != null)
                {
                    var ticketJSONO = JsonConvert.DeserializeObject<WXTicketModel>(cache.ToString());
                    ticket = ticketJSONO.ticket;//获取ticket
                }
                else {
                    string response = HttpHelper.HttpGet(api);
                    var ticketJSONO = JsonConvert.DeserializeObject<WXTicketModel>(response);
                    ticket = ticketJSONO.ticket;//获取ticket
                    _cache.Set("ticket", ticket, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(ticketJSONO.expires_in)));
                }

            }
            catch (Exception e)
            {
               
            }
            return ticket;
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="url">页面url地址</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getSignature(string url)
        {
            log.Info("进入后台"+ url);
            SignatureModel singnaModel = new SignatureModel();
            //获取noncestr
            string noncestr = CreatenNonce_str();
            //获取timestamp
            string timestamp = GetTimeStamp();
            //获取jspai_ticket
            string jsapi_ticket = getTicket(wxtokenModel.Access_Token);
            //将四个数据进行组合，传给SHA1进行加密
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("jsapi_ticket=").Append(jsapi_ticket).Append("&")
              .Append("noncestr=").Append(noncestr).Append("&")
              .Append("timestamp=").Append(timestamp).Append("&")
              .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            //sha1加密
            singnaModel.signature = Sha1Sign(stringBuilder.ToString());
            singnaModel.timestamp = timestamp;
            singnaModel.noncestr = noncestr;
            log.Info("地址" + url+" , 签名:"+ singnaModel.signature);
            return Json(singnaModel);
        }




        /// <summary>
        /// HMAC-SHA1加密算法
        /// </summary>
        /// <param name="str">加密字符串</param>
        /// <returns></returns>
        public string HmacSha1Sign(string str)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.Default.GetBytes(str));
            string byte2String = null;
            for (int i = 0; i < hash.Length; i++)
            {
                byte2String += hash[i].ToString("x2");
            }
            return byte2String;
        }


        /// <summary>
        /// Sha1加密签名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Sha1Sign(string str)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = System.Text.UTF8Encoding.Default.GetBytes(str);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string signature = BitConverter.ToString(bytes_sha1_out);
            signature = signature.Replace("-", "").ToLower();
            return signature;
        }

        /// <summary>
        /// 获取微信JS-JDK时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);

            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// 第二种时间戳
        /// </summary>
        /// <returns></returns>
        public static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// JS-JDK 创建随机字符串
        /// </summary>
        /// <returns></returns>
        public static string CreatenNonce_str()
        {
            string[] strs = new string[]{
                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                  "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
            };
            Random r = new Random();
            var sb = new StringBuilder();
            var length = strs.Length;
            for (int i = 0; i < 15; i++)
            {
                sb.Append(strs[r.Next(length - 1)]);
            }
            return sb.ToString();
        }


        public IActionResult Index()
        {
            return View();
        }

    }
}
