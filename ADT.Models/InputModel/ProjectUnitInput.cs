using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class ProjectUnitInput
    {
        /// <summary>
        /// 考试类型id
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// 科目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 科目列表
        /// </summary>
        public List<C_Project_Unit> UnitList { get; set; }
    }
}
