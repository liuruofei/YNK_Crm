using ADT.Common;
using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.InputModel;
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
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class CourseWorkController : BaseController
    {
        private RedisConfig redisConfig;
        private WXSetting _wxConfig;
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        private IC_CourseWorkService _courseWork;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(CourseWorkController));
        public CourseWorkController(ICurrencyService currencyService, IC_ContracService contrac, IC_CourseWorkService courseWork, IOptions<RedisConfig> _redisConfig, IOptions<WXSetting> wxConfig)
        {
            _currencyService = currencyService;
            _contrac = contrac;
            _courseWork = courseWork;
            _wxConfig = wxConfig.Value;
            redisConfig = _redisConfig.Value;
        }

        protected override void Init()
        {
            this.MenuID = "V-350";
        }

        [UsersRoleAuthFilter("V-350", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }



        [UsersRoleAuthFilter("V-350", FunctionEnum.Add)]
        public IActionResult Add(string dataStr, string teacherName)
        {
            if (string.IsNullOrEmpty(dataStr)) {
                dataStr = DateTime.Now.ToString("yyyy-MM-dd");
            }
            ViewBag.ID = 0;
            ViewBag.teacherId = "";
            ViewBag.DataStr = dataStr;
            if (!string.IsNullOrEmpty(teacherName))
            {
                var teach = _currencyService.DbAccess().Queryable<sys_user>().Where(it => it.User_Name.Equals(teacherName)).First();
                if (teach != null)
                {
                    ViewBag.teacherId = teach.User_ID;
                }
            }
            return View();
        }

        [UsersRoleAuthFilter("V-350", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.teacherId = "";
            ViewBag.DataStr = "";
            ViewBag.ID = ID;
            return View("Add");
        }


        [UsersRoleAuthFilter("V-350", "Add,Edit")]
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

        public IActionResult AddComment(int ID) {
            ViewBag.ID = ID;
            return View();
        }

        /// <summary>
        /// 查询老师
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryTeacher()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<sys_user> list= _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.CampusId == Convert.ToInt32(campusId) && (r.Role_Name == "教师" || r.Role_Name == "教学校长")).ToList();
            list.Add(new sys_user { User_ID = "", User_Name = "-请选择教师-",User_CreateTime=DateTime.Now});
            rsg.data = list.OrderByDescending(n=>n.User_CreateTime).ToList();
            return Json(rsg);
        }
        /// <summary>
        /// 查询时间段
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryRangeTime()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Range_Time> list = _currencyService.DbAccess().Queryable<C_Range_Time>().Where(r=>r.CampusId==Convert.ToInt32(campusId)).ToList();
            list = list.OrderBy(it => DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + it.StartTime)).ToList();
            list.Add(new C_Range_Time() { Id = 0, TimeName = "--自定义--" });
            rsg.data = list;
            return Json(rsg);
        }
        /// <summary>
        /// 下拉框查询小班或者一对一
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryUserChild(int studyModel, string title)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            if (studyModel == 1)
            {
                
                var list = _currencyService.DbAccess().Queryable<C_Contrac_Child_Detail, C_Contrac_User, C_Subject, C_Project,C_Contrac_Child>((ch, cu, su, pr,c) => new object[] { JoinType.Left, ch.StudentUid == cu.StudentUid, JoinType.Left, ch.SubjectId == su.SubjectId, JoinType.Left, ch.ProjectId == pr.ProjectId, JoinType.Left, ch.Contra_ChildNo ==c.Contra_ChildNo })
                    .Where((ch, cu, su, pr, c)=>(c.Pay_Stutas==(int)ConstraChild_Pay_Stutas.PartPay|| c.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PayOk)
                    && (c.Contrac_Child_Status == (int)ConstraChild_Status.Confirmationed || c.Contrac_Child_Status == (int)ConstraChild_Status.ChangeClassOk || c.Contrac_Child_Status == (int)ConstraChild_Status.ChangeOk)&&c.CampusId==Convert.ToInt32(campusId))
                    .WhereIF(!string.IsNullOrEmpty(title), (ch, cu) => cu.Student_Name.Contains(title)).Select<StudyWorkModel>((ch, cu, su, pr) => new StudyWorkModel
                {
                    Contra_ChildNo = ch.Contra_ChildNo,
                    StudentUid = ch.StudentUid,
                    Student_Name = cu.Student_Name,
                    StudyMode = 1,
                    SubjectId = ch.SubjectId,
                    ProjectId = ch.ProjectId,
                    SubjectName = su.SubjectName,
                    ProjectName = pr.ProjectName
                }).ToPageList(1, 30);
                if (list != null && list.Count > 0) {
                  var arrChildNo = list.Select(n => n.Contra_ChildNo).ToList(); 
                  var arrCourseTime = _currencyService.DbAccess().Queryable<C_User_CourseTime>().Where(cour => arrChildNo.Contains(cour.Contra_ChildNo)).ToList();
                  if (arrCourseTime != null && arrCourseTime.Count > 0) {
                  list.ForEach(it =>{
                      if (it.StudyMode == 1) {
                          var currtTime = arrCourseTime.Where(v => v.Contra_ChildNo == it.Contra_ChildNo && v.SubjectId == it.SubjectId && v.ProjectId == it.ProjectId&&v.StudentUid==it.StudentUid).First();
                          if (currtTime != null) {
                              it.Course_Time = currtTime.Course_Time;
                              it.Course_UseTime = currtTime.Course_UseTime;
                          }
                      }
                  });
                 }
                }
                list = list.Where(n => n.Course_UseTime != n.Course_Time).ToList();
                rsg.data = list;
            }
            else
            {
                var list = _currencyService.DbAccess().Queryable<C_Class>().Where(it => SqlFunc.Subqueryable<C_Contrac_Child>().Where(s => s.ClassId == it.ClassId&&(s.Contrac_Child_Status==(int)ConstraChild_Status.Confirmationed|| s.Contrac_Child_Status == (int)ConstraChild_Status.ChangeClassOk|| s.Contrac_Child_Status == (int)ConstraChild_Status.ChangeOk)&&s.CampusId==Convert.ToInt32(campusId)).Any())
                    .WhereIF(!string.IsNullOrEmpty(title), cla => cla.Class_Name.Contains(title)).OrderBy(cla => cla.CreateTime, OrderByType.Desc).Select<StudyWorkModel>(cla =>
                   new StudyWorkModel { ClasssId = cla.ClassId, ClassName = cla.Class_Name, SubjectId = cla.SubjectId, StudyMode = 2 }).ToPageList(1, 30);
                if (list != null && list.Count > 0)
                {
                    var arrClassids = list.Select(n => n.ClasssId).ToList(); ;
                    var arrCourseTime = _currencyService.DbAccess().Queryable<C_User_CourseTime>().Where(cour => arrClassids.Contains(cour.ClassId)).ToList();
                    if (arrCourseTime != null && arrCourseTime.Count > 0)
                    {
                        list.ForEach(it => {
                            if (it.StudyMode ==2)
                            {
                                var currtTime = arrCourseTime.Where(v =>v.ClassId==it.ClasssId).First();
                                if (currtTime != null)
                                {
                                    it.Class_Course_Time = currtTime.Class_Course_Time;
                                    it.Class_Course_UseTime = currtTime.Class_Course_UseTime;
                                }
                            }
                        });
                    }
                }
                list = list.Where(n => n.Class_Course_Time != n.Class_Course_UseTime).ToList();
                rsg.data = list;
            }
            return Json(rsg);
        }


        /// <summary>
        /// 获取赠送课时用户
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public IActionResult QueryGiveCourseUser(string title) {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            var list = _currencyService.DbAccess().Queryable<C_Contrac_Child,C_Contrac_User>((ctr,u)=>new object[] {JoinType.Left,ctr.StudentUid==u.StudentUid}).Where((ctr, u)=>ctr.StudyMode==1&&ctr.PresentTime>0&&
            ctr.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PayOk && ctr.CampusId == Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (ctr, u)=>u.Student_Name.Contains(title)).Select((ctr, u)=>new StudyWorkModel { 
             Student_Name=u.Student_Name,
             Contra_ChildNo=ctr.Contra_ChildNo,
             StudentUid=u.StudentUid
            }).ToPageList(1, 30);
            if (list != null && list.Count > 0) {
                var arrChildNo = list.Select(n => n.Contra_ChildNo).ToList();
                var arrCourseTime = _currencyService.DbAccess().Queryable<C_User_PresentTime>().Where(cour => arrChildNo.Contains(cour.Contra_ChildNo)).ToList();
                list.ForEach(it => {
                    var currtTime = arrCourseTime.Where(v => v.Contra_ChildNo == it.Contra_ChildNo&& v.StudentUid == it.StudentUid).First();
                    if (currtTime != null)
                    {
                        it.Course_Time = currtTime.Present_Time;
                        it.Course_UseTime = currtTime.Present_UseTime;
                    }
                });
            }
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        /// 查询学员名称
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IActionResult QueryListenUser(string userName) {
           var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            var list= _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(u => u.Student_Name.Contains(userName)).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        /// 查询督学
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryTa()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<sys_user> list = _currencyService.DbAccess().Queryable<sys_userrole, sys_user, sys_role>((ur, u, r) => new object[] { JoinType.Left, ur.UserRole_UserID == u.User_ID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((ur, u, r) => r.Role_Name == "督学"&&u.CampusId==Convert.ToInt32(campusId)).Select<sys_user>((ur, u, r) => u).ToList();
            list.Add(new sys_user() { User_ID = "", User_Name = "--督学--", User_CreateTime = DateTime.Now });
            list = list.OrderByDescending(it => it.User_CreateTime).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        public IActionResult QueryRoom(string atDate,string stTime,string edTime,int? defaultRoomId=0)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<int> notInRoomId = new List<int>();
            if (!string.IsNullOrEmpty(atDate) && !string.IsNullOrEmpty(stTime) && !string.IsNullOrEmpty(edTime)) {
                var ListWork = _currencyService.DbAccess().Queryable<C_Course_Work>("c").AddJoinInfo("C_Room","r", "c.RoomId=r.Id",JoinType.Left)
                                         .Where("c.RoomId!=@roomId and charindex('线上',r.RoomName)<1 and charindex('考场',r.RoomName)<1  and" +
                                         " ((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                         ")")
                                         .AddParameters(new { roomId = defaultRoomId.Value, AtDate = atDate, StartTime = stTime, EndTime = edTime }).GroupBy("c.RoomId").Select("c.RoomId").ToList();
                if (ListWork != null && ListWork.Count > 0) {
                    ListWork.ForEach(item => {
                        notInRoomId.Add(item.RoomId);
                    });
                }
            }
            List<C_Room> list = _currencyService.DbAccess().Queryable<C_Room>().Where(r=>r.CampusId==Convert.ToInt32(campusId))
                .WhereIF(notInRoomId.Count>0,r=>!notInRoomId.Contains(r.Id)).ToList();
            list.Add(new C_Room() { Id = 0, RoomName = "--选择教室--" });
            list = list.OrderBy(it => it.Id).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        /// 查询科目集合
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public IActionResult QueryProject(int subjectId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Project> list = _currencyService.DbAccess().Queryable<C_Project>().Where(it => it.SubjectId == subjectId).ToList();
            rsg.data = list;
            return Json(rsg);
        }
        /// <summary>
        /// 查询科目单元
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryUnit(int projectId) {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Project_Unit> list = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(it => it.ProjectId == projectId).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        //查询学员，教师，或者班级的名称
        public IActionResult QueryNameAll(string title) {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<SeachUNameModel> listSerchName = new List<SeachUNameModel>();
            if (!string.IsNullOrEmpty(title)) {
                listSerchName = _currencyService.DbAccess().Queryable(@"(select u.Student_Name as Name from C_Contrac_User u where  charindex(@title,u.Student_Name)>0 union all
             (select tach.User_Name as Name from Sys_User tach left join Sys_UserRole ur on tach.User_ID=ur.UserRole_UserID left join Sys_Role r on ur.UserRole_RoleID=r.Role_ID 
              where  (r.Role_Name='教师' or r.Role_Name='教学校长') and charindex(@title,tach.User_Name)>0) union all 
             select c.Class_Name as Name from  C_Class c where  charindex(@title,c.Class_Name)>0)", "orgin").AddParameters(new { title = title }).Select<SeachUNameModel>().ToList();
                rsg.data = listSerchName;
            }
            return Json(rsg);
        }

        /// <summary>
        /// 保存排课课程
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveCourseWork(CourseWorkInput vmodel)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            vmodel.CreateUid = userId;
            vmodel.CampusId=int.Parse(campusId);
            var ccOrSale = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
             .Where((u, ur, r) => u.User_ID == userId &&(r.Role_Name == "销售主管"||r.Role_Name=="顾问" || r.Role_Name == "销售")).First();
            ResResult rsg = new ResResult() { code = 200, msg = "保存排课课程成功" };
            if (ccOrSale != null &&vmodel.StudyMode != 4&&vmodel.StudyMode != 5) {
                rsg.code = 0;
                rsg.msg = "你已超过当前权限,只能添加试听课或者模考";
                return Json(rsg);
            }
            rsg = _courseWork.SaveCourseWork(vmodel);
            return Json(rsg);
        }

        /// <summary>
        /// 拖拽排课
        /// </summary>
        /// <param name="id"></param>
        /// <param name="upAtDate"></param>
        /// <returns></returns>
        public IActionResult DropCourseWork(int id, string upAtDate)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "保存排课课程成功" };
            DateTime adDate = DateTime.Parse(upAtDate);
            rsg = _courseWork.DropCourseWork(id, adDate, userId,_wxConfig.TemplateId);
            return Json(rsg);
        }


        /// <summary>
        /// 删除排课
        /// </summary>
        /// <param name="id"></param>
        /// <param name="upAtDate"></param>
        /// <returns></returns>
        public IActionResult RemoveCourseWork(int id)
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "删除当前课程成功" };
            rsg = _courseWork.RemoveCourseWork(id, userId,_wxConfig.TemplateId);
            return Json(rsg);
        }

        /// <summary>
        /// 复制课程
        /// </summary>
        /// <param name="workIds"></param>
        /// <param name="workDate"></param>
        /// <returns></returns>
        public IActionResult CopyCourseWork(int[] workIds, DateTime? workDate = null) {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "复制课程成功" };
            rsg = _courseWork.CopyCourseWork(workIds, userId,_wxConfig.TemplateId, workDate);
            return Json(rsg);
        }


        /// <summary>
        /// 保存模考和实考点评
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveCommend(C_Course_Work vmodel)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "保存点评成功" };
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var model = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(it => it.Id == vmodel.Id).First();
            var courseTime = DateTime.Parse(model.AT_Date.ToString("yyyy-MM-dd") + " " + model.EndTime);
            if (DateTime.Now < courseTime)
            {
                rsg.code = 0;
                rsg.msg = "当前课程时间还未结束，你无法点评";
            }
            else
            {
                vmodel.CreateUid = userId;
                var result = _currencyService.DbAccess().Updateable<C_Course_Work>().SetColumns(it => new C_Course_Work { Comment = vmodel.Comment, Work_Stutas = 1,Score=vmodel.Score,Comment_Time=DateTime.Now }).Where(it => it.Id == vmodel.Id).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "点评成功";
                }
            }
            return Json(rsg);
        }

        /// <summary>
        /// 获取微信公众号accesstoken
        /// </summary>
        /// <returns></returns>
        public WXAcceSSToken GetwxToken() {
            WXAcceSSToken wxtokenModel = null;
            if (RedisLock.KeyExists("wxAccessToken", redisConfig.RedisCon))
            {
                wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", redisConfig.RedisCon);
                log.Info("得到缓存" + wxtokenModel.Access_Token);
            }
            else
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, redisConfig.RedisCon);
            }
            return wxtokenModel;
        }






        /// <summary>
        /// 获取排课计划列表
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startStr, string endStr, string userName, int subjectId, int projectId,int studyMode)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            var teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(t => t.User_Name.Equals(userName)).First();
            var students = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(t => t.Student_Name.Contains(userName)).ToList();
            List<int> classIds = new List<int>();
            if (students != null && students.Count > 0)
            {
                var studentids = students.Select(v => v.StudentUid).ToList();
                //排除退班的班级
                int contracStatus =(int)ConstraChild_Status.RetrunClassOk;
                classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child,C_Class>((c,cl)=>new object[] { JoinType.Left,c.ClassId==cl.ClassId}).Where(c => studentids.Contains(c.StudentUid) && c.ClassId > 0&&c.Contrac_Child_Status!= contracStatus).WhereIF(subjectId>0,(c,cl)=>cl.SubjectId==subjectId).Select(c => c.ClassId).ToList();
            }
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date)";
            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or charindex(@userName,contracU.Student_Name)>0) ";
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                    sql += " and (charindex(@userName,tc.User_Name)>0 or charindex(@userName,contracU.Student_Name)>0 or charindex(@userName,wk.ListeningName)>0 or charindex(@userName,cl.Class_Name)>0) ";
            }
            if (subjectId > 0) {
                sql += " and wk.SubjectId="+subjectId;
            }
            if (projectId > 0)
            {
                sql += " and wk.ProjectId=" + projectId;
            }
            if (studyMode > 0)
            {
                sql += " and wk.StudyMode=" + studyMode;
            }
            sql += " and wk.CampusId="+campusId;
            sql += ")";
            dynamic list = _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr, userName = userName })
            .Select("*").OrderBy("orginSql.CreateTime desc").ToList();
            if (list != null && list.Count > 0 && !string.IsNullOrEmpty(userName))
            {
                reg.totalRow = new totalRow();
                if (teacher != null)
                {
                    //统计老师课时，点评已完成才算课时
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                        .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(subjectId > 0, it => it.ProjectId == projectId)
                        .Where(it => it.TeacherUid == teacher.User_ID && it.StudyMode != 3 && it.StudyMode != 7 && it.StudyMode != 5 && it.StudyMode != 6 && it.Work_Stutas==1&& it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                        .Sum(it => it.CourseTime);
                }
                if (students != null&& students.Count==1)
                {
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(projectId > 0, it => it.ProjectId == projectId)
                    .Where(it => (it.StudentUid == students[0].StudentUid||classIds.Contains(it.ClasssId)) && it.StudyMode != 3 && it.StudyMode != 7 && it.StudyMode != 4 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
                }
            }
            reg.data = list;
            reg.code = 0;
            reg.msg = "获取成功";
            return Json(reg);
        }


        public IActionResult QueryWorkSource2(string startStr, string endStr, string userName, int subjectId, int projectId,int studyMode, int page = 1, int limit = 10)
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult reg = new ResResult();
            var teacher = _currencyService.DbAccess().Queryable<sys_user>().Where(t => t.User_Name.Equals(userName)).First();
            var students = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(t => t.Student_Name.Contains(userName)).ToList();
            List<int> classIds = new List<int>();
            if (students != null&& students.Count>0)
            {
                var studentids = students.Select(v => v.StudentUid).ToList();
                //排除退班的班级
                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                classIds = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((c, cl) => new object[] { JoinType.Left, c.ClassId == cl.ClassId }).Where(c => studentids.Contains(c.StudentUid) && c.ClassId > 0 && c.Contrac_Child_Status != contracStatus).WhereIF(subjectId > 0, (c, cl) => cl.SubjectId == subjectId).Select(c => c.ClassId).ToList();
            }
            string sql = @"(select wk.*,contracU.Student_Name,ta.User_Name as TaUserName,tc.User_Name as TeacherName,rm.RoomName from C_Course_Work wk  left join C_Contrac_User contracU on wk.StudentUid=contracU.StudentUid
                left join C_Class cl on wk.ClasssId=cl.ClassId  left join Sys_User tc on wk.TeacherUid=tc.User_ID  left join Sys_User ta on wk.TA_Uid=ta.User_ID
                left join C_Room rm on wk.RoomId=rm.Id where wk.AT_Date>=CAST(@startStr AS date) AND wk.AT_Date<CAST(@endStr AS date)";
            if (classIds != null && classIds.Count > 0)
            {
                sql += " and (wk.ClasssId in(" + string.Join(",", classIds) + ") or charindex(@userName,contracU.Student_Name)>0) ";
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                    sql += " and (charindex(@userName,tc.User_Name)>0 or charindex(@userName,contracU.Student_Name)>0 or charindex(@userName,wk.ListeningName)>0 or charindex(@userName,cl.Class_Name)>0) ";
            }
            if (subjectId > 0)
            {
                sql += " and wk.SubjectId=" + subjectId;
            }
            if (projectId > 0)
            {
                sql += " and wk.ProjectId=" + projectId;
            }
            if (studyMode > 0)
            {
                sql += " and wk.StudyMode=" + studyMode;
            }
            sql += " and wk.CampusId=" + campusId;
            sql += ")";
            int total = 0;
            PageList<CourseWorkModel> pageModel = new PageList<CourseWorkModel>();
            List<CourseWorkModel> list= _currencyService.DbAccess().Queryable(sql, "orginSql")
            .AddParameters(new { startStr = startStr, endStr = endStr, userName = userName })
            .Select<CourseWorkModel>().OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            if (list != null && list.Count > 0 && !string.IsNullOrEmpty(userName))
            {
                reg.totalRow = new totalRow();
                if (teacher != null)
                {
                    //统计老师课时，点评已完成才算课时
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                        .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(subjectId > 0, it => it.ProjectId == projectId)
                        .Where(it => it.TeacherUid == teacher.User_ID && it.StudyMode != 3 && it.StudyMode != 7&& it.StudyMode != 5 && it.StudyMode != 6 && it.Work_Stutas == 1 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                        .Sum(it => it.CourseTime);
                }
                if (students != null&& students.Count==1)
                {
                    reg.totalRow.totalCourseTime = _currencyService.DbAccess().Queryable<C_Course_Work>()
                    .WhereIF(subjectId > 0, it => it.SubjectId == subjectId).WhereIF(projectId > 0, it => it.ProjectId == projectId)
                    .Where(it => (it.StudentUid == students[0].StudentUid || classIds.Contains(it.ClasssId)) && it.StudyMode != 3 && it.StudyMode != 7 && it.StudyMode != 4 && it.AT_Date >= DateTime.Parse(startStr) && it.AT_Date < DateTime.Parse(endStr))
                    .Sum(it => it.CourseTime);
                }
            }
            reg.data = list;
            reg.code = 0;
            reg.count = total;
            reg.msg = "获取成功";
            return Json(reg);
        }




    }
}
