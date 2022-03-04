using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{

    /// <summary>
    /// 签约合同用户
    /// </summary>
   public class C_Contrac_User
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
        /// 学生邮箱
        /// </summary>
        public string Student_Email { get; set; }

        /// <summary>
        /// 学生微信
        /// </summary>
        public string Student_Wechat { get; set; }

        /// <summary>
        /// 家长邮箱
        /// </summary>
        public string Elder_Email { get; set; }


        /// <summary>
        /// 家长姓名
        /// </summary>
        public string Elder_Name { get; set; }


        /// <summary>
        /// 家长手机
        /// </summary>
        public string Elder_Phone { get; set; }

        /// <summary>
        /// 家长微信
        /// </summary>
        public string Elder_Wechat { get; set; }

        /// <summary>
        /// 家长2邮箱
        /// </summary>
        public string Elder2_Email { get; set; }

        /// <summary>
        /// 家长2姓名
        /// </summary>
        public string Elder2_Name { get; set; }

        /// <summary>
        /// 家长2手机
        /// </summary>
        public string Elder2_Phone { get; set; }

        /// <summary>
        /// 家长2微信
        /// </summary>
        public string Elder2_Wechat { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Soure { get; set; }

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
        /// 小班课时
        /// </summary>
        public float Class_Course_Time { get; set; }

        /// <summary>
        /// 小班已用课时
        /// </summary>
        public float Class_Course_Use_Time { get; set; }

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
        /// <summary>
        /// 绑定微信关注公众号openId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 绑定的家长公众号OpenId
        /// </summary>
        public string Elder_OpenId { get; set; }
        /// <summary>
        /// 家长2OpenId
        /// </summary>
        public string Elder2_OpenId { get; set; }

        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
    }
}
