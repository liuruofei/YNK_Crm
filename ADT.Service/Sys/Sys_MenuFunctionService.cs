using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class Sys_MenuFunctionService : BaseService<sys_menufunction>, ISys_MenuFunctionService
    {
        private ISys_MenuFunctionRepository _Sys_MenuFunctionRepositoryS;
        public Sys_MenuFunctionService(ISys_MenuFunctionRepository Sys_MenuFunctionRepositoryS) : base(Sys_MenuFunctionRepositoryS)
        {
            _Sys_MenuFunctionRepositoryS = Sys_MenuFunctionRepositoryS;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_menufunction> GetPageList(Expression<Func<sys_menufunction, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_menufunction> pageList = new PageList<sys_menufunction>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_MenuFunctionRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
