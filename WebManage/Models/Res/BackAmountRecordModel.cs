using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class BackAmountRecordModel
    {
        public int Id { get; set; }
        public int StudentUid { get; set; }
        public decimal BackAmount { get; set; }
        public string BackDate { get; set; }
        public string CreateUid { get; set; }
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Student_Name { get; set; }

        public string CreateName { get; set; }
    }
}
