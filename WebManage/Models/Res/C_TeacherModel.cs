using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_TeacherModel
    {


        public string TeacherUid { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string User_Name { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string User_LoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string User_Pwd { get; set; }



        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }
    }
}
