using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    /// <summary>
    /// 获取微信手机号码参数实体
    /// </summary>
    public class GetWeChartPhoneQuery
    {
        /// <summary>
        /// 登陆时获取的sessionKey 
        /// </summary>
        public string session_key { get; set; }
        /// <summary>
        /// 包括敏感数据在内的完整用户信息的加密数据
        /// </summary>
        public string encryptedData { get; set; }
        /// <summary>
        /// 加密算法的初始向量
        /// </summary>
        public string iv { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nick_name { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string head_photo { get; set; }
    }
}
