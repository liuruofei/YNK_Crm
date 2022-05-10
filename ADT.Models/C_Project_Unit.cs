using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 科目单元
    /// </summary>
   public class C_Project_Unit
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }

        /// <summary>
        /// 单元编码
        /// </summary>
        public string UnitCode { get; set; }

        public int ProjectId { get; set; }
        public int SubjectId { get; set; }
        public int Sort { get; set; }
    }
}
