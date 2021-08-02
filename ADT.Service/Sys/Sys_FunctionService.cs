using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class Sys_FunctionService : BaseService<sys_function>, ISys_FunctionService
    {
        private ISys_FunctionRepository _Sys_FunctionRepositoryS;
        public Sys_FunctionService(ISys_FunctionRepository Sys_FunctionRepositoryS) : base(Sys_FunctionRepositoryS)
        {
            _Sys_FunctionRepositoryS = Sys_FunctionRepositoryS;
        }

        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_function> GetPageList(Expression<Func<sys_function, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_function> pageList = new PageList<sys_function>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_FunctionRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
