using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class YS_User_EnterService : BaseService<YS_User_Enter>, IYS_User_EnterService
    {
        private IYS_User_EnterRepository _ys_User_EnterRepository;
        public YS_User_EnterService(IYS_User_EnterRepository ys_User_EnterRepository) : base(ys_User_EnterRepository)
        {
            _ys_User_EnterRepository = ys_User_EnterRepository;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<YS_User_Enter> GetPageList(Expression<Func<YS_User_Enter, bool>> predicate, int page, int row, string         orderBy)
        {
            PageList<YS_User_Enter> pageList = new PageList<YS_User_Enter>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _ys_User_EnterRepository.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
