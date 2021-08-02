using ADT.Models;
using ADT.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Areas.Admin.Filter
{
    public static class RegisterRoleMenu
    {
        private static List<view_sys_user_role_function> _funcList;
        public static List<view_sys_user_role_function> funcList
        {
            get
            {
                if (_funcList == null || _funcList.Count == 0)
                {
                    AddRoleMenu();
                }
                return _funcList;
            }
        }
        public static void AddRoleMenu()
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                _funcList = db.Queryable<view_sys_user_role_function>().ToList();
            }
        }
    }
}
