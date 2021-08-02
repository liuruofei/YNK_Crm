using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    [SugarTable("YNK_Banner")]
    public class YNK_Banner
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// banner图片
        /// </summary>
        public string BannerImage { get; set; }

        /// <summary>
        /// 手机端图片
        /// </summary>
        public string BannerImageMobile { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remarks { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
