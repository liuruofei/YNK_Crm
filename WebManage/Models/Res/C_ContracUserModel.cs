using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ContracUserModel
    {
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }
        /// <summary>
        /// 学生编码
        /// </summary>
        public string Student_No { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Student_Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 学生账号
        /// </summary>
        public string Student_Account { get; set; }
        /// <summary>
        /// 学生密码
        /// </summary>
        public string Student_Pwd { get; set; }
        /// <summary>
        /// 学生手机号
        /// </summary>
        public string Student_Phone { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 签约校区
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 所在学校
        /// </summary>
        public string InSchool { get; set; }
        /// <summary>
        /// 课时
        /// </summary>
        public float Course_Time { get; set; }
        /// <summary>
        /// 已用课时
        /// </summary>
        public float Course_Use_Time { get; set; }
        /// <summary>
        /// 销售Uid
        /// </summary>
        public string CR_Uid { get; set; }
        /// <summary>
        /// 顾问uid
        /// </summary>
        public string CC_Uid { get; set; }
        /// <summary>
        /// 督导uid
        /// </summary>
        public string TA_Uid { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactFamily { get; set; }
        /// <summary>
        /// 是否申请退费
        /// </summary>
        public int IsBackAmount { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 首次到校日期
        /// </summary>
        public DateTime First_School_Time { get; set; }
        /// <summary>
        /// 开课日期
        /// </summary>
        public DateTime School_Begins { get; set; }
        /// <summary>
        /// 最近考试时间
        /// </summary>
        public DateTime Last_Test_Time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 顾问名称
        /// </summary>
        public string CC_UserName { get; set; }

        /// <summary>
        /// 销售名称
        /// </summary>
        public string CR_UserName { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }
    }
}
