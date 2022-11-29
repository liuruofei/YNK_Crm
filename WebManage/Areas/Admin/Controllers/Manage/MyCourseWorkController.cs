using ADT.Common;
using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class MyCourseWorkController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        private IC_CourseWorkService _courseWork;
        private RedisConfig _redsconfig;
        private WXSetting _wxConfig;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(MyCourseWorkController));
        public MyCourseWorkController(ICurrencyService currencyService, IC_ContracService contrac, IOptions<WXSetting> wxConfig, IOptions<RedisConfig> redisConfig, IC_CourseWorkService courseWork)
        {
            _currencyService = currencyService;
            _contrac = contrac;
            _courseWork = courseWork;
            _wxConfig = wxConfig.Value;
            _redsconfig = redisConfig.Value;
        }

        protected override void Init()
        {
            this.MenuID = "V-352";
        }

        [UsersRoleAuthFilter("V-352", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }


        [UsersRoleAuthFilter("V-352", FunctionEnum.Edit)]
        public IActionResult Add(int ID)
        {
            ViewBag.ID = ID;
            return View();
        }


        [UsersRoleAuthFilter("V-352", "Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 保存点评
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveCourseWork(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "保存点评成功" };
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var model = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => it.Id == vmodel.Id).First();
            var courseTime = DateTime.Parse(model.AT_Date.ToString("yyyy-MM-dd") + " " + model.EndTime);
            if (model.TeacherUid != userId)
            {
                rsg.code = 0;
                rsg.msg = "你不属于当前课程老师,无法点评";
            }
            else if (DateTime.Now < courseTime)
            {
                rsg.code = 0;
                rsg.msg = "当前课程时间还未结束，你无法点评";
            }
            else {
                vmodel.CreateUid = userId;
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
                        if (string.IsNullOrEmpty(vmodel.Comment))
                        {
                            model.Work_Stutas = 0;
                        }
                    }
                }
                else {
                    if (!string.IsNullOrEmpty(vmodel.Comment) && model.Work_Stutas != 1)
                    {
                        model.Work_Stutas = 1;
                    }
                }
                if (vmodel.UnitId > 0&&model.UnitId!=vmodel.UnitId) {
                    model.UnitId = vmodel.UnitId;
                    var unitM = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(y => y.UnitId == vmodel.UnitId).First();
                    if (model.Work_Title.Split("_").Length == 4) {
                        model.Work_Title = model.Work_Title.Substring(0, model.Work_Title.LastIndexOf("_"));
                    }
                    model.Work_Title = model.Work_Title + "_" + unitM.UnitName;
                }
                model.Comment = vmodel.Comment;
                model.Comment_Time = DateTime.Now;
                var result = _currencyService.DbAccess().Updateable<C_Course_Work>(model).Where(it => it.Id == vmodel.Id).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "点评成功";
                }
            }
            return Json(rsg);
        }
        /// <summary>
        /// 保存作业
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveWork(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "保存作业成功" };
            try
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var model = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => it.Id == vmodel.Id).First();
                if (vmodel.IsSendWork == 1)
                {
                    rsg.code = 0;
                    rsg.msg = "课程作业已被推送！不能再推送";
                }
                else if (string.IsNullOrEmpty(vmodel.CourseWork))
                {
                    rsg.code = 0;
                    rsg.msg = "作业内容为空！不能保存";
                }
                else
                {
                    var toaken = GetWXToken();
                    //1对1
                    if (model.StudentUid > 0) {
                        var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(u => u.StudentUid==model.StudentUid).First();
                        if (!string.IsNullOrEmpty(student.OpenId)&&!string.IsNullOrEmpty(toaken)) {
                            SendMsgHomeWork(student.OpenId, _wxConfig.TemplateId,toaken,"课程作业提醒", model.Work_Title,"", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                        }
                        if (!string.IsNullOrEmpty(student.Elder_OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            SendMsgHomeWork(student.Elder_OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                        }
                        if (!string.IsNullOrEmpty(student.Elder2_OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            SendMsgHomeWork(student.Elder2_OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                        }
                    }
                    //班课
                    if (model.ClasssId > 0) {
                        List<string> studentOpenIds = new List<string>();
                        List<string> elderOpenIds = new List<string>();
                        List<string> elderOpenIds2 = new List<string>();
                        int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                        List<C_Contrac_User> listchild = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Contrac_User>((contrac, u) => new object[] { JoinType.Left, contrac.StudentUid == u.StudentUid }).Where(contrac => contrac.ClassId ==model.ClasssId && contrac.Contrac_Child_Status != contracStatus).Select((contrac, u) => u).ToList();
                        listchild.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(iv.OpenId) && !studentOpenIds.Contains(iv.OpenId))
                            {
                                studentOpenIds.Add(iv.OpenId);
                            }
                            if (!string.IsNullOrEmpty(iv.Elder_OpenId) && !elderOpenIds.Contains(iv.Elder_OpenId))
                            {
                                elderOpenIds.Add(iv.Elder_OpenId);
                            }
                            if (!string.IsNullOrEmpty(iv.Elder2_OpenId) && !elderOpenIds2.Contains(iv.Elder2_OpenId))
                            {
                                elderOpenIds2.Add(iv.Elder2_OpenId);
                            }
                        });
                        //推送给学生
                        studentOpenIds.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            }
                        });
                        //推送给家长1
                        elderOpenIds.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            }
                        });
                        //推送给家长2
                        elderOpenIds2.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            }
                        });
                    }
                    vmodel.CreateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(it => new C_Course_Work { CourseWork = vmodel.CourseWork,IsSendWork=1}).Where(it => it.Id == vmodel.Id).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存作业成功";
                    }
                }
            }
            catch (Exception er) {
                rsg.code = 0;
                rsg.msg = er.Message;
                log.Info("课程作业推送失败，原因"+er.Message);
            }
            return Json(rsg);
        }

        public string GetWXToken() {
            WXAcceSSToken wxtokenModel = new WXAcceSSToken(); ;
            if (RedisLock.KeyExists("wxAccessToken", _redsconfig.RedisCon))
            {
                wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", _redsconfig.RedisCon);
            }
            else
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, _redsconfig.RedisCon);
            }
            return wxtokenModel.Access_Token;
        }


        /// <summary>
        /// 手动推送点评消息
        /// </summary>
        /// <param name="wkId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendCourseComment(int wkId) {
            ResResult rsg = new ResResult() { code = 200, msg = "推送点评成功" };
            try
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                WXAcceSSToken wxtokenModel = new WXAcceSSToken(); ;
                if (RedisLock.KeyExists("wxAccessToken", _redsconfig.RedisCon))
                {
                    wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", _redsconfig.RedisCon);
                }
                else
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                    string tokenCotent = HttpHelper.HttpGet(url);
                    wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                    RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, _redsconfig.RedisCon);
                }
                CourseWorkModel item = _currencyService.DbAccess().Queryable<C_Course_Work, sys_user, C_Room, C_Contrac_User>((cour, ta, room, u) => new object[] { JoinType.Left, cour.TeacherUid == ta.User_ID, JoinType.Left, cour.RoomId == room.Id, JoinType.Left, cour.StudentUid == u.StudentUid })
                    .Where(cour => cour.Id == wkId).Select<CourseWorkModel>((cour, ta, room, u) => new CourseWorkModel
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
                        RoomName = room.RoomName,
                        Student_Name = u.Student_Name,
                        StudentOpenId = u.OpenId,
                        ElderOpenId = u.Elder_OpenId,
                        Elder2OpenId = u.Elder2_OpenId,
                        TeacherOpenId = ta.OpenId,
                        IsSendComment=cour.IsSendComment
                    }).First();
                if (string.IsNullOrEmpty(item.Comment))
                {
                    rsg.code = 0;
                    rsg.msg = "当前课程未点评,无法推送";
                    return Json(rsg);
                }
                if (item.IsSendComment == 1)
                {
                    rsg.code = 0;
                    rsg.msg = "当前课程已被推送,无法连续推送";
                    return Json(rsg);
                }
                string msg = "课程点评消息";
                string wkTime = item.AT_Date.ToString("yyyy-MM-dd") + " " + item.StartTime + "-" + item.EndTime;
                if (item.StudyMode == 1)
                {
                    //推送消息给学生
                    if (!string.IsNullOrEmpty(item.StudentOpenId) && !string.IsNullOrEmpty(item.Comment)&&!string.IsNullOrEmpty(wxtokenModel.Access_Token))
                    {
                        SendMsg(item.StudentOpenId, _wxConfig.TemplateIdComend, wxtokenModel.Access_Token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, item.Student_Name, item.Id);
                    }
                    //推送消息给家长1
                    if (!string.IsNullOrEmpty(item.ElderOpenId) && !string.IsNullOrEmpty(item.Comment) && !string.IsNullOrEmpty(wxtokenModel.Access_Token))
                    {
                       SendMsg(item.ElderOpenId, _wxConfig.TemplateIdComend, wxtokenModel.Access_Token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, item.Student_Name, item.Id);
                    }
                    //推送消息给家长2
                    if (!string.IsNullOrEmpty(item.Elder2OpenId) && !string.IsNullOrEmpty(item.Comment) && !string.IsNullOrEmpty(wxtokenModel.Access_Token))
                    {
                        SendMsg(item.Elder2OpenId, _wxConfig.TemplateIdComend, wxtokenModel.Access_Token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, item.Student_Name, item.Id);
                    }
                }
                else if (item.StudyMode == 2)
                {
                    List<string> studentOpenIds = new List<string>();
                    List<string> elderOpenIds = new List<string>();
                    List<string> elderOpenIds2 = new List<string>();
                    int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                    List<C_Contrac_User> listchild = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Contrac_User>((contrac, u) => new object[] { JoinType.Left, contrac.StudentUid == u.StudentUid }).Where(contrac => contrac.ClassId == item.ClasssId&&contrac.Contrac_Child_Status!= contracStatus).Select((contrac, u)=>u).ToList();
                    listchild.ForEach(iv =>
                    {
                        if (!string.IsNullOrEmpty(iv.OpenId)&&!studentOpenIds.Contains(iv.OpenId))
                        {
                            studentOpenIds.Add(iv.OpenId);
                        }
                        if (!string.IsNullOrEmpty(iv.Elder_OpenId) && !elderOpenIds.Contains(iv.Elder_OpenId))
                        {
                            elderOpenIds.Add(iv.Elder_OpenId);
                        }
                        if (!string.IsNullOrEmpty(iv.Elder2_OpenId) && !elderOpenIds2.Contains(iv.Elder2_OpenId))
                        {
                            elderOpenIds2.Add(iv.Elder2_OpenId);
                        }
                    });
                    //推送给学生
                    studentOpenIds.ForEach(iv =>
                    {
                        var studentUser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cm => cm.OpenId == iv).First();
                        if (!string.IsNullOrEmpty(item.Comment) && studentUser != null&&wxtokenModel!=null && !string.IsNullOrEmpty(wxtokenModel.Access_Token))
                        {
                            SendMsg(iv, _wxConfig.TemplateIdComend, wxtokenModel.Access_Token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, studentUser.Student_Name, item.Id);
                        }
                    });
                    //推送给家长1
                    elderOpenIds.ForEach(iv =>
                    {
                        var studentUser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cm => cm.Elder_OpenId == iv).First();
                        if (!string.IsNullOrEmpty(item.Comment) && studentUser != null&&wxtokenModel!=null && !string.IsNullOrEmpty(wxtokenModel.Access_Token))
                        {
                            SendMsg(iv, _wxConfig.TemplateIdComend, wxtokenModel.Access_Token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, studentUser.Student_Name, item.Id);
                        }
                    });
                    //推送给家长2
                    elderOpenIds2.ForEach(iv =>
                    {
                        var studentUser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cm => cm.Elder2_OpenId == iv).First();
                        if (!string.IsNullOrEmpty(item.Comment)&&studentUser!=null && wxtokenModel != null && !string.IsNullOrEmpty(wxtokenModel.Access_Token))
                        {
                            SendMsg(iv, _wxConfig.TemplateIdComend, wxtokenModel.Access_Token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, studentUser.Student_Name, item.Id);
                        }
                    });
                }
                var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(it => new C_Course_Work { IsSendComment = 1 }).Where(it => it.Id == wkId).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "推送点评成功";
                }

            }
            catch (Exception er) {
                log.Info(er.Message);
                rsg.code = 0;
                rsg.msg = "推送异常,异常原因"+er.Message;
            }
            return Json(rsg);
        }

        public void SendMsg(string openId, string templateId, string wxaccessToken, string msg, string wkTitle, string wkTeacher, string wkTime, string commend, string studentName, int wkId)
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
            keyword3.Add("value", "任课老师");
            keyword3.Add("color", "#173177");
            Dictionary<string, string> remark = new Dictionary<string, string>();
            remark.Add("value", "点评内容:" + commend);
            remark.Add("color", "#173177");
            data.Add("first", first);
            data.Add("keyword1", keyword1);
            data.Add("keyword2", keyword2);
            data.Add("keyword3", keyword3);
            data.Add("remark", remark);
            jsonObject.Add("data", data);
            jsonObject.Add("url", "http://crm.younengkao.com/WxTemplateChild/Index?wkId=" + wkId);//设置链接
            var jsonStr = JsonConvert.SerializeObject(jsonObject);
            var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + wxaccessToken;
            string content = HttpHelper.HttpPost(api, jsonStr, "application/json");
        }

        //发送家庭作业
        public void SendMsgHomeWork(string openId, string templateId, string wxaccessToken, string msg, string wkTitle, string wkTeacher, string wkTime, string homeWork)
        {
            Dictionary<string, object> jsonObject = new Dictionary<string, object>();
            jsonObject.Add("touser", openId);   // openid
            jsonObject.Add("template_id", templateId);
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, string> first = new Dictionary<string, string>();
            first.Add("value", msg);
            first.Add("color", "#173177");
            Dictionary<string, string> keyword1 = new Dictionary<string, string>();
            keyword1.Add("value", wkTitle);
            keyword1.Add("color", "#173177");
            Dictionary<string, string> keyword2 = new Dictionary<string, string>();
            keyword2.Add("value", wkTime);
            keyword2.Add("color", "#173177");
            Dictionary<string, string> remark = new Dictionary<string, string>();
            remark.Add("value", "布置作业:" + homeWork);
            remark.Add("color", "#173177");

            data.Add("first", first);
            data.Add("keyword1", keyword1);
            data.Add("keyword2", keyword2);
            data.Add("remark", remark);
            jsonObject.Add("data", data);
            var jsonStr = JsonConvert.SerializeObject(jsonObject);
            var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + wxaccessToken;
            string content = HttpHelper.HttpPost(api, jsonStr, "application/json");
        }


        /// <summary>
        /// 获取排课计划列表
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="studymode"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startStr, string endStr, string userName, int studymode)
        {
            //获取当前登录老师的课程
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            ResResult reg = new ResResult();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(@"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName,cl.Class_Name from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.TeacherUid=@TeacherUid and wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date))", "orginSql")
           .AddParameters(new { startStr = startStr, endStr = endStr, TeacherUid = userId })
          .WhereIF(!string.IsNullOrEmpty(userName), "(orginSql.Student_Name=@userName or orginSql.Class_Name=@userName)").AddParameters(new { userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToList();
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            if (list != null && list.Count > 0)
            {
                reg.totalRow = new totalRow();
                //统计老师课时，点评已完成才算课时
                reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .Where(it => it.TeacherUid == userId&& it.StudyMode != 3 && it.StudyMode != 7 && it.StudyMode != 5 && it.StudyMode != 6 && it.Work_Stutas == 1 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
            }
            return Json(reg);
        }


        public IActionResult QueryWorkSource2(string startStr, string endStr, string userName, int studymode,int page=10,int limit=10)
        {
            //获取当前登录老师的课程
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            ResResult reg = new ResResult();
            List<CourseWorkModel> list = _currencyService.DbAccess().Queryable(@"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName,cl.Class_Name from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.TeacherUid=@TeacherUid and wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date))", "orginSql")
           .AddParameters(new { startStr = startStr, endStr = endStr, TeacherUid = userId })
          .WhereIF(!string.IsNullOrEmpty(userName), "(orginSql.Student_Name=@userName or orginSql.Class_Name=@userName)").AddParameters(new { userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            if (list != null && list.Count > 0)
            {
                reg.totalRow = new totalRow();
                //统计老师课时，点评已完成才算课时
                reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .Where(it => it.TeacherUid == userId && it.StudyMode != 3 && it.StudyMode != 7 && it.StudyMode != 5 && it.StudyMode != 6 && it.Work_Stutas == 1 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
            }
            reg.count = total;
            return Json(reg);
        }


    }
}
