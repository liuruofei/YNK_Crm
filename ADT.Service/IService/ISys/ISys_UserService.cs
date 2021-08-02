using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using SqlSugar;
using System.Data;

namespace ADT.Service.IService
{
    public interface ISys_UserService : IBaseService<sys_user>
    {
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        PageList<sys_user> GetPageList(Expression<Func<sys_user, bool>> predicate, int page, int pageSize, string orderBy);
        /// <summary>
        /// 创建用户并生成关联角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool InsertUsers(view_sys_user_role model);
        /// <summary>
        /// 修改用户并修改关联角色
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        bool UpdateUsers(view_sys_user_role u);
    }
}
