using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ProjectUnitModel
    {
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

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 科目列表
        /// </summary>
        public List<C_Project_Unit> UnitList { get; set; }
    }
}
