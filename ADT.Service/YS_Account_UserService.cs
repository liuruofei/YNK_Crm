using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;

namespace ADT.Service
{
    public class YS_Account_UserService : BaseService<YS_Account_User>, IYS_Account_UserService
    {
        private IYS_Account_UserRepository _ys_Account_UserRepository;
        public YS_Account_UserService(IYS_Account_UserRepository ys_Account_UserRepository) : base(ys_Account_UserRepository)
        {
            _ys_Account_UserRepository = ys_Account_UserRepository;
        }
        /// <summary>
        /// 获取管理后台分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">煤业显示多少条</param>
        /// <param name="orderBy">排序条件</param>
        /// <returns></returns>
        public PageList<YS_Account_User> GetPageList(Expression<Func<YS_Account_User, bool>> predicate, int page, int row, string         orderBy)
        {
            PageList<YS_Account_User> pageList = new PageList<YS_Account_User>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _ys_Account_UserRepository.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
    }
}
