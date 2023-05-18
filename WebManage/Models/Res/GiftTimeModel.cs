using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class GiftTimeModel
    {
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContraNo { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// 赠送课时
        /// </summary>
        public float PresentTime { get; set; }
    }
}
