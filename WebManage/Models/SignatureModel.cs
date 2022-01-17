using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models
{
    public class SignatureModel
    {
        /// <summary>
        /// 随机数
        /// </summary>
        public string noncestr { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        public string signature { get; set; }


        public string appId { get; set; }
    }
}
