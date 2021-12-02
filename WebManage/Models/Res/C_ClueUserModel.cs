using ADT.Common;
using ADT.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class ResClueUserModel
    {

        public int ClueId { get; set; }

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
        public DateTime? Visit_Date { get; set; } = null;

        /// <summary>
        /// 首次到校时间
        /// </summary>
        public DateTime? FirstTime { get; set; }

        /// <summary>
        /// 签约概率
        /// </summary>
        public int ContracRate { get; set; }
        /// <summary>
        /// 跟踪日期
        /// </summary>
        public DateTime? Follow_Date { get; set; } = null;
        /// <summary>
        /// 跟踪计划
        /// </summary>
        public string Follow_Plan { get; set; }
        /// <summary>
        /// 学生描述
        /// </summary>
        public string Recommend { get; set; }

        /// <summary>
        /// 上门状态
        /// </summary>
        public string IsVisitName {
            get {
                return EnumHelper.GetDescription<Student_Status>(Is_Visit);
            }
        }
        
        /// <summary>
        /// 签约可能性
        /// </summary>
        public string ContracRateName {
            get {
                return EnumHelper.GetDescription<ContracRate>(ContracRate);
            }
        }

        /// <summary>
        /// 学生微信
        /// </summary>
        public string Student_Wechat { get; set; }


        /// <summary>
        /// 家长微信
        /// </summary>
        public string Elder_Wechat { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }

        /// <summary>
        /// 跟踪次数
        /// </summary>
        public int Follow_Count { get; set; }

        /// <summary>
        /// 顾问拥有者名称
        /// </summary>
        public string CC_UserName { get; set; }

        /// <summary>
        /// 顾问创建者
        /// </summary>
        public string Default_CC_UserName { get; set; }

        /// <summary>
        /// 线索所选课程
        /// </summary>
        public List<int> SubjectIds { get; set; }


        public DateTime CreateTime { get; set; }
    }
}
