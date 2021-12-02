using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    [SugarTable("view_user")]
    public class view_user
    {
        /// <summary>
        /// 用户uid
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Uid { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string User_AccountName { get; set; }
        /// <summary>
        /// 用户英文名称
        /// </summary>
        public string User_Ename { get; set; }
        /// <summary>
        /// 用户中文名称
        /// </summary>
        public string User_Cname { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public long User_No { get; set; }
        /// <summary>
        /// 用户类型(1学生,2老师,3管理员)
        /// </summary>
        public int User_Type { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string User_Password { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string User_Mobile { get; set; }
        /// <summary>
        /// 用户身份证号码
        /// </summary>
        public string User_IDCard_Number { get; set; }
        /// <summary>
        /// 用户年龄
        /// </summary>
        public int User_Age { get; set; }

        /// <summary>
        /// 班级Id,逗号隔开
        /// </summary>
        public string User_ClasseIds { get; set; }

        /// <summary>
        /// 班级名称,逗号隔开
        /// </summary>
        public string User_ClasseNames { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
