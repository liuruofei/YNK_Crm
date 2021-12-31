using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class ClueUserController : BaseController
    {

        private ICurrencyService _currencyService;
        private IC_ClueUserService _clueUserService;
        public ClueUserController(ICurrencyService currencyService, IC_ClueUserService clueUserService)
        {
            _currencyService = currencyService;
            _clueUserService = clueUserService;
        }
        protected override void Init()
        {
            this.MenuID = "A-102";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("A-102", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("A-102", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            string roleName = "";
            var role = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID }).Where((u, ur, r) => u.User_ID == userId&&r.Role_Name=="顾问").Select<sys_role>((u, ur, r)=>r).First();
            if (role != null)
            {
                roleName = role.Role_Name;
            }
            PageList<ResClueUserModel> pageModel = new PageList<ResClueUserModel>();
            var list = _currencyService.DbAccess().Queryable("C_ClueUser", "clue").AddJoinInfo(@"Sys_User", "cc", "clue.CC_Uid=cc.User_ID", SqlSugar.JoinType.Left).AddJoinInfo("C_Campus", "camp", "clue.CampusId=camp.CampusId", SqlSugar.JoinType.Left)
                .AddJoinInfo("C_ClueUser", "clueDefault", "clue.Defualt_ClueId=clueDefault.ClueId", SqlSugar.JoinType.Left).AddJoinInfo(@"Sys_User", "cc2", "clueDefault.CC_Uid=cc2.User_ID", SqlSugar.JoinType.Left)
                .Where("clue.CampusId=@campusId and clue.Status<1 and isnull(clue.Contrac_StudentUid,0)<1").AddParameters(new { campusId = campusId })
                .WhereIF(!string.IsNullOrEmpty(title), "clue.Student_Name=@title").AddParameters(new { title = title })
                .WhereIF(!string.IsNullOrEmpty(roleName), "clue.CC_Uid=@CCid").AddParameters(new { CCid = userId })
                .Select<ResClueUserModel>(@"clue.*,cc.User_Name as CC_UserName,cc2.User_Name as Default_CC_UserName,(select Count(*) from C_ClueUser_Record record where record.ClueId=clue.ClueId) as Follow_Count,
                 camp.CampusName").OrderBy("clue.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("A-102", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("A-102", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("A-102", FunctionEnum.Audit)]
        public IActionResult SetAssign(int ID)
        {
            ViewBag.ID = ID;
            return View("SetAssign");
        }

        [UsersRoleAuthFilter("A-102", FunctionEnum.Edit)]
        public IActionResult OwinClue(int ID) {
            ViewBag.ID = ID;
            return View("OwinClue");
        }

        [UsersRoleAuthFilter("A-102", FunctionEnum.Add)]
        public IActionResult AddRecord(int ID)
        {
            ViewBag.ID = ID;
            return View("AddRecord");
        }


        /// <summary>
        /// 获取课程列表
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCourseList()
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Subject>().Where(n => n.Status < 1).ToList();
            return Json(rsg);
        }

        /// <summary>
        /// 获取课程列表
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCC()
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where((u, ur, r) => r.Role_Name.Contains("顾问")).Select((u, ur, r) => u).ToList();
            return Json(rsg);
        }




        [UsersRoleAuthFilter("A-102", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                ResClueUserModel detail = _currencyService.DbAccess().Queryable<C_ClueUser>().Where(f => f.ClueId == ID).Select<ResClueUserModel>(f => new ResClueUserModel()
                {
                    ClueId = f.ClueId,
                    Follow_Date = f.Follow_Date,
                    Follow_Plan = f.Follow_Plan,
                    Birthday = f.Birthday,
                    CC_Uid = f.CC_Uid,
                    ContracRate = f.ContracRate,
                    Elder_Email = f.Elder_Email,
                    Elder_Email2 = f.Elder_Email2,
                    Elder_Identity = f.Elder_Identity,
                    Elder_Identity2 = f.Elder_Identity2,
                    Elder_Wechat=f.Elder_Wechat,
                    Elder_Name = f.Elder_Name,
                    Elder_Name2 = f.Elder_Name2,
                    Elder_Phone = f.Elder_Phone,
                    Elder_Phone2 = f.Elder_Phone2,
                    Student_Email = f.Student_Email,
                    Sex = f.Sex,
                    Grade = f.Grade,
                    Soure = f.Soure,
                    Student_Name = f.Student_Name,
                    Student_Phone = f.Student_Phone,
                    Student_Wechat=f.Student_Wechat,
                    InSchool = f.InSchool,
                    Student_Status = f.Student_Status,
                    Is_Visit = f.Is_Visit,
                    More_Contacts = f.More_Contacts,
                    Recommend = f.Recommend,
                    Visit_Date = f.Visit_Date,
                    FirstTime = f.FirstTime,
                    CampusId = f.CampusId
                }).First();
                if (detail != null)
                {
                    detail.SubjectIds = _currencyService.DbAccess().Queryable<C_ClueUser_Subject>().Where(f => f.ClueId == detail.ClueId).Select(f => f.SubjectId).ToList();
                }
                list = detail;
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-102", "Add,Edit")]
        public IActionResult SaveInfo(ClueUserInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Student_Name))
                    return Json(new { code = 0, msg = "学生名称不能为空" });
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId= this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                vmodel.CreateUid = userId;
                vmodel.CampusId = Convert.ToInt32(campusId);
                var userModel = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID }).Where((u, ur, r) => u.User_ID == userId).Select((u, ur, r) => r).First();
                //如果是顾问自己添加则默认顾问
                if (userModel!=null&&userModel.Role_Name== "顾问"&& vmodel.ClueId<1) {
                    vmodel.CC_Uid = userId;
                }
                rsg = _clueUserService.SaveClueUser(vmodel);

            }
            return Json(rsg);
        }

        /// <summary>
        /// 转移线索
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveOwin(ClueUserInput vmodel) {
            ResResult rsg = new ResResult() { code = 0, msg = "转移失败" };
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Owin_CC_Uid))
                    return Json(new { code = 0, msg = "转移顾问不能为空" });
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                vmodel.CreateUid = userId;
                rsg = _clueUserService.OwinClue(vmodel);

            }
            return Json(rsg);

        }

        /// <summary>
        /// 指派顾问
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-102", "Audit")]
        public IActionResult SaveAssign(ClueUserInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.CC_Uid))
                    return Json(new { code = 0, msg = "请选择指派一个顾问" });
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var result = _currencyService.DbAccess().Updateable<C_ClueUser>().SetColumns(item =>
                  new C_ClueUser { CC_Uid = vmodel.CC_Uid, UpdateTime = DateTime.Now, UpdateUid = userId }).Where(item => item.ClueId == vmodel.ClueId).ExecuteCommand();
                if (result > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "指派成功";
                    ;
                }
            }
            return Json(rsg);
        }




        /// <summary>
        /// 保存追踪记录
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveClueRecord(ClueRecordInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                if (string.IsNullOrEmpty(vmodel.Follow_Content))
                    return Json(new { code = 0, msg = "跟踪内容不能为空" });
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                vmodel.CampusId = Convert.ToInt32(campusId);
                vmodel.CreateUid = userId;
                rsg = _clueUserService.SaveClueRecord(vmodel);
            }
            return Json(rsg);
        }

        /// <summary>
        /// 删除线索
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("A-102", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "删除失败" };
            if (Id < 1)
            {
                rsg.msg = "缺少参数";
            }
            else
            {
                rsg = _clueUserService.DeleteClueUser(Id);
            }
            return Json(rsg);
        }
    }
}
