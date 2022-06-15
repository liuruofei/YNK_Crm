using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ProjectUnitTimeModel
    {
        public int Id { get; set; }
        public string Unit_TimeName { get; set; }
        public string Unit_TimeType { get; set; }
        public DateTime? At_Date { get; set; }
        /// <summary>
        /// 考试类型id
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        ///考试类型名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 科目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 单元Id
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// 单元名称
        /// </summary>
        public string UnitName { get; set; }

        public List<C_Project_Unit_Time> UnitTimeList { get; set; }
    }
}
