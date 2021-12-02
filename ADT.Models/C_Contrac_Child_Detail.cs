using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 子合同明细
    /// </summary>
    public class C_Contrac_Child_Detail
    {
        public int Id { get; set; }
        public string ContraNo { get; set; }
        public string Contra_ChildNo { get; set; }
        public int StudentUid { get; set; }
        public float Course_Time { get; set; }
        public int SubjectId { get; set; }
        public int ProjectId { get; set; }
        public int Level { get; set; }
        public decimal Price { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
    }
}
