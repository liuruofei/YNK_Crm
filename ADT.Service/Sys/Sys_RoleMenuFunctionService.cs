using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class Sys_RoleMenuFunctionService : BaseService<sys_rolemenufunction>, ISys_RoleMenuFunctionService
    {
        private ISys_RoleMenuFunctionRepository _Sys_RoleMenuFunctionRepositoryS;
        public Sys_RoleMenuFunctionService(ISys_RoleMenuFunctionRepository Sys_RoleMenuFunctionRepositoryS) : base(Sys_RoleMenuFunctionRepositoryS)
        {
            _Sys_RoleMenuFunctionRepositoryS = Sys_RoleMenuFunctionRepositoryS;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_rolemenufunction> GetPageList(Expression<Func<sys_rolemenufunction, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_rolemenufunction> pageList = new PageList<sys_rolemenufunction>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_RoleMenuFunctionRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
