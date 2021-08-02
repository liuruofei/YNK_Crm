using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class YNK_Category
    {
        public int Id { get; set; }

        /// <summary>
        /// key值
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        public int Sort { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParenId { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remarks { get; set; }
    }
}
