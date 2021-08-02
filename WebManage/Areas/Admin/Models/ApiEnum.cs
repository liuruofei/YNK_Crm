using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Areas.Admin.Models
{
    /// <summary>
    /// 报名用户审核状态
    /// </summary>
    public enum ApiUserAuditEnum
    {
        /// <summary>
        /// 初级资料上传成功,待审核
        /// </summary>
        [Description("初级资料上传成功,待审核")]
        WatingAudit =0,
        /// <summary>
        /// 初级审核成功,等待上传凭证资料
        /// </summary>
        [Description("审核成功,待上传凭证资料")]
        AuditSuccess = 1,
        /// <summary>
        /// 初级审核驳回
        /// </summary>
        [Description("初级审核驳回")]
        AuditFaill =2,
        /// <summary>
        /// 上传报名凭证资料成功,待审核
        /// </summary>
        [Description("上传报名凭证资料成功,待审核")]
        UploadSuccess =3,
        /// <summary>
        /// 终极审核成功
        /// </summary>
        [Description("终极审核成功")]
        AuditUltimateSuccess =4,
        /// <summary>
        /// 终极审核驳回
        /// </summary>
        [Description("终极审核驳回")]
        AuditUltimateFaill = 5
    }
}
