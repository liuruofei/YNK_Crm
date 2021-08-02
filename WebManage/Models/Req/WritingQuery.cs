using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    public class GetOrallanguageListQuery
    {
        /// <summary>
        /// 口语类型集合
        /// </summary>
        public List<string> list { get; set; }
        /// <summary>
        /// 一级分类id
        /// </summary>
        public string id { get; set; }
    }
}
