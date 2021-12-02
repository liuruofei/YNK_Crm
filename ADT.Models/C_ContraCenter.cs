using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class C_ContraCenter
    {
        public int Id { get; set; }
        public string ContraCenter_Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }


    }
}
