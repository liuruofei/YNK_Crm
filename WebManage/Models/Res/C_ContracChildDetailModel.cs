using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ContracChildDetailModel
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
        public string SubjectName { get; set; }

        public string ProjectName { get; set; }

    }
}
