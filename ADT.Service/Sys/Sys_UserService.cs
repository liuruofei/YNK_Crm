using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;
using SqlSugar;
using System.Data;

namespace ADT.Service
{
    public class Sys_UserService : BaseService<sys_user>, ISys_UserService
    {
        private ISys_UserRepository _Sys_UserRepositoryS;
        private ISys_UserRoleRepository _Sys_UserRoleRepositoryS;
        public Sys_UserService(ISys_UserRepository Sys_UserRepositoryS, ISys_UserRoleRepository Sys_UserRoleRepositoryS) : base(Sys_UserRepositoryS)
        {
            _Sys_UserRepositoryS = Sys_UserRepositoryS;
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
        public PageList<sys_user> GetPageList(Expression<Func<sys_user, bool>> predicate, int page, int row, string orderBy)
        {
            PageList<sys_user> pageList = new PageList<sys_user>();
            pageList.code = 0;
            pageList.msg = "获取成功";
            int totalNumber = 0;
            pageList.data = _Sys_UserRepositoryS.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
            pageList.count = totalNumber;
            return pageList;
        }
        /// <summary>
        /// 修改用户并修改关联角色
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool UpdateUsers(view_sys_user_role u)
        {
            var model = new sys_user();
            model.User_ID = u.User_ID;
            model.User_LoginName = u.User_LoginName;
            model.User_Name = u.User_Name;
            model.User_Pwd = u.User_Pwd;
            model.User_Email = u.User_Email;
            model.User_IsDelete = u.User_IsDelete;
            model.CampusId = u.CampusId;
            var result = _Sys_UserRepositoryS.Update(model, p => new { p.User_LoginName, p.User_Name, p.User_Pwd, p.User_Email, p.User_CreateTime, p.User_IsDelete,p.CampusId}, true);
            if (result)
            {
                var rmodel = new sys_userrole();
                rmodel.UserRole_ID = u.UserRole_ID;
                rmodel.UserRole_RoleID = u.Role_ID;
                rmodel.UserRole_UserID = u.User_ID;
                _Sys_UserRoleRepositoryS.Update(rmodel, p => new { p.UserRole_RoleID, p.UserRole_UserID }, true);
            }
            return result;
        }
        /// <summary>
        /// 创建用户并关联角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertUsers(view_sys_user_role model)
        {
            var umodel = new sys_user();
            umodel.User_ID = Guid.NewGuid().ToString();
            umodel.User_CreateTime = DateTime.Now;
            umodel.User_Email = model.User_Email;
            umodel.User_IsDelete = model.User_IsDelete;
            umodel.User_LoginName = model.User_LoginName;
            umodel.User_Name = model.User_Name;
            umodel.User_Pwd = model.User_Pwd;
            umodel.CampusId = model.CampusId;
            var result = _Sys_UserRepositoryS.Insert(umodel);
            if (result > 0)
            {
                var urmodel = new sys_userrole();
                urmodel.UserRole_CreateTime = DateTime.Now;
                urmodel.UserRole_ID = Guid.NewGuid().ToString();
                urmodel.UserRole_RoleID = model.Role_ID;
                urmodel.UserRole_UserID = umodel.User_ID;
                result = _Sys_UserRoleRepositoryS.Insert(urmodel);
                if (result <= 0)
                    _Sys_UserRepositoryS.Delete(umodel);
            }
            return result > 0;
        }
    }
}
