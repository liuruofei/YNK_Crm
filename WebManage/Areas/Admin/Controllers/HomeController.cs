using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WebManage.Models;
using WebManage.Models.Res;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private ISys_MenuService _sys_MenuService;
        private ISys_RoleMenuFunctionService _Sys_RoleMenuFunctionService;
        private ICurrencyService _currencyService;
        public HomeController(ICurrencyService currencyService, ISys_MenuService sys_MenuService, ISys_RoleMenuFunctionService Sys_RoleMenuFunctionService)
        {
            _currencyService = currencyService;
            _sys_MenuService = sys_MenuService;
            _Sys_RoleMenuFunctionService = Sys_RoleMenuFunctionService;
        }

        protected override void Init()
        {
            this.MenuID = "Home";
            base.Init();
        }
        public IActionResult Index()
        {
            ViewBag.UserName = account.UserName;
            return View();
        }

        public IActionResult Welcome()
        {
            //System.Threading.Thread.Sleep(3 * 1000);
            return View();
        }

        public IActionResult MyTask()
        {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            string[] adminRoleName = new string[] { "校长", "教学校长", "超级管理员","督学校长" };
            var userRole = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.User_ID == userId).Select<sys_role>((u, ur, r) => r).First();
            ///判断是否是管理员
            if (userRole != null && adminRoleName.Contains(userRole.Role_Name))
                return View("MakeTask");
            else
                return View("MyTask");
        }

        public IActionResult MakeTask()
        {
            return View();
        }
        public IActionResult AddTask(int ID,string dataStr)
        {
            ViewBag.ID = ID;
            ViewBag.DataStr = dataStr;
            return View();
        }

        public IActionResult EditTask(int ID)
        {
            ViewBag.ID = ID;
            return View();
        }


        public IActionResult FindTask(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_TeacherTask>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 查询老师
        /// </summary>
        /// <returns></returns>
        public IActionResult QuerySysUser()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<sys_user> list = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID })
                .Where((u, ur, r) => u.CampusId == Convert.ToInt32(campusId) && u.User_IsDelete == 2).ToList();
            list.Add(new sys_user { User_ID = "", User_Name = "-请选择用户-", User_CreateTime = DateTime.Now });
            rsg.data = list.OrderByDescending(n => n.User_CreateTime).ToList();
            return Json(rsg);
        }


        //保存任务
        public IActionResult SaveTask(TeacherTaskInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {

                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.Title))
                {
                    rsg.msg = "任务标题不能为空";
                    return Json(rsg);
                }
                if (vmodel.Id < 1)
                {
                    if (vmodel.UidArry==null||(vmodel.UidArry!=null&& vmodel.UidArry.Count<0))
                    {
                        rsg.msg = "请选择相关用户";
                        return Json(rsg);
                    }
                    foreach (var cr in vmodel.UidArry) {
                        C_TeacherTask task = new C_TeacherTask();
                        task.CreateUid = userId;
                        task.CreatTime = DateTime.Now;
                        task.TaskStatus = 0;
                        task.SysUid = cr;
                        task.EndDate = vmodel.EndDate;
                        task.EndTime = vmodel.EndTime;
                        task.StartDate = vmodel.StartDate;
                        task.StartTime = vmodel.StartTime;
                        task.Title = vmodel.Title;
                        task.TaskComment = vmodel.TaskComment;
                        task.TaskRemarks = vmodel.TaskRemarks;
                        rsg.code = _currencyService.DbAccess().Insertable<C_TeacherTask>(task).ExecuteCommand();
                    }
            
                }
                else {
                    var model = _currencyService.DbAccess().Queryable<C_TeacherTask>().Where(c => c.Id == vmodel.Id).First();
                    model.EndDate = vmodel.EndDate;
                    model.EndTime = vmodel.EndTime;
                    model.StartDate = vmodel.StartDate;
                    model.StartTime = vmodel.StartTime;
                    model.Title = vmodel.Title;
                    model.TaskComment = vmodel.TaskComment;
                    model.TaskStatus = vmodel.TaskStatus;
                    model.SysUid = vmodel.SysUid;
                    model.TaskRemarks = vmodel.TaskRemarks;
                    rsg.code = _currencyService.DbAccess().Updateable<C_TeacherTask>(model).ExecuteCommand();
                }
                if (rsg.code > 0)
                {
                    rsg.code = 200;
                    rsg.msg = "保存任务成功";
                }
               
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }


        public IActionResult RemoveTask(int id) {
            ResResult rsg = new ResResult() { code = 0, msg = "删除失败" };
            try
            {
                rsg.code = _currencyService.DbAccess().Deleteable<C_TeacherTask>().Where(tk => tk.Id == id).ExecuteCommand();
                rsg.code = rsg.code > 0 ? 200 : 0;
                rsg.msg = rsg.code > 0 ? "删除成功" : rsg.msg;
            }
            catch (Exception er) {
                rsg.code = 0;
                rsg.msg = "删除失败!原因:"+er.Message;
            
            }
            return Json(rsg);
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="startStr"></param>
        /// <param name="endStr"></param>
        /// <returns></returns>
        public IActionResult QueryTaskSource(string startStr, string endStr)
        {
            ResResult reg = new ResResult() { code = 200, msg = "获取成功" };
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            string[] adminRoleName = new string[] { "校长", "教学校长", "超级管理员" };
            var userRole = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new Object[] { JoinType.Left, u.User_ID == ur.UserRole_UserID, JoinType.Left, ur.UserRole_RoleID == r.Role_ID })
                 .Where((u, ur, r) => u.User_ID == userId).Select<sys_role>((u, ur, r) => r).First();
            var listTask = _currencyService.DbAccess().Queryable<C_TeacherTask, sys_user>((tk, u) => new Object[] { JoinType.Left, tk.SysUid == u.User_ID })
                .Where((tk, u) => tk.StartDate >= DateTime.Parse(startStr) && tk.EndDate <= DateTime.Parse(endStr)).WhereIF(!adminRoleName.Contains(userRole.Role_Name), (tk, u) => tk.SysUid.Equals(userId))
                .Select<TeacherTaskModel>((tk, u) => new TeacherTaskModel
                {
                    Id = tk.Id,
                    SysUid = tk.SysUid,
                    Title = tk.Title,
                    User_Name = u.User_Name,
                    StartDate=tk.StartDate,
                    EndDate=tk.EndDate,
                    StartTime = tk.StartTime,
                    EndTime = tk.EndTime,
                    TaskStatus = tk.TaskStatus,
                    TaskComment = tk.TaskComment,
                    TaskRemarks=tk.TaskRemarks
                }).ToList();
            reg.data = listTask;
            return Json(reg);
        }



        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenu()
        {
            var menu_list = _sys_MenuService.FindListByClause(p => p.Menu_IsShow == 1, "Menu_Num asc");
            List<MenuList> lists = new List<MenuList>();
            var rolemenu = new List<sys_rolemenufunction>();
            if (!account.IsSuperManage)
                rolemenu = (List<sys_rolemenufunction>)_Sys_RoleMenuFunctionService.FindListByClause(p => p.RoleMenuFunction_RoleID == account.RoleID && p.RoleMenuFunction_FunctionID == "C9518758-B2E1-4F51-B517-5282E273889C", "");
            foreach (var item in menu_list.Where(p => string.IsNullOrEmpty(p.Menu_ParentID)))
            {
                if (!account.IsSuperManage)
                {
                    if (rolemenu.Find(p => p.RoleMenuFunction_MenuID == item.Menu_ID) == null)
                    {
                        continue;
                    }
                }
                MenuList menuList = new MenuList();
                menuList.name = item.Menu_Name;
                menuList.icon = item.Menu_Icon;
                menuList.url = item.Menu_Url;
                menuList.subMenus = BindMenu(menu_list, rolemenu, item.Menu_ID);
                lists.Add(menuList);
            }
            //lists = menu_list.Where(p => string.IsNullOrEmpty(p.Menu_ParentID)).Select(p => new MenuList { name = p.Menu_Name, icon = p.Menu_Icon, url = p.Menu_Url, subMenus = BindMenu() });
            return Json(lists);
        }

        public List<MenuList> BindMenu(IEnumerable<sys_menu> sys_Menus, List<sys_rolemenufunction> rolemenu, string parentId)
        {
            List<MenuList> lists = new List<MenuList>();
            foreach (var item in sys_Menus.Where(p => p.Menu_ParentID == parentId))
            {
                if (!account.IsSuperManage)
                {
                    if (rolemenu.Find(p => p.RoleMenuFunction_MenuID == item.Menu_ID) == null)
                    {
                        continue;
                    }
                }
                MenuList menuList = new MenuList();
                menuList.name = item.Menu_Name;
                menuList.icon = item.Menu_Icon;
                menuList.url = item.Menu_Url;
                menuList.subMenus = BindMenu(sys_Menus, rolemenu, item.Menu_ID);
                lists.Add(menuList);
            }
            return lists;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TplTheme()
        {
            return View();
        }
    }
}
