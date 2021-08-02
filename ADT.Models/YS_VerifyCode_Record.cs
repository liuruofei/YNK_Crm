using System;
using System.Linq;
using System.Text;

namespace ADT.Models
{
    ///<summary>
    ///验证码表
    ///</summary>
    public partial class YS_VerifyCode_Record
    {
        public YS_VerifyCode_Record()
        {


        }
        /// <summary>
        /// 自增id
        /// </summary>
        public int SmsId { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string VerifyCode { get; set; }
        /// <summary>
        /// 验证码状态
        /// </summary>
        public int VerifyStatus { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendDateTime { get; set; }
        /// <summary>
        /// 验证码到期时间
        /// </summary>
        public DateTime VerifyDateTime { get; set; }
        /// <summary>
        /// 行状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
