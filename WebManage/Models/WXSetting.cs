﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models
{
    public class WXSetting
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string TemplateId { get; set; }

        /// <summary>
        /// 评论模板
        /// </summary>
        public string TemplateIdComend { get; set; }
        /// <summary>
        /// 家长沟通消息模板
        /// </summary>
        public string TemplateSummary { get; set; }

        /// <summary>
        /// 业务交易消息
        /// </summary>
        public string TemplateBusines { get; set; }

        /// <summary>
        /// 作业提醒
        /// </summary>
        public string TemplateHomeWork { get; set; }
    }
}
