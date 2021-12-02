using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
   public class C_ClueUser_Subject
    {
        public int Id { get; set; }
        /// <summary>
        /// 线程id
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 分类Id
        /// </summary>
        public int SubjectId { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }

    }
}
