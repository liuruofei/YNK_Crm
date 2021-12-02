using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ADT.Common;
using ADT.Models;
using ADT.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SqlSugar;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    public class BaseController : Controller
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string MenuID { get; set; } = string.Empty;
        /// <summary>
        /// 是否执行权限逻辑
        /// </summary>
        public bool IsExecutePowerLogic { get; set; } = true;
        /// <summary>
        /// 帐户 信息 对象
        /// </summary>
        public Sys_Account account = new Sys_Account();

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init() { }
        public BaseController()
        {
            this.Init();
        }
        /// <summary>
        /// Action 执行之前 发生
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsExecutePowerLogic || MenuID == "Home")
            {
                Console.WriteLine(filterContext.HttpContext.Request.Path.Value);
                var token = string.Empty;
                var cktoken = filterContext.HttpContext.Request.Cookies["authtoken"];
                var attoken = filterContext.HttpContext.Request.Headers["authtoken"].ToString();
                if (!string.IsNullOrEmpty(cktoken))
                {
                    //Console.WriteLine(cktoken);
                    token = cktoken;
                }
                if (!string.IsNullOrEmpty(attoken))
                    token = attoken;
                if (!string.IsNullOrEmpty(token))
                {
                    var userid = string.Empty;
                    var handers = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", ""));
                    foreach (var item in handers.Claims)
                    {
                        if (item.Type == "ID")
                            userid = item.Value;
                        //Console.WriteLine(item.Type);
                        //Console.WriteLine(item.Value);
                    }
                    //if (!string.IsNullOrEmpty(Convert.ToString(CacheHelper.Get(userid))))
                    //{
                    //    CacheHelper.Set(userid, "1", DateTime.Now.AddMinutes(120));
                    using (var db = SqlSugarHelper.GetInstance())
                    {
                        var model = db.Queryable<sys_user, sys_userrole>((u, ur) => new object[] {
                        JoinType.Inner, u.User_ID == ur.UserRole_UserID }
                        ).Where((u) => u.User_ID == userid)
                        .Select((u, ur) => new { u.User_ID, u.User_Name, ur.UserRole_RoleID }).First();
                        if (model != null)
                        {
                            account.RoleID = model.UserRole_RoleID;
                            account.UserID = model.User_ID;
                            account.UserName = model.User_Name;
                            account.IsSuperManage = model.UserRole_RoleID.ToStr().ToLower() == AppConfig.Admin_RoleID;
                            filterContext.RouteData.Values.Add("account", account);
                            base.OnActionExecuting(filterContext);
                        }
                        else
                        {
                            throw new MessageBox("请重新登陆", EMsgStatus.无效权限401);
                        }
                    }
                }
                else
                {
                    throw new MessageBox("请重新登陆", EMsgStatus.无效权限401);
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }

        /// <summary>
        /// Action 执行之后 发生
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (IsExecutePowerLogic && account != null)
            {
                this.PowerLogic(filterContext);
            }
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// 权限逻辑
        /// </summary>
        /// <param name="context"></param>
        private void PowerLogic(ActionExecutedContext context)
        {
            if (MenuID != "Home")
            {
                //Console.WriteLine(context.HttpContext.Request.Headers["X-Requested-With"].ToString());
                if (!(context.HttpContext.Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest"))
                {
                    using (var db = SqlSugarHelper.GetInstance())
                    {
                        var _RouteValues = context.ActionDescriptor.RouteValues;
                        var _Area = _RouteValues["area"];
                        var _Controller = _RouteValues["controller"];
                        var _Action = _RouteValues["action"];

                        var _func_list = db.Queryable<sys_function>().OrderBy("Function_Num asc").ToList();
                        var _power_list = new Dictionary<string, object>();
                        //这里得判断一下是否是查找带回调用页面
                        string findback = context.HttpContext.Request.Query["findback"];

                        if (string.IsNullOrEmpty(findback))
                        {
                            //dynamic model = new ExpandoObject();
                            if (string.IsNullOrEmpty(MenuID))
                            {
                                throw new MessageBox("区域(" + _Area + "),控制器(" + _Controller + "):的程序中缺少菜单ID");
                            }

                            var _Menu = db.Queryable<sys_menu>().First(p => p.Menu_Num == MenuID);
                            //if (!_Menu.Menu_Url.ToStr().StartsWith("/" + _Area + "/" + _Controller + "/"))
                            //{
                            //    throw new MessageBox("区域(" + _Area + "),控制器(" + _Controller + "):的程序中缺少菜单ID与该页面不匹配");
                            //}


                            if (!account.IsSuperManage)
                            {
                                var _role_menu_func_list = db.Queryable<sys_rolemenufunction>().ToList();
                                _power_list = new Dictionary<string, object>();
                                _func_list.ForEach(item =>
                                {
                                    var ispower = _role_menu_func_list.FindAll(x =>
                                        x.RoleMenuFunction_RoleID == account.RoleID &&
                                        x.RoleMenuFunction_MenuID == _Menu.Menu_ID &&
                                        x.RoleMenuFunction_FunctionID == item.Function_ID);

                                    _power_list.Add(item.Function_ByName, (ispower.Count > 0));

                                });
                            }
                            else
                            {
                                var _menu_func_list = db.Queryable<sys_menufunction>().ToList();
                                _func_list.ForEach(item =>
                                {
                                    //_power_list.Add(item.Function_ByName, true);
                                    var ispower = _menu_func_list.FindAll(x => x.MenuFunction_MenuID == _Menu.Menu_ID && x.MenuFunction_FunctionID == item.Function_ID);
                                    if (ispower.Count > 0)
                                        _power_list.Add(item.Function_ByName, true);
                                    else
                                        _power_list.Add(item.Function_ByName, false);
                                });
                                _power_list["Audit"] = true;
                                _power_list["AddContract"] = true;
                            }
                        }
                        else
                        {
                            _power_list = new Dictionary<string, object>();
                            _func_list.ForEach(item =>
                            {
                                _power_list.Add(item.Function_ByName, false);
                            });
                            _power_list["Have"] = true;
                            _power_list["Search"] = true;
                        }
                        this.ViewData["PowerModel"] = _power_list.SerializeObject();
                    }
                }
            }
        }
        /// <summary>
        /// 表数据转换为EXCEL
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columns">表头字段</param>
        /// <param name="pccolumns">排除字段</param>
        /// <returns></returns>
        [NonAction]
        public virtual byte[] DBToExcel(DataTable dt, string[] columns, string[] pccolumns = null)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            //填充表头
            IRow dataRow = sheet.CreateRow(0);
            for (int i = 0; i < columns.Length; i++)
            {
                dataRow.CreateCell(i).SetCellValue(columns[i]);
            }
            //填充内容
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataRow = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if ((pccolumns == null || !pccolumns.Contains(dt.Columns[j].ColumnName)) && dt.Columns[j].ColumnName != "RowIndex")
                    {
                        dataRow.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                    }
                }
            }
            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                return ms.ToArray();
            }
        }
    }
}
