using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class C_SequenceCode
    {
        public int Id { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public long SequenceNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
