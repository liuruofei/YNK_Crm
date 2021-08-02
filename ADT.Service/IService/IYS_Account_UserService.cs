using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;

namespace ADT.Service.IService
{
    public interface IYS_Account_UserService : IBaseService<YS_Account_User>
    {
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        PageList<YS_Account_User> GetPageList(Expression<Func<YS_Account_User, bool>> predicate, int page, int pageSize, string orderBy);
    }
}
