using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Models.Sys;

namespace ADT.Service.IService
{
    public interface ISys_MenuService : IBaseService<sys_menu>
    {
        /// <summary>
        /// 左侧菜单
        /// </summary>
        /// <returns></returns>
        string GetSysMenu(Sys_Account account);
        #region  系统管理》菜单功能，角色功能  树的json处理

        /// <summary>
        /// 获取菜单和功能树
        /// </summary>
        /// <returns></returns>
        List<sys_menulistfunc> GetMenuAndFunctionTree();
        /// <summary>
        /// 获取角色对应的功能树
        /// </summary>
        /// <returns></returns>
        List<MenuRoleList> GetRoleMenuFunctionTree(string roleid);
        #endregion 系统管理》菜单功能，角色功能  树的json处理
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        PageList<sys_menu> GetPageList(Expression<Func<sys_menu, bool>> predicate, int page, int pageSize, string orderBy);
        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <returns></returns>
        PageList<sys_menulist> GetMenuPageList(Expression<Func<sys_menu, bool>> predicate, int page, int row, string orderBy);
    }
}
