using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ADT.Models;
using ADT.Repository.IRepository;
using SqlSugar;

namespace ADT.Repository
{
    public class Sys_MenuRepository : BaseRepository<sys_menu>, ISys_MenuRepository
    {
        /// <summary>
        /// 获取菜单和功能树
        /// </summary>
        /// <returns></returns>
        public List<sys_menulistfunc> GetMenuAndFunctionTree(string RoleID = "")
        {
            //菜单功能查询

            if (!string.IsNullOrEmpty(RoleID))
            {
                //角色功能查询
                using (var db = SqlSugarHelper.GetInstance())
                {
                    return db.Queryable<sys_menu, sys_rolemenufunction>((m, rmf) => new object[] { JoinType.Left, m.Menu_ID == rmf.RoleMenuFunction_MenuID }).OrderBy(p => p.Menu_Num, SqlSugar.OrderByType.Asc).Select<sys_menulistfunc>(@"Menu_Name+'('+Menu_Num+')' name,Menu_ID id,Menu_ParentID pId,Menu_Num num,Menu_Url ur,'false' checked,null tag,'true' chkDisabled,
case when  Menu_ParentID is not null then 'false' else 'true' end openname").ToList();
                }
            }
            using (var db = SqlSugarHelper.GetInstance())
            {
                return db.Queryable<sys_menu>().OrderBy(p => p.Menu_Num, SqlSugar.OrderByType.Asc).Select<sys_menulistfunc>(@"Menu_Name+'('+Menu_Num+')' name,Menu_ID id,Menu_ParentID pId,Menu_Num num,Menu_Url ur,'false' checkeds,null tag,'true' chkDisabled,
case when  Menu_ParentID is not null then 'false' else 'true' end openname").ToList();
            }
        }

        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        public List<sys_menulist> GetMenuPageList(Expression<Func<sys_menu, bool>> predicate, int page, int row, string orderBy, ref int totalNumber)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<sys_menu, sys_menu>((m, m1) => new object[] { JoinType.Left, m.Menu_ParentID == m1.Menu_ID });
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.Select((m, m1) => new sys_menulist { Menu_Url = m.Menu_Url, Menu_Num = m.Menu_Num, Parent_Name = m1.Menu_Name, Menu_Name = m.Menu_Name, Menu_ParentID = m.Menu_ParentID, Menu_IsShow = m.Menu_IsShow, Menu_CreateTime = m.Menu_CreateTime, Menu_Icon = m.Menu_Icon, Menu_ID = m.Menu_ID }).ToPageList(page, row, ref totalNumber);
            }
        }
    }
}
