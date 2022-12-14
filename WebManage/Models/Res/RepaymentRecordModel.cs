using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class RepaymentRecordModel
    {
        public int Id { get; set; }
        public int CollgeId { get; set; }
        public decimal RepaymentAmount { get; set; }
        public string Contra_ChildNo { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }

    }
}
