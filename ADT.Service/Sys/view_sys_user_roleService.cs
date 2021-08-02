using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class view_sys_user_roleService : BaseService<view_sys_user_role>, Iview_sys_user_roleService
    {
        private Iview_sys_user_roleRepository _view_sys_user_roleRepositoryS;
        public view_sys_user_roleService(Iview_sys_user_roleRepository view_sys_user_roleRepositoryS) : base(view_sys_user_roleRepositoryS)
        {
            _view_sys_user_roleRepositoryS = view_sys_user_roleRepositoryS;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<view_sys_user_role> GetPageList(Expression<Func<view_sys_user_role, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<view_sys_user_role> pageList = new PageList<view_sys_user_role>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _view_sys_user_roleRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
