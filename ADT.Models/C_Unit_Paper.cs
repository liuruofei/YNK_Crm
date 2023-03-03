using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 单元试卷
    /// </summary>
   public class C_Unit_Paper
    {
        public int PaperId { get; set; }
        /// <summary>
        /// 试卷编号
        /// </summary>
        public string PaperCode { get; set; }
        public int SubjectId { get; set; }
        public int ProjectId { get; set; }
        public int UnitId { get; set; }
        /// <summary>
        /// 平均分数线
        /// </summary>
        public string AvgScore { get; set; }
    }
}
