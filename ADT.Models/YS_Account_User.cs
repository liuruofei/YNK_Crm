using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///后台配置授权登录账号数据
    ///</summary>
    public partial class YS_Account_User
    {
        public YS_Account_User()
        {


        }
        /// <summary>
        /// 登录用户uid
        /// </summary>
        public int Account_Uid { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string Account_Name { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Account_Pwd { get; set; }

        /// <summary>
        /// 展示密码
        /// </summary>
        public string Pwd_Display { get; set; }

        /// <summary>
        /// 绑定的手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 是否已绑定
        /// </summary>
        public int IsUseed { get; set; }
        /// <summary>
        /// 登录服务器ip
        /// </summary>
        public string Login_Ip { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime Login_Time { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 行状态
        /// </summary>
        public int Status { get; set; }
    }
}
