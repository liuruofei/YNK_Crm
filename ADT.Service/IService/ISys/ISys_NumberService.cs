using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;

namespace ADT.Service.IService
{
    public interface ISys_NumberService : IBaseService<sys_number>
    {
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        PageList<sys_number> GetPageList(Expression<Func<sys_number, bool>> predicate, int page, int pageSize, string orderBy);
    }
}
