using ADT.Common;
using ADT.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Models;

namespace WebManage.Areas.Admin.Filter
{
    public class UsersRoleAuthFilterAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        private string _menu;
        /// <summary>
        /// 功能英文名
        /// </summary>
        private string[] _role;
        /// <summary>
        /// 初始化权限判断过滤器
        /// </summary>
        /// <param name="menu">菜单编号</param>
        /// <param name="role">功能英文名</param>
        public UsersRoleAuthFilterAttribute(string menu, FunctionEnum role)
        {
            _menu = menu;
            _role = role.ToString().Split(",");
        }
        /// <summary>
        /// 初始化权限判断过滤器
        /// </summary>
        /// <param name="menu">菜单编号</param>
        /// <param name="role">功能英文名</param>
        public UsersRoleAuthFilterAttribute(string menu, string role)
        {
            _menu = menu;
            _role = role.Split(",");
        }

        public UsersRoleAuthFilterAttribute() { }
        /// <summary>
        /// Action之前发生
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Sys_Account account = (Sys_Account)context.RouteData.Values["account"];
            if (account != null)
            {
                if (!account.IsSuperManage)
                {
                    var funcList = RegisterRoleMenu.funcList;
                    if (funcList != null && funcList.Count > 0)
                    {
                        var funcModel = funcList.Where(p => p.RoleMenuFunction_RoleID == account.RoleID && p.Menu_Num == _menu && _role.Contains(p.Function_ByName)).FirstOrDefault();
                        if (funcModel == null)
                        {
                            throw new MessageBox("您没有该功能的权限", EMsgStatus.无效权限403);
                        }
                    }
                    else
                    {
                        throw new MessageBox("您没有该功能的权限", EMsgStatus.无效权限403);
                    }
                }
            }
            else
            {
                throw new MessageBox("登陆超时，请重新登陆", EMsgStatus.无效权限401);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
