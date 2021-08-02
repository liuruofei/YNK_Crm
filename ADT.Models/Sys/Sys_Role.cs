using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台权限表
    ///</summary>
    public partial class sys_role
    {
        public sys_role()
        {
        }

        /// <summary>
        /// Desc:ID
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Role_ID { get; set; }

        /// <summary>
        /// Desc:编号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Role_Num { get; set; }

        /// <summary>
        /// Desc:角色名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Role_Name { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Role_Remark { get; set; }

        /// <summary>
        /// Desc:是否可删除（1：是 2：否）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? Role_IsDelete { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:DateTime.Now
        /// Nullable:True
        /// </summary>           
        public DateTime? Role_CreateTime { get; set; }

    }
}
