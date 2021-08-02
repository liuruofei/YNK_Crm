using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 报名用户表
    /// </summary>
    public partial class YS_User_Enter
    {
        public YS_User_Enter()
        {

        }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UsersID { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证正面
        /// </summary>
        public string ID_Card_Front { get; set; }
        /// <summary>
        /// 身份证反面
        /// </summary>
        public string ID_Card_Back { get; set; }
        /// <summary>
        /// 报名机构
        /// </summary>
        public string Enter_OrganIzation { get; set; }
        /// <summary>
        /// 历史最高总分
        /// </summary>
        public decimal History_Max_Total { get; set; }
        /// <summary>
        /// 历史最高听力分
        /// </summary>
        public decimal History_Max_Listen { get; set; }
        /// <summary>
        /// 历史最高分(说)
        /// </summary>
        public decimal History_Max_Talk { get; set; }
        /// <summary>
        /// 历史最高分(读)
        /// </summary>
        public decimal History_Max_Reder { get; set; }
        /// <summary>
        /// 历史最高分(写)
        /// </summary>
        public decimal History_Max_Write { get; set; }
        /// <summary>
        /// 上次最高总分
        /// </summary>
        public decimal Last_Max_Total { get; set; }
        /// <summary>
        /// 上次最高听力分
        /// </summary>
        public decimal Last_Max_Listen { get; set; }
        /// <summary>
        /// 上次最高分(说)
        /// </summary>
        public decimal Last_Max_Talk { get; set; }
        /// <summary>
        /// 上次最高分(读)
        /// </summary>
        public decimal Last_Max_Reder { get; set; }
        /// <summary>
        /// 上次最高分写
        /// </summary>
        public decimal Last_Max_Write { get; set; }
        /// <summary>
        /// 是否签订协议
        /// </summary>
        public int Sign_Contract { get; set; }
        /// <summary>
        /// 报名时间
        /// </summary>
        public DateTime Join_date { get; set; }
        /// <summary>
        /// 报名地址
        /// </summary>
        public string Join_Address { get; set; }

        /// <summary>
        /// 报名考场
        /// </summary>
        public string ExaminationPlace { get; set; }
        /// <summary>
        /// 分数范围
        /// </summary>
        public string ScoreRange { get; set; }

        /// <summary>
        /// 上次成绩证明
        /// </summary>
        public string Last_Score_Img_Url { get; set; }


        /// <summary>
        /// 上次报名时间
        /// </summary>
        public string Last_Join_Time { get; set; }

        /// <summary>
        /// 是否第一次报名
        /// </summary>
        public int IsFirst { get; set; }


        /// <summary>
        /// 证明图片地址
        /// </summary>
        public string Certificate_Img_Url { get; set; }
        public int AuditUid { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditStatus { get; set; }
        /// <summary>
        /// 审核内容
        /// </summary>
        public string AuditMsg { get; set; }
        /// <summary>
        /// 是否被抽查
        /// </summary>
        public int IsChecked { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 数据行状态
        /// </summary>
        public int Status { get; set; }
        public string Remarks { get; set; }
    }
}
