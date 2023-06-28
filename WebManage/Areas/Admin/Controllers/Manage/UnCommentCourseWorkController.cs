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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
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
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user,C_Contrac_User>((c, ca, ta,u) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID, JoinType.Left,c.StudentUid==u.StudentUid })
                .Where(c => c.CampusId == Convert.ToInt32(campusId)&&c.StudyMode!=3&&c.StudyMode!=7&& c.StudyMode!=5 && c.StudyMode != 6 && c.StudyMode != 9)
                .WhereIF(workStutas>0,c=>!string.IsNullOrEmpty(c.Comment)&&!string.IsNullOrEmpty(c.CourseWork))
                .WhereIF(workStutas <1,c =>(string.IsNullOrEmpty(c.Comment)||string.IsNullOrEmpty(c.CourseWork)))
                .WhereIF(studymode>0, c => c.StudyMode== studymode)
                .WhereIF(teacher!=null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue,c=>c.AT_Date>=startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date< endTime.Value)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca,ta,u) => c.Work_Title.Contains(title)||ta.User_Name.Contains(title)||u.Student_Name.Contains(title)).OrderByIF(workStutas>0, c => c.AT_Date,OrderByType.Desc).OrderByIF(workStutas <1, c => c.AT_Date, OrderByType.Asc).Select<CourseWorkModel>((c, ca, ta,u) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName=ta.User_Name,
                    Student_Name=u.Student_Name,
                    ListeningName = c.ListeningName,
                    Score =c.Score,
                    StudyMode=c.StudyMode,
                    AT_Date=c.AT_Date,
                    Comment=c.Comment,
                    Comment_Time=c.Comment_Time,
                    CreateTime = c.CreateTime,
                    CreateUid = c.CreateUid,
                    Work_Stutas=c.Work_Stutas,
                    CourseWork=c.CourseWork
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
            ResResult rsg = new ResResult() { code = 0, msg = "保存点评失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var roleModel = _currencyService.DbAccess().Queryable<sys_userrole, sys_role>((ur, r) => new Object[] { JoinType.Left, ur.UserRole_RoleID == r.Role_ID }).Where((ur, r) => ur.UserRole_UserID == userId).Select((ur, r)=>r).First();
                if (string.IsNullOrEmpty(vmodel.Comment)&&roleModel.Role_Name!="校长"&& roleModel.Role_Name != "督学校长" && roleModel.Role_Name != "超级管理员")
                    return Json(new { code = 0, msg = "点评内容不能为空" });
                if (vmodel.Id > 0)
                {
                    C_Course_Work work = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(f => f.Id == vmodel.Id).First();

                    if ((roleModel.Role_Name == "校长" || roleModel.Role_Name == "督学校长" || roleModel.Role_Name == "超级管理员"))
                    {
                        if (!string.IsNullOrEmpty(vmodel.Comment)) {
                            work.Work_Stutas = 1;
                        }
                        else {
                            work.Work_Stutas =0;
                            work.Comment_Time = null;
                        }
                    }
                    else {
                        if (work.IsSendComment == 1)
                        {
                            return Json(new { code = 0, msg = "点评内容已被推送，无法修改" });
                        }
                        var valiteTime = DateTime.Parse(work.AT_Date.ToString("yyyy-MM-dd") + " " + work.EndTime);
                        work.UpdateUid = userId;
                        //如果在23小时之内,则课时有效
                        if (work.StudyMode != 5 && work.StudyMode != 6)
                        {
                            if (work.Work_Stutas != 1)
                            {
                                if (valiteTime.AddHours(24) > DateTime.Now && !string.IsNullOrEmpty(vmodel.Comment))
                                {
                                    work.Work_Stutas = 1;
                                }
                                else
                                {
                                    work.Work_Stutas = 0;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(vmodel.Comment))
                                {
                                    work.Work_Stutas = 0;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(vmodel.Comment) && work.Work_Stutas != 1)
                            {
                                work.Work_Stutas = 1;
                            }
                        }
                        if (vmodel.UnitId > 0 && work.UnitId != vmodel.UnitId)
                        {
                            work.UnitId = vmodel.UnitId;
                            var unitM = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(y => y.UnitId == vmodel.UnitId).First();
                            if (work.Work_Title.Split("_").Length == 4)
                            {
                                work.Work_Title = work.Work_Title.Substring(0, work.Work_Title.LastIndexOf("_"));
                            }
                            work.Work_Title = work.Work_Title + "_" + unitM.UnitName;
                        }
                        work.Comment_Time = DateTime.Now;//点评时间
                    }
                    work.Comment = vmodel.Comment;
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>(work).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存点评成功";
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
        /// 手动推送点评消息
        /// </summary>
        /// <param name="wkId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendCourseComment(int wkId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "推送点评成功" };
            try
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
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
                        IsSendComment = cour.IsSendComment
                    }).First();
                if (string.IsNullOrEmpty(item.Comment))
                {
                    rsg.code = 0;
                    rsg.msg = "当前课程点评未保存,请先保存点评";
                    return Json(rsg);
                }
                if (item.IsSendComment == 1)
                {
                    rsg.code = 0;
                    rsg.msg = "当前课程已被推送,无法连续推送";
                    return Json(rsg);
                }
                var token = GetWXToken();
                string msg = "课程点评消息";
                string wkTime = item.AT_Date.ToString("yyyy-MM-dd") + " " + item.StartTime + "-" + item.EndTime;
                if (item.StudyMode == 1)
                {
                    //推送消息给学生
                    if (!string.IsNullOrEmpty(item.StudentOpenId) && !string.IsNullOrEmpty(item.Comment) && !string.IsNullOrEmpty(token))
                    {
                        SendMsg(item.StudentOpenId, _wxConfig.TemplateIdComend,token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, item.Student_Name, item.Id);
                    }
                    //推送消息给家长1
                    if (!string.IsNullOrEmpty(item.ElderOpenId) && !string.IsNullOrEmpty(item.Comment) && !string.IsNullOrEmpty(token))
                    {
                        SendMsg(item.ElderOpenId, _wxConfig.TemplateIdComend, token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, item.Student_Name, item.Id);
                    }
                    //推送消息给家长2
                    if (!string.IsNullOrEmpty(item.Elder2OpenId) && !string.IsNullOrEmpty(item.Comment) && !string.IsNullOrEmpty(token))
                    {
                        SendMsg(item.Elder2OpenId, _wxConfig.TemplateIdComend, token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, item.Student_Name, item.Id);
                    }
                }
                else if (item.StudyMode == 2)
                {
                    List<string> studentOpenIds = new List<string>();
                    List<string> elderOpenIds = new List<string>();
                    List<string> elderOpenIds2 = new List<string>();
                    int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                    List<C_Contrac_User> listchild = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Contrac_User>((contrac, u) => new object[] { JoinType.Left, contrac.StudentUid == u.StudentUid }).Where(contrac => contrac.ClassId == item.ClasssId && contrac.Contrac_Child_Status != contracStatus).Select((contrac, u) => u).ToList();
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
                        var studentUser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cm => cm.OpenId == iv).First();
                        if (!string.IsNullOrEmpty(item.Comment) && studentUser != null && !string.IsNullOrEmpty(token))
                        {
                            SendMsg(iv, _wxConfig.TemplateIdComend,token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, studentUser.Student_Name, item.Id);
                        }
                    });
                    //推送给家长1
                    elderOpenIds.ForEach(iv =>
                    {
                        var studentUser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cm => cm.Elder_OpenId == iv).First();
                        if (!string.IsNullOrEmpty(item.Comment) && studentUser != null &&!string.IsNullOrEmpty(token))
                        {
                            SendMsg(iv, _wxConfig.TemplateIdComend, token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, studentUser.Student_Name, item.Id);
                        }
                    });
                    //推送给家长2
                    elderOpenIds2.ForEach(iv =>
                    {
                        var studentUser = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cm => cm.Elder2_OpenId == iv).First();
                        if (!string.IsNullOrEmpty(item.Comment) && studentUser != null&& !string.IsNullOrEmpty(token))
                        {
                            SendMsg(iv, _wxConfig.TemplateIdComend, token, msg, item.Work_Title, item.TeacherName, wkTime, item.Comment, studentUser.Student_Name, item.Id);
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
            catch (Exception er)
            {
                log.Info(er.Message);
                rsg.code = 0;
                rsg.msg = "推送异常,异常原因" + er.Message;
            }
            return Json(rsg);
        }


        //更新分数
        public IActionResult SetScore(C_Course_Work vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.Id > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(n => new C_Course_Work { Score = vmodel.Score }).Where(n => n.Id == vmodel.Id).ExecuteCommand();
                    if (result > 0)
                    {
                        return Json(new { code = 200, msg = "更新成功" });
                    }
                    else
                    {
                        return Json(new { code = code, msg = "更新失败" });
                    }
                }
            }
            catch (Exception er)
            {

            }
            return Json(new { code = code, msg = "缺少参数" });
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
                            //SendMsgHomeWork(student.OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            SendMsgHomeWork2(student.OpenId, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title,"", model.AT_Date.ToString("yyyy-MM-dd"),vmodel.CourseWork,student.Student_Name,model.Id);
                        }
                        if (!string.IsNullOrEmpty(student.Elder_OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            //SendMsgHomeWork(student.Elder_OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            SendMsgHomeWork2(student.Elder_OpenId, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork, student.Student_Name, model.Id);
                        }
                        if (!string.IsNullOrEmpty(student.Elder2_OpenId) && !string.IsNullOrEmpty(toaken))
                        {
                            //SendMsgHomeWork(student.Elder2_OpenId, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                            SendMsgHomeWork2(student.Elder_OpenId, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork,student.Student_Name, model.Id);
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
                                ovtStudent.Add(new C_Contrac_User{OpenId=iv.OpenId,Student_Name=iv.Student_Name });
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
                                var stuName = ovtStudent.Find(cc=>cc.OpenId==iv).Student_Name;
                                SendMsgHomeWork2(iv, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork,stuName, model.Id);
                            }
                        });
                        //推送给家长1
                        elderOpenIds.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                //SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                                var stuName = ovtElder.Find(cc => cc.Elder_OpenId == iv).Student_Name;
                                SendMsgHomeWork2(iv, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork, stuName, model.Id);
                            }
                        });
                        //推送给家长2
                        elderOpenIds2.ForEach(iv =>
                        {
                            if (!string.IsNullOrEmpty(toaken) && !string.IsNullOrEmpty(model.CourseWork))
                            {
                                //SendMsgHomeWork(iv, _wxConfig.TemplateId, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork);
                                var stuName = ovtElder2.Find(cc => cc.Elder2_OpenId == iv).Student_Name;
                                SendMsgHomeWork2(iv, _wxConfig.TemplateHomeWork, toaken, "课程作业提醒", model.Work_Title, "", model.AT_Date.ToString("yyyy-MM-dd"), vmodel.CourseWork, stuName, model.Id);
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
        //发送点评消息
        public void SendMsg(string openId, string templateId, string wxaccessToken, string msg, string wkTitle, string wkTeacher, string wkTime, string commend, string studentName, int wkId)
        {
            try
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
            catch
            {
                if (RedisLock.KeyExists("wxAccessToken", _redsconfig.RedisCon))
                {
                    RedisLock.KeyDelete("wxAccessToken", _redsconfig.RedisCon);
                }
                wxaccessToken = GetWXToken();
                SendMsg(openId, templateId, wxaccessToken, msg, wkTitle, wkTeacher, wkTime, commend, studentName, wkId);
            }

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

        //发送家庭作业模板2
        public void SendMsgHomeWork2(string openId, string templateId, string wxaccessToken, string msg, string wkTitle, string wkTeacher, string wkTime, string courseWork, string studentName, int wkId)
        {
            try
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
            catch {
                if (RedisLock.KeyExists("wxAccessToken", _redsconfig.RedisCon))
                {
                    RedisLock.KeyDelete("wxAccessToken", _redsconfig.RedisCon);
                }
                wxaccessToken = GetWXToken();
                SendMsgHomeWork2(openId, templateId, wxaccessToken,msg, wkTitle, wkTeacher, wkTime, courseWork, studentName, wkId);
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="endtime"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public virtual IActionResult ExportPlan(string title, int workStutas, int studymode, DateTime? startTime = null, DateTime? endTime = null)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            sys_user teacher = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.User_ID == userId && r.Role_Name.Contains("教师")).First();
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Campus, sys_user,C_Contrac_User>((c, ca, ta,u) => new Object[] { JoinType.Left, c.CampusId == ca.CampusId, JoinType.Left, c.TeacherUid == ta.User_ID, JoinType.Left, c.StudentUid == u.StudentUid })
                .Where(c => c.CampusId == Convert.ToInt32(campusId) && c.Work_Stutas == workStutas && c.StudyMode != 3 && c.StudyMode != 7 && c.StudyMode != 5 && c.StudyMode != 6)
                .WhereIF(studymode > 0, c => c.StudyMode == studymode)
                .WhereIF(teacher != null, c => c.TeacherUid == userId)
                .WhereIF(startTime.HasValue, c => c.AT_Date >= startTime.Value)
                .WhereIF(endTime.HasValue, c => c.AT_Date < endTime.Value)
                .WhereIF(!string.IsNullOrEmpty(title), (c, ca, ta,u) => c.Work_Title.Contains(title) || ta.User_Name.Contains(title)||u.Student_Name.Contains(title)).OrderByIF(workStutas > 0, c => c.AT_Date, OrderByType.Desc).OrderByIF(workStutas < 1, c => c.AT_Date, OrderByType.Asc).Select<CourseWorkModel>((c, ca, ta,u) => new CourseWorkModel
                {
                    Id = c.Id,
                    Work_Title = c.Work_Title,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    TeacherName = ta.User_Name,
                    Student_Name=u.Student_Name,
                    ListeningName=c.ListeningName,
                    Score=c.Score,
                    StudyMode = c.StudyMode,
                    AT_Date = c.AT_Date,
                    Comment = c.Comment,
                    Comment_Time = c.Comment_Time,
                    CreateTime = c.CreateTime,
                    CreateUid = c.CreateUid
                }).ToList();
            //导出代码
            var workbook = new HSSFWorkbook();
            //标题列样式
            var headFont = workbook.CreateFont();
            headFont.IsBold = true;
            var headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.BorderLeft = BorderStyle.Thin;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.WrapText = true;//自动换行
            headStyle.SetFont(headFont);
            var sheet = workbook.CreateSheet("点评列表");
            sheet.DefaultColumnWidth = 25;
            string[] cellNames = new string[] { "课程名称", "上课日期", "开始时间", "结束时间", "教师", "上课模式", "点评内容","点评时间"};
            //循环行
            var row0 = sheet.CreateRow(0);
            for (var i = 0; i < cellNames.Length; i++)
            {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(cellNames[i]);
                cell.CellStyle = headStyle;
            }
            if (list != null && list.Count > 0)
            {
                for (var y = 0; y < list.Count; y++)
                {
                    string studyModelName = "";
                    string comment = "";
                    if (list[y].StudyMode == 1) {
                        studyModelName = "1对1";
                    }
                    else if (list[y].StudyMode == 2) {
                        studyModelName = "小班";
                    }
                    else if (list[y].StudyMode == 4)
                    {
                        studyModelName = "试听";
                    }
                    else if (list[y].StudyMode == 5)
                    {
                        studyModelName = "模考";
                    }
                    else if (list[y].StudyMode ==6)
                    {
                        studyModelName = "实考";
                    }
                    if (!string.IsNullOrEmpty(list[y].Comment))
                    {

                        comment = list[y].Comment;
                    }
                    var row = sheet.CreateRow(y + 1);
                    var cell0 = row.CreateCell(0);
                    cell0.SetCellValue(list[y].Work_Title);
                    cell0.CellStyle = headStyle;
                    var cell1 = row.CreateCell(1);
                    cell1.SetCellValue(list[y].AT_Date.ToString("yyyy-MM-dd"));
                    cell1.CellStyle = headStyle;
                    var cell2 = row.CreateCell(2);
                    cell2.SetCellValue(list[y].StartTime);
                    cell2.CellStyle = headStyle;
                    var cell3 = row.CreateCell(3);
                    cell3.SetCellValue(list[y].EndTime);
                    cell3.CellStyle = headStyle;
                    var cell4 = row.CreateCell(4);
                    cell4.SetCellValue(list[y].TeacherName);
                    cell4.CellStyle = headStyle;
                    var cell5 = row.CreateCell(5);
                    cell5.SetCellValue(studyModelName);
                    cell5.CellStyle = headStyle;
                    var cell6 = row.CreateCell(6);
                    cell6.SetCellValue(comment);
                    cell6.CellStyle = headStyle;
                    var cell7 = row.CreateCell(7);
                    cell7.SetCellValue(list[y].Comment_Time.HasValue? list[y].Comment_Time.Value.ToString("yyyy-MM-dd"):"");
                    cell7.CellStyle = headStyle;
                }
            }
            //保存
            byte[] streamArr = null;
            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                streamArr = ms.ToArray();
            }
            return File(streamArr, "application/vnd.ms-excel", "点评列表" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }
    }
}
