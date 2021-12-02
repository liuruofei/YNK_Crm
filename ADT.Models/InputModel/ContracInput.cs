using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
    public class ContracInput
    {
        public int ContracId { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContraNo { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name { get; set; }

        /// <summary>
        /// 学生手机
        /// </summary>
        public string Student_Phone { get; set; }

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
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }


        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Soure { get; set; }

        /// <summary>
        /// 学生邮箱
        /// </summary>
        public string Student_Email { get; set; }


        /// <summary>
        /// 在读学校
        /// </summary>
        public string InSchool { get; set; }


        /// <summary>
        /// 合同中心
        /// </summary>
        public int ContraCenterId { get; set; }
        /// <summary>
        /// 线索id
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 顾问uid
        /// </summary>
        public string CC_Uid { get; set; }


        public int Constra_Status { get; set; }
        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Total_Amount { get; set; }
        /// <summary>
        ///已付金额
        /// </summary>
        public decimal Pay_Amount { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start_Time { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End_Time { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }

        /// <summary>
        /// 学员编码
        /// </summary>
        public string Student_No { get; set; }

        public List<ContracChildInput> childList { get; set; }

    }
}
