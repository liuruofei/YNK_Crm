using ADT.Common;
using ADT.Models.ResModel;
using ADT.Service.IService;
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
using WebManage.Models;
using ADT.Models;
using WebManage.Models.Res;
using SqlSugar;

namespace WebManage.Controllers
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
            return Json(wxtokenModel.Access_Token);
        }


        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="jsonMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMenu() {
            log.Info("菜单创建路径"+ _hostingEnvironment.ContentRootPath);
            var response = "";
            try
            {
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string menu = "";
                using (FileStream fs = new FileStream(webRootPath + "\\JsonConfig\\wxMenu.json", FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                    menu = sr.ReadToEnd();
                    sr.Close();
                }
                var api = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + wxtokenModel.Access_Token;
                var jsonStr = JsonConvert.SerializeObject(menu, Formatting.None);
                response = await HttpHelper.HttpPostAsync(api, menu, "application/json");
                log.Info("菜单列表" + menu);
                log.Info("菜单创建返回问题" + response);
            }
            catch (Exception er) {
                log.Info("创建菜单异常" +er.Message);
            }
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
        [HttpPost]
        public async Task<IActionResult> QueryOpenID(string code) {
          var api= string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&grant_type=authorization_code&code={2}", _wxConfig.AppId, _wxConfig.AppSecret,code);
          string opentJson =await HttpHelper.HttpGetAsync(api);
          log.Info("返回openId:"+opentJson);
          return Json(opentJson);
        }

        /// <summary>
        /// 检查是否已绑定openId,同时返回课程列表
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckOpenId(string openId,string dateStr) {
            ResResult rsg = new ResResult() { code = 0, msg = "未绑定" };
            //查询是否老师
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(v => v.OpenId == openId).First();
            C_Contrac_User student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v => v.OpenId == openId).First();
            List<CourseWorkModel> list = new List<CourseWorkModel>();
            if (teacher != null) {
                list = _currencyService.DbAccess().Queryable(@"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName,cl.Class_Name from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.TeacherUid=@TeacherUid and wk.AT_Date=CAST(@startStr AS date))", "orginSql")
               .AddParameters(new { startStr = dateStr, TeacherUid = teacher.User_ID })
                .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToList();
            }
            if (student != null)
            {
                List<int> classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((ch, cl) => new object[] { JoinType.Left, ch.ClassId == cl.ClassId }).Where(ch => ch.StudentUid == student.StudentUid && ch.ClassId > 0).Select(ch => ch.ClassId).ToList();
                list = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
                    .Where(cour => (cour.StudentUid == student.StudentUid || classIds.Contains(cour.ClasssId)) && cour.StudyMode != 3 && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00")== DateTime.Parse(dateStr+ " 00:00"))
                    .Select<CourseWorkModel>((cour, ta, room) => new CourseWorkModel
                    {
                        Id = cour.Id,
                        AT_Date = cour.AT_Date,
                        ClasssId = cour.ClasssId,
                        Comment = cour.Comment,
                        Contra_ChildNo = cour.Contra_ChildNo,
                        CourseTime = cour.CourseTime,
                        CreateTime = cour.CreateTime,
                        CreateUid = cour.CreateUid,
                        EndTime = cour.EndTime,
                        ProjectId = cour.ProjectId,
                        RangTimeId = cour.RangTimeId,
                        RoomId = cour.RoomId,
                        StartTime = cour.StartTime,
                        StudentUid = cour.StudentUid,
                        StudyMode = cour.StudyMode,
                        SubjectId = cour.SubjectId,
                        TA_Uid = cour.TA_Uid,
                        TeacherName = ta.User_Name,
                        TeacherUid = cour.TeacherUid,
                        Work_Stutas = cour.Work_Stutas,
                        Work_Title = cour.Work_Title,
                        Status = cour.Status,
                        UpdateTime = cour.UpdateTime,
                        UpdateUid = cour.UpdateUid,
                        RoomName = room.RoomName
                    }).ToList();
            }
            if (student != null || teacher != null) {
                rsg.code = 200;
                rsg.data = list;
                rsg.msg = "已绑定";
                return Json(rsg);
            }
            return Json(rsg);
        }

        /// <summary>
        /// 绑定学员或者老师openId
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveOpenID(string openId,string userName) {
            ResResult rsg = new ResResult() { code = 0, msg = "绑定失败" };
            try
            {
                int result = 0;
                //查询是否老师
                sys_user teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(v => v.User_Name == userName).First();
                C_Contrac_User student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v => v.Student_Name == userName).First();
                if (teacher != null&&!string.IsNullOrEmpty(openId)) {
                    teacher.OpenId = openId;
                    result= _currencyService.DbAccess().Updateable<sys_user>(teacher).ExecuteCommand();
                }
                if (student != null && !string.IsNullOrEmpty(openId)) {
                    result = _currencyService.DbAccess().Updateable<C_Contrac_User>().SetColumns(item => new C_Contrac_User { OpenId = openId }).Where(item=>item.StudentUid==student.StudentUid).ExecuteCommand();
                }
                if (result > 0) {
                    rsg.code = 200;
                    rsg.msg = "绑定成功";
                }

            }
            catch(Exception er) {
                log.Info("微信公众号绑定用户crm系统"+userName+"失败，失败原因"+er.Message);
            }
            return Json(rsg);
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
            jsonObject.Add("template_id",_wxConfig.TemplateId);
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
            singnaModel.appId = _wxConfig.AppId;
            return Json(singnaModel);
        }

        [HttpGet]
        public JsonResult getMenuAll() {
            var api = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token="+ wxtokenModel.Access_Token;
            string response = "";
            try
            {
                response = HttpHelper.HttpGet(api);

            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            return Json(response);
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
