using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台用户权限绑定表
    ///</summary>
    public partial class sys_userrole
    {
        public sys_userrole()
        {


        }
        /// <summary>
        /// Desc:ID
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string UserRole_ID { get; set; }

        /// <summary>
        /// Desc:用户ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string UserRole_UserID { get; set; }

        /// <summary>
        /// Desc:角色ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string UserRole_RoleID { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:DateTime.Now
        /// Nullable:True
        /// </summary>           
        public DateTime? UserRole_CreateTime { get; set; }

    }
}
