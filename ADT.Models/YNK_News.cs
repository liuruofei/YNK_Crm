using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    /// <summary>
    /// 新闻
    /// </summary>
   public class YNK_News
    {
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 手机端图片
        /// </summary>
        public string MobileImgUrl { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
