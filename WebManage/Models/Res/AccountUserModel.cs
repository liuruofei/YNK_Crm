using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class AccountUserModel
    {
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string Account_Name{ get;set;}

        /// <summary>
        /// token
        /// </summary>
        public string apptoken { get; set; }

        /// <summary>
        /// 绑定手机号
        /// </summary>
        public string Mobile { get; set; }
    }
}
