using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
   public class ProjectUnitTimeInput
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
        /// 单元Id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 科目列表
        /// </summary>
        public List<C_Project_Unit_Time> UnitTimeList { get; set; }
    }
}
