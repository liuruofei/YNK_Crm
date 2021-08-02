using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class Sys_RoleService : BaseService<sys_role>, ISys_RoleService
    {
        private ISys_RoleRepository _Sys_RoleRepositoryS;
        public Sys_RoleService(ISys_RoleRepository Sys_RoleRepositoryS) : base(Sys_RoleRepositoryS)
        {
            _Sys_RoleRepositoryS = Sys_RoleRepositoryS;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_role> GetPageList(Expression<Func<sys_role, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_role> pageList = new PageList<sys_role>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_RoleRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
