using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
   public class PartBackCostInput
    {
        public int Id { get; set; }
        public string Contra_ChildNo { get; set; }

        public int StudentUid { get; set; }
        public int SubjectId { get; set; }

        /// <summary>
        /// 科目Id
        /// </summary>
        public int ProjectId { get; set; }

        public int ClassId { get; set; }

        public float BackCourseTime { get; set; }

        public string CreateUid { get; set; }

        public decimal BackAmount { get; set; }
    }
}
