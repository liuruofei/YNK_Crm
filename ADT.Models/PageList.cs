using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models
{
    public class PageList<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 列表数据
        /// </summary>
        public IEnumerable<T> data { get; set; }
    }
}
