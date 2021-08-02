using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class Sys_NumberService : BaseService<sys_number>, ISys_NumberService
    {
        private ISys_NumberRepository _Sys_NumberRepositoryS;
        public Sys_NumberService(ISys_NumberRepository Sys_NumberRepositoryS) : base(Sys_NumberRepositoryS)
        {
            _Sys_NumberRepositoryS = Sys_NumberRepositoryS;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_number> GetPageList(Expression<Func<sys_number, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_number> pageList = new PageList<sys_number>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_NumberRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
