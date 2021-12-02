using ADT.Common;
using ADT.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ClueRecordModel
    {
        /// <summary>
        /// 跟踪内容
        /// </summary>
        public string Follow_Content { get; set; }
        /// <summary>
        /// 跟踪计划
        /// </summary>
        public string Follow_Plan { get; set; }
        public DateTime Follow_Date { get; set; }
        public int ContracRate { get; set; }

        /// <summary>
        /// 是否上门
        /// </summary>
        public int Is_Visit { get; set; }

        /// <summary>
        /// 上门状态
        /// </summary>
        public string IsVisitName
        {
            get
            {
                return EnumHelper.GetDescription<Student_Status>(Is_Visit);
            }
        }

        /// <summary>
        /// 签约可能性
        /// </summary>
        public string ContracRateName
        {
            get
            {
                return EnumHelper.GetDescription<ContracRate>(ContracRate);
            }
        }

        /// <summary>
        /// 顾问名称
        /// </summary>
        public string CCUserName {
            get;set;
        }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name {
            get; set;
        }

        /// <summary>
        /// 上门日期
        /// </summary>
        public DateTime Visit_Date { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
