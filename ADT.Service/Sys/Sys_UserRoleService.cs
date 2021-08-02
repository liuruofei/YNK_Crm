using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class Sys_UserRoleService : BaseService<sys_userrole>, ISys_UserRoleService
    {
        private ISys_UserRoleRepository _Sys_UserRoleRepositoryS;
        public Sys_UserRoleService(ISys_UserRoleRepository Sys_UserRoleRepositoryS) : base(Sys_UserRoleRepositoryS)
        {
            _Sys_UserRoleRepositoryS = Sys_UserRoleRepositoryS;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<sys_userrole> GetPageList(Expression<Func<sys_userrole, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_userrole> pageList = new PageList<sys_userrole>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_UserRoleRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
