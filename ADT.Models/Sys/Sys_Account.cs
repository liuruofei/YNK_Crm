using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class Sys_Account
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 当前登录人 是否是 超级管理员
        /// </summary>
        public bool IsSuperManage { get; set; }

        public Sys_Account()
        {
            this.IsSuperManage = false;
        }
    }
}
