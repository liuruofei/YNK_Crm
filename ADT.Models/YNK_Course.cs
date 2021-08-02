using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class YNK_Course
    {
        public int Id { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public int IsEnble { get; set; }
        public int Sort { get; set; }
        /// <summary>
        /// 课程标题
        /// </summary>
        public string CourseTitle { get; set; }

        /// <summary>
        /// 课程图片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 手机端图片
        /// </summary>
        public string MobileImgUrl { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
