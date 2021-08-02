using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Common
{
    /// <summary>
    /// 返回数据帮助类
    /// </summary>
    public class MessageDataResult
    {
        public MessageDataResult(int _status = 400, string _msg = "参数无效")
        {
            this.status = _status;
            this.msg = _msg;
        }
        /// <summary>
        /// 状态吗（成功返回200）
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 成功或失败提示
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回的数据源
        /// </summary>
        public object data { get; set; }
    }
}
