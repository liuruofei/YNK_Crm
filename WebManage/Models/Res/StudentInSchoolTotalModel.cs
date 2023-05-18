using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class StudentInSchoolTotalDayModel
    {
        public int StudentUid { get; set;}

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name{ get; set;}

        /// <summary>
        /// 学生在校总计天数
        /// </summary>
        public int TotalDay { get; set; }

        /// <summary>
        /// 总计在校时长
        /// </summary>
        public string TotalHourse { get; set; }
        /// <summary>
        /// 总计分钟
        /// </summary>
        public float TotalMinus { get; set; }
    }

    public class InSchoolHourseTotalModel {
        public int StudentUid { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name { get; set; }


        public DateTime? WorkDate { get; set; }

        public string TotalTime { get; set; }
    }
}
