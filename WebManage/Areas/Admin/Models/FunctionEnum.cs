using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Areas.Admin.Models
{
    /// <summary>
    /// 操作类型枚举
    /// </summary>
    public enum FunctionEnum
    {
        /// <summary>
        /// 能否拥有该菜单
        /// </summary>
        Have,
        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 保存
        /// </summary>
        Save,
        /// <summary>
        /// 导出
        /// </summary>
        GetExcel,
        /// <summary>
        /// 审核
        /// </summary>
        Audit,
    }
}
