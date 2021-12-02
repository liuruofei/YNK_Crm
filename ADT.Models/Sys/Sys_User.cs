using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///管理后台用户表
    ///</summary>
  
    public partial class sys_user
    {
        public sys_user()
        {


        }
        /// <summary>
        /// Desc:ID
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string User_ID { get; set; }

        /// <summary>
        /// Desc:用户名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string User_Name { get; set; }

        /// <summary>
        /// Desc:登陆名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string User_LoginName { get; set; }

        /// <summary>
        /// Desc:登陆密码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string User_Pwd { get; set; }

        /// <summary>
        /// Desc:邮件
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string User_Email { get; set; }

        /// <summary>
        /// Desc:是否可删除（1：是 2：否）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? User_IsDelete { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:DateTime.Now
        /// Nullable:True
        /// </summary>           
        public DateTime? User_CreateTime { get; set; }


        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

    }
}
