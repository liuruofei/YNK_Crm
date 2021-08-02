using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ADT.Models;

namespace ADT.Repository.IRepository
{
    public interface ISys_MenuRepository : IBaseRepository<sys_menu>
    {
        /// <summary>
        /// 获取菜单和功能树
        /// </summary>
        /// <returns></returns>
        List<sys_menulistfunc> GetMenuAndFunctionTree(string RoleID = "");
        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        List<sys_menulist> GetMenuPageList(Expression<Func<sys_menu, bool>> predicate, int page, int row, string orderBy, ref int totalNumber);
    }
}
