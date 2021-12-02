﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.InputModel
{
   public class ClueRecordInput
    {

        public int Id { get; set; }
        /// <summary>
        /// 线索id
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 跟踪者uid
        /// </summary>
        public int CC_Uid { get; set; }
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
        /// 上门日期
        /// </summary>
        public DateTime Visit_Date { get; set; }


        public string CreateUid { get; set; }

    }
}
