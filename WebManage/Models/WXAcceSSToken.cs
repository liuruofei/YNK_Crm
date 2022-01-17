using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models
{
    public class WXAcceSSToken
    {
        public string Access_Token { get; set; }

        /// <summary>
        /// 秒
        /// </summary>
        public int Expires_in { get; set; }

        /// <summary>
        /// 记录本次时间
        /// </summary>
        public DateTime Date { get; set; }
    }
}
