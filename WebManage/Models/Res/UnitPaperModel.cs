using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class UnitPaperModel
    {
        public int PaperId { get; set; }
        /// <summary>
        /// 试卷编号
        /// </summary>
        public string PaperCode { get; set; }
        public int SubjectId { get; set; }
        public int ProjectId { get; set; }
        public int UnitId { get; set; }


        public string AvgScore { get; set; }

        /// <summary>
        ///考试类型名称
        /// </summary>
        public string SubjectName { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }
    }
}
