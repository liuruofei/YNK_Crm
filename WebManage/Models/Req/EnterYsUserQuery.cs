using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    /// <summary>
    /// 提交线下精准请求参数实体
    /// </summary>
    public class EnterYsUserQuery
    {
        /// <summary>
        /// 验证码
        /// </summary>
       public string VerifyCode { get; set; }
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
        /// 证明图片地址
        /// </summary>
        public string Certificate_Img_Url { get; set; }

        /// <summary>
        /// 是否被抽查
        /// </summary>
        public int IsChecked { get; set; }

    }
}
