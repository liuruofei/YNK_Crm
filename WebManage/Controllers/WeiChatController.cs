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
using ADT.Models.Enum;

namespace WebManage.Controllers
{
    public class WeiChatController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private ICurrencyService _currencyService;
        private const string token = "younengkao";
        private WXSetting _wxConfig;
        private IMemoryCache _cache;
        private RedisConfig redisConfig;
        private WXAcceSSToken wxtokenModel = new WXAcceSSToken();
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeiChatController));
        public WeiChatController(ICurrencyService currencyService, IOptions<WXSetting> wxConfig, IOptions<RedisConfig> _redisConfig, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _currencyService = currencyService;
            _wxConfig = wxConfig.Value;
            //_cache = memoryCache;
            redisConfig = _redisConfig.Value;
            if (RedisLock.KeyExists("wxAccessToken", redisConfig.RedisCon))
            {
                wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", redisConfig.RedisCon);
            }
            else
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, redisConfig.RedisCon);
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
            if (RedisLock.KeyExists("wxAccessToken", redisConfig.RedisCon))
            {
                wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", redisConfig.RedisCon);
            }
            else
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, redisConfig.RedisCon);
            }
            return Json(wxtokenModel.Access_Token);
        }

        public string GetWXToken()
        {
            WXAcceSSToken wxtokenModel = new WXAcceSSToken(); ;
            if (RedisLock.KeyExists("wxAccessToken", redisConfig.RedisCon))
            {
                wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", redisConfig.RedisCon);
            }
            else
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, redisConfig.RedisCon);
            }
            return wxtokenModel.Access_Token;
        }


        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="jsonMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMenu() {
            log.Info("菜单创建路径" + _hostingEnvironment.ContentRootPath);
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
                log.Info("创建菜单异常" + er.Message);
            }
            return Content(response);
        }


        /// <summary>
        /// 获取所有关注的openId
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QueryOpenList() {
            var api = string.Format("https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid=", wxtokenModel.Access_Token);
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
            var api = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&grant_type=authorization_code&code={2}", _wxConfig.AppId, _wxConfig.AppSecret, code);
            string opentJson = await HttpHelper.HttpGetAsync(api);
            return Json(opentJson);
        }

        /// <summary>
        /// 检查是否已绑定openId,同时返回课程列表
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckOpenId(string openId, string dateStr) {
            ResResult rsg = new ResResult() { code = 0, msg = "未绑定" };
            //查询是否老师
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(v => v.OpenId == openId).First();
            C_Contrac_User student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v => v.OpenId == openId || v.Elder_OpenId == openId || v.Elder2_OpenId == openId).First();
            List<CourseWorkModel> list = new List<CourseWorkModel>();
            if (teacher != null && !string.IsNullOrEmpty(dateStr)) {
                list = _currencyService.DbAccess().Queryable(@"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName,cl.Class_Name from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.TeacherUid=@TeacherUid and wk.AT_Date=CAST(@startStr AS date))", "orginSql")
               .AddParameters(new { startStr = dateStr, TeacherUid = teacher.User_ID })
                .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToList();
                list.ForEach(it => { it.StudentOpenId = "";it.TeacherOpenId = teacher.OpenId; });
            }
            if (student != null && !string.IsNullOrEmpty(dateStr))
            {
                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                List<int> classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((ch, cl) => new object[] { JoinType.Left, ch.ClassId == cl.ClassId }).Where(ch => ch.StudentUid == student.StudentUid && ch.ClassId > 0&&ch.Contrac_Child_Status != contracStatus).Select(ch => ch.ClassId).ToList();
                list = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room>((cour, ta, room) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id })
                    .Where(cour => (cour.StudentUid == student.StudentUid || classIds.Contains(cour.ClasssId)) && cour.StudyMode != 3 && DateTime.Parse(cour.AT_Date.ToString("yyyy-MM-dd") + " 00:00") == DateTime.Parse(dateStr + " 00:00"))
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
                   list.ForEach(it => { it.StudentOpenId =student.OpenId; it.TeacherOpenId = "";});
            }
            if (student != null || teacher != null) {
                rsg.code = 200;
                rsg.data = list;
                rsg.msg = "已绑定";
                return Json(rsg);
            }
            return Json(rsg);
        }


        public async Task<IActionResult> GetCourseDayArr(string openId, string dtMonth) {
            ResResult ru = new ResResult() { code = 0, msg = "未获取数据" };
            string wehredtMonth = DateTime.Parse(dtMonth).ToString("yyyy-MM");
            List<C_Course_Work> list = new List<C_Course_Work>();
            sys_user tacher = _currencyService.DbAccess().Queryable<sys_user>().Where(ta => ta.OpenId == openId).First();
            C_Contrac_User student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(st => st.OpenId == openId || st.Elder_OpenId == openId || st.Elder2_OpenId == openId).First();
            if (tacher != null) {
                 
                list = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(n => n.AT_Date.ToString("yyyy-MM") == wehredtMonth && n.StudyMode != 3)
                .WhereIF(tacher != null && !string.IsNullOrEmpty(tacher.OpenId), n => n.TeacherUid == tacher.User_ID).ToList();
            }
            if (student != null)
            {
                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                List<int> classIds = new List<int>();
                List<C_Contrac_Child> listchild = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(te => te.StudentUid == student.StudentUid && te.StudyMode == 2&&te.Contrac_Child_Status!= contracStatus).ToList();
                if (listchild != null && listchild.Count > 0) {
                    listchild.ForEach(it => {
                        classIds.Add(it.ClassId);
                    });
                }
                list = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(n => n.AT_Date.ToString("yyyy-MM") == wehredtMonth && n.StudyMode != 3)
               .WhereIF(student != null, n => n.StudentUid == student.StudentUid|| classIds.Contains(n.ClasssId)).ToList();
            }
            if (list != null && list.Count > 0)
            {
                List<string> listDay = new List<string>();
                list.ForEach(it =>
                {
                    listDay.Add(it.AT_Date.ToString("yyyy-M-d"));
                });
                ru.data = listDay;
                ru.code = 200;
                ru.msg = "获取成功";
            }
            return Json(ru);
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
                C_Contrac_User student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(v => v.Student_Name == userName||v.Elder_Name==userName || v.Elder2_Name == userName).First();
                if (teacher != null&&!string.IsNullOrEmpty(openId)) {
                    if (!string.IsNullOrEmpty(teacher.OpenId))
                    {
                        rsg.msg = "该名称已被绑定,无法重复绑定";
                        return Json(rsg);
                    }
                    teacher.OpenId = openId;
                    result= _currencyService.DbAccess().Updateable<sys_user>(teacher).ExecuteCommand();
                }
                if (student != null && !string.IsNullOrEmpty(openId)) {

                    if (!string.IsNullOrEmpty(student.OpenId)&&userName==student.Student_Name) {
                        rsg.msg = "该名称已被绑定,无法重复绑定";
                        return Json(rsg);
                    }
                    //绑定学生
                    if (student.Student_Name == userName) {
                        result = _currencyService.DbAccess().Updateable<C_Contrac_User>().SetColumns(item => new C_Contrac_User { OpenId = openId }).Where(item => item.StudentUid == student.StudentUid).ExecuteCommand();
                    }
                    //绑定学生家长
                    if (student.Elder_Name == userName)
                    {
                        result = _currencyService.DbAccess().Updateable<C_Contrac_User>().SetColumns(item => new C_Contrac_User { Elder_OpenId = openId }).Where(item => item.StudentUid == student.StudentUid).ExecuteCommand();
                    }
                    //绑定学生家长2
                    if (student.Elder2_Name == userName)
                    {
                        result = _currencyService.DbAccess().Updateable<C_Contrac_User>().SetColumns(item => new C_Contrac_User { Elder2_OpenId = openId }).Where(item => item.StudentUid == student.StudentUid).ExecuteCommand();
                    }
                }
                if (student == null && teacher == null) {
                    rsg.msg = "系统用户不存在,绑定失败";
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
        [HttpPost]
        public async Task<IActionResult> GetWorkModel(int wkId) {
            ResResult rsg = new ResResult() { code = 0, msg = "获取失败" };
            var model = _currencyService.DbAccess().Queryable<C_Course_Work,C_Contrac_User>((it,u)=>new object[]{JoinType.Left,it.StudentUid==u.StudentUid }).Where((it,u)=> it.Id == wkId).Select<CourseWorkModel>((it,u)=>new CourseWorkModel { 
             Id=it.Id,
             Work_Title=it.Work_Title,
             AT_Date=it.AT_Date,
             StartTime=it.StartTime,
             EndTime=it.EndTime,
             Contra_ChildNo=it.Contra_ChildNo,
             Comment=it.Comment,
             CourseWork=it.CourseWork,
             StudyMode=it.StudyMode,
             StudentUid=it.StudentUid,
             TeacherUid=it.TeacherUid,
             Work_Stutas=it.Work_Stutas,
             RangTimeId=it.RangTimeId,
             SubjectId=it.SubjectId,
             ProjectId=it.ProjectId,
             UnitId=it.UnitId,
             IsSendComment=it.IsSendComment,
             IsSendWork=it.IsSendWork,
             ListeningName=it.ListeningName,
             ClasssId=it.ClasssId,
             Score=it.Score,
             Comment_Time=it.Comment_Time,
             Student_Name=u.Student_Name,
             IsUsePresent=it.IsUsePresent,
             CourseTime=it.CourseTime,
             CreateTime=it.CreateTime,
             CreateUid=it.CreateUid
            }).First();
            if (model != null) {
                rsg.data = model;
                rsg.code = 200;
                rsg.msg = "获取成功";
            }
            return Json(rsg);
        }


        public async Task<IActionResult> QueryUnit(int projectId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Project_Unit> list = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(it => it.ProjectId == projectId).ToList();
            rsg.data = list;
            return Json(rsg);
        }


        /// <summary>
        /// 保存作业
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveWork(string openId, int wkId, string homework)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "保存作业成功" };
            try
            {
                var model = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => it.Id ==wkId).First();
                if (model.IsSendWork == 1)
                {
                    rsg.code = 0;
                    rsg.msg = "课程作业已被推送！不能再推送";
                }
                else if (string.IsNullOrEmpty(homework))
                {
                    rsg.code = 0;
                    rsg.msg = "作业内容为空！不能保存";
                }
                else
                {
                    var toaken = GetWXToken();
                    //1对1
                    if (model.StudentUid > 0)
                    {
                        var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(u => u.StudentUid == model.StudentUid).First();
                        if (!string.IsNullOrEmpty(student.OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            //SendMsgHomeWork(student.OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            SendMsgHomeWork2(student.OpenId, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"),homework, student.Student_Name, model.Id);
                        }
                        if (!string.IsNullOrEmpty(student.Elder_OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            //SendMsgHomeWork(student.Elder_OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            SendMsgHomeWork2(student.Elder_OpenId, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"),homework, student.Student_Name, model.Id);
                        }
                        if (!string.IsNullOrEmpty(student.Elder2_OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            //SendMsgHomeWork(student.Elder2_OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            SendMsgHomeWork2(student.Elder_OpenId, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"),homework, student.Student_Name, model.Id);
                        }
                    }
                    //班课
                    if (model.ClasssId > 0)
                    {
                        List<C_Contrac_User> ovtStudent = new List<C_Contrac_User>();
                        List<C_Contrac_User> ovtElder = new List<C_Contrac_User>();
                        List<C_Contrac_User> ovtElder2 = new List<C_Contrac_User>();
                        List<string> studentOpenIds = new List<string>();
                        List<string> elderOpenIds = new List<string>();
                        List<string> elderOpenIds2 = new List<string>();
                        int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                        List<C_Contrac_User> listchild = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Contrac_User>((contrac, u) => new object[] { JoinType.Left, contrac.StudentUid == u.StudentUid }).Where(contrac => contrac.ClassId == model.ClasssId && contrac.Contrac_Child_Status != contracStatus).Select((contrac, u) => u).ToList();
                        listchild.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(iv.OpenId) && !studentOpenIds.Contains(iv.OpenId))
                            {
                                studentOpenIds.Add(iv.OpenId);
                                ovtStudent.Add(new C_Contrac_User { OpenId = iv.OpenId, Student_Name = iv.Student_Name });
                            }
                            if (!string.IsNullOrEmpty(iv.Elder_OpenId) && !elderOpenIds.Contains(iv.Elder_OpenId))
                            {
                                elderOpenIds.Add(iv.Elder_OpenId);
                                ovtElder.Add(new C_Contrac_User { Elder_OpenId = iv.Elder_OpenId, Student_Name = iv.Student_Name });
                            }
                            if (!string.IsNullOrEmpty(iv.Elder2_OpenId) && !elderOpenIds2.Contains(iv.Elder2_OpenId))
                            {
                                elderOpenIds2.Add(iv.Elder2_OpenId);
                                ovtElder2.Add(new C_Contrac_User { Elder2_OpenId = iv.Elder2_OpenId, Student_Name = iv.Student_Name });
                            }
                        });
                        //推送给学生
                        studentOpenIds.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                //SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                                var stuName = ovtStudent.Find(cc => cc.OpenId == iv).Student_Name;
                                SendMsgHomeWork2(iv, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"),homework, stuName, model.Id);
                            }
                        });
                        //推送给家长1
                        elderOpenIds.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                //SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                                var stuName = ovtElder.Find(cc => cc.Elder_OpenId == iv).Student_Name;
                                SendMsgHomeWork2(iv, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"),homework, stuName, model.Id);
                            }
                        });
                        //推送给家长2
                        elderOpenIds2.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                //SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                                var stuName = ovtElder2.Find(cc => cc.Elder2_OpenId == iv).Student_Name;
                                SendMsgHomeWork2(iv, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"),homework, stuName, model.Id);
                            }
                        });
                    }
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(it => new C_Course_Work { CourseWork =homework, IsSendWork = 1 }).Where(it => it.Id == wkId).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存作业成功";
                    }
                }
            }
            catch (Exception er)
            {
                rsg.code = 0;
                rsg.msg = er.Message;
                log.Info("课程作业推送失败，原因" + er.Message);
            }
            return Json(rsg);
        }


        /// <summary>
        /// 保存点评
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="wkId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveComment(string openId,int wkId,string comment,int unitId) {
            ResResult rsg = new ResResult() { code = 0, msg = "保存点评失败" };
            try {
                if (wkId<1)
                {
                    rsg.code = 0;
                    rsg.msg = "缺少参数,无法点评";
                    return Json(rsg);
                }
                if(string.IsNullOrEmpty(openId))
                {
                    rsg.code = 0;
                    rsg.msg = "你当前还未绑定Crm系统，无法点评";
                    return Json(rsg);
                }
                if (string.IsNullOrEmpty(comment))
                {
                    rsg.code = 0;
                    rsg.msg = "点评内容不能为空";
                    return Json(rsg);
                }
                var model = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => it.Id == wkId).First();
                var user = _currencyService.DbAccess().Queryable<sys_user>().Where(it => it.OpenId == openId).First();
                var courseTime = DateTime.Parse(model.AT_Date.ToString("yyyy-MM-dd") + " " + model.EndTime);
                if (user.User_ID!=model.TeacherUid)
                {
                    rsg.code = 0;
                    rsg.msg = "你不属于当前课程老师,无法点评";
                }
                else if (model.IsSendComment==1)
                {
                    rsg.code = 0;
                    rsg.msg = "你的点评已被推送,无法再修改";
                }
                else if (DateTime.Now < courseTime)
                {
                    rsg.code = 0;
                    rsg.msg = "当前课程时间还未结束，你无法点评";
                }
                else
                {
                    var valiteTime = DateTime.Parse(model.AT_Date.ToString("yyyy-MM-dd") + " " + model.EndTime);
                    //如果点评在23小时之内,则课时有效
                    if (model.StudyMode != 5 && model.StudyMode != 6)
                    {
                        if (model.Work_Stutas != 1)
                        {
                            if (valiteTime.AddHours(24) > DateTime.Now)
                            {
                                model.Work_Stutas = 1;
                            }
                            else
                            {
                                model.Work_Stutas = 0;
                            }
                        }
                        else {
                            if (string.IsNullOrEmpty(comment)) {
                                model.Work_Stutas = 0;
                            }
                        }
                    }
                    else {
                        if (!string.IsNullOrEmpty(comment) && model.Work_Stutas!=1) {
                            model.Work_Stutas = 1;
                        }
                    }
                    if (unitId> 0 && model.UnitId != unitId)
                    {
                        model.UnitId = unitId;
                        var unitM = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(y => y.UnitId == unitId).First();
                        if (model.Work_Title.Split("_").Length == 4)
                        {
                            model.Work_Title = model.Work_Title.Substring(0, model.Work_Title.LastIndexOf("_"));
                        }
                        model.Work_Title = model.Work_Title + "_" + unitM.UnitName;
                    }
                    model.Comment = comment;
                    model.Comment_Time = DateTime.Now;
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>(model).Where(it => it.Id == wkId).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.data = model;
                        rsg.code = 200;
                        rsg.msg = "点评成功";
                    }
                }

            }
            catch (Exception er)
            {
                rsg.msg ="保存点评失败，失败原因"+er.Message;
                log.Info("保存点评失败，失败原因" + er.Message);
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


        //发送家庭作业模板2
        public void SendMsgHomeWork2(string openId, string templateId, string wxaccessToken, string msg, string wkTitle, string wkTeacher, string wkTime, string courseWork, string studentName, int wkId)
        {
            Dictionary<string, object> jsonObject = new Dictionary<string, object>();
            jsonObject.Add("touser", openId);   // openid
            jsonObject.Add("template_id", templateId);
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, string> first = new Dictionary<string, string>();
            first.Add("value", msg);
            first.Add("color", "#173177");
            Dictionary<string, string> keyword1 = new Dictionary<string, string>();
            keyword1.Add("value", studentName);
            keyword1.Add("color", "#173177");
            Dictionary<string, string> keyword2 = new Dictionary<string, string>();
            keyword2.Add("value", wkTitle);
            keyword2.Add("color", "#173177");
            Dictionary<string, string> keyword3 = new Dictionary<string, string>();
            keyword3.Add("value", courseWork);
            keyword3.Add("color", "#173177");
            Dictionary<string, string> remark = new Dictionary<string, string>();
            remark.Add("value", "课程作业:" + courseWork);
            remark.Add("color", "#173177");
            data.Add("first", first);
            data.Add("keyword1", keyword1);
            data.Add("keyword2", keyword2);
            data.Add("keyword3", keyword3);
            data.Add("remark", remark);
            jsonObject.Add("data", data);
            jsonObject.Add("url", "http://crm.younengkao.com/WxTemplateChild/Zuoye?wkId=" + wkId);//设置链接
            var jsonStr = JsonConvert.SerializeObject(jsonObject);
            var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + wxaccessToken;
            string content = HttpHelper.HttpPost(api, jsonStr, "application/json");
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
