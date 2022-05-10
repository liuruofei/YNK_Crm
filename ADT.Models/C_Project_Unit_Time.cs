using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
  public  class C_Project_Unit_Time
    {
        public int Id { get; set; }
        public string Unit_TimeName { get; set; }
        public string Unit_TimeType { get; set; }
        public DateTime? At_Date { get; set; }
        public int UnitId { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 科目分类
        /// </summary>
        public int SubjectId { get; set; }

        public string Remark { get; set; }
    }
}
