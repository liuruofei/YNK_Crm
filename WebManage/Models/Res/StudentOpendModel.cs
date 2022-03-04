using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
   public class StudentOpendModel
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
        /// 绑定公众号OpenId
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
        /// <summary>
        /// 判断子合同状态用
        /// </summary>
        public int Contrac_Child_Status { get; set; }
    }
}
