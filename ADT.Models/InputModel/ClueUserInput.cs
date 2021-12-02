using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
    public class ClueUserInput
    {
        public int ClueId { get; set; }
        /// <summary>
        /// 线索编码
        /// </summary>
        public string ClueNo { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 所在学校
        /// </summary>
        public string InSchool { get; set; }
        /// <summary>
        /// 课程类型
        /// </summary>
        public string Course_Type { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Soure { get; set; }
        /// <summary>
        /// 学生手机
        /// </summary>
        public string Student_Phone { get; set; }
        /// <summary>
        /// 学生Email
        /// </summary>
        public string Student_Email { get; set; }
        /// <summary>
        /// 家长1Email
        /// </summary>
        public string Elder_Email { get; set; }
        /// <summary>
        /// 家长名称
        /// </summary>
        public string Elder_Name { get; set; }
        /// <summary>
        /// 家长身份
        /// </summary>
        public string Elder_Identity { get; set; }
        /// <summary>
        /// 家长手机
        /// </summary>
        public string Elder_Phone { get; set; }
        /// <summary>
        /// 家长2名称
        /// </summary>
        public string Elder_Name2 { get; set; }
        /// <summary>
        /// 家长2身份
        /// </summary>
        public string Elder_Identity2 { get; set; }
        /// <summary>
        /// 家长2手机
        /// </summary>
        public string Elder_Phone2 { get; set; }
        /// <summary>
        /// 家长2邮箱
        /// </summary>
        public string Elder_Email2 { get; set; }
        /// <summary>
        /// 更多联系人
        /// </summary>
        public string More_Contacts { get; set; }
        /// <summary>
        /// 学生状态
        /// </summary>
        public int Student_Status { get; set; }
        /// <summary>
        /// 顾问UID
        /// </summary>
        public string CC_Uid { get; set; }

        /// <summary>
        ///销售Uid
        /// </summary>
        public string CR_Uid { get; set; }

        /// <summary>
        /// 校区
        /// </summary>
        public int CampusId { get; set; }

        /// <summary>
        /// 是否上门
        /// </summary>
        public int Is_Visit { get; set; }
        /// <summary>
        /// 上门日期
        /// </summary>
        public DateTime? Visit_Date { get; set; }

        /// <summary>
        /// 首次到校时间
        /// </summary>
        public DateTime? FirstTime { get; set; }

        /// <summary>
        /// 学生微信
        /// </summary>
        public string Student_Wechat { get; set; }

        /// <summary>
        /// 家长微信
        /// </summary>
        public string Elder_Wechat { get; set; }


        /// <summary>
        /// 签约概率
        /// </summary>
        public int ContracRate { get; set; }

        /// <summary>
        /// 线索拥有者
        /// </summary>
        public string Owin_CC_Uid { get; set; }

        /// <summary>
        /// 跟踪日期
        /// </summary>
        public DateTime? Follow_Date { get; set; }
        /// <summary>
        /// 跟踪计划
        /// </summary>
        public string Follow_Plan { get; set; }
        /// <summary>
        /// 学生描述
        /// </summary>
        public string Recommend { get; set; }
        /// <summary>
        /// 数据行状态
        /// </summary>
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
        /// <summary>
        /// 分类集合Id
        /// </summary>
        public List<int> SubjectIds { get; set; }
    }
}
