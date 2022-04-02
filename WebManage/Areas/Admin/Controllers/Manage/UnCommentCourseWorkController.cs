﻿using ADT.Common;
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
    public class UnCommentCourseWorkController : BaseController
    {

        private ICurrencyService _currencyService;
        private RedisConfig _redsconfig;
        private WXSetting _wxConfig;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(UnCommentCourseWorkController));
        public UnCommentCourseWorkController(ICurrencyService currencyService, IOptions<WXSetting> wxConfig, IOptions<RedisConfig> redisConfig)
        {
            _currencyService = currencyService;
            _wxConfig = wxConfig.Value;
            _redsconfig = redisConfig.Value;
        }

        protected override void Init()
        {
            this.MenuID = "V-353";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("V-353", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("V-353", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title,int workStutas, int studymode, DateTime? startTime=null, DateTime? endTime = null, int page = 1, int limit = 10)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID== ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u,ur,r)=>u.User_ID==userId&&r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user>((c, ca, ta) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID })
                .Where(c => c.CampusId == Convert.ToInt32(campusId)&& c.Work_Stutas == workStutas&&c.StudyMode!=3)
                .WhereIF(studymode>0, c => c.StudyMode== studymode)
                .WhereIF(teacher!=null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue,c=>c.AT_Date>=startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date< endTime.Value)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca) => c.Work_Title.Contains(title)).OrderByIF(workStutas>0, c => c.AT_Date,OrderByType.Desc).OrderByIF(workStutas <1, c => c.AT_Date, OrderByType.Asc).Select<CourseWorkModel>((c, ca, ta) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName=ta.User_Name,
                    StudyMode=c.StudyMode,
                    AT_Date=c.AT_Date,
                    Comment=c.Comment,
                    Comment_Time=c.Comment_Time,
                    CreateTime = c.CreateTime,
                    CreateUid = c.CreateUid
                }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        [UsersRoleAuthFilter("V-353", "Add,Edit")]
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

        [UsersRoleAuthFilter("V-353", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("V-353", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }


        /// <summary>
        /// 保存点评
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("V-353", "Add,Edit")]
        public IActionResult SaveCommend(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.Comment))
                    return Json(new { code = 0, msg = "点评内容不能为空" });
                if (vmodel.Id > 0)
                {
                    C_Course_Work work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id == vmodel.Id).First();
                    if (work.IsSendComment==1) {
                        return Json(new { code = 0, msg = "点评内容已被推送，无法修改" });
                    }
                    work.Comment = vmodel.Comment;
                    work.UpdateUid = userId;
                    work.Work_Stutas = 1;
                    work.Comment_Time = DateTime.Now;//点评时间
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>(work).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
            }
            else
            {
                rsg.msg = "缺少参数";
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
                    if (model.StudentUid > 0)
                    {
                        var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(u => u.StudentUid == model.StudentUid).First();
                        if (!string.IsNullOrEmpty(student.OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            SendMsgHomeWork(student.OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
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
                    if (model.ClasssId > 0)
                    {
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
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(it => new C_Course_Work { CourseWork = vmodel.CourseWork,IsSendWork=1 }).Where(it => it.Id == vmodel.Id).ExecuteCommand();
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
                log.Info("课程作业推送失败，原因" + er.Message);
            }
            return Json(rsg);
        }

        public string GetWXToken()
        {
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

    }
}
