using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Service.IService
{
    public interface ICurrencyService
    {
        /// <summary>
        /// 获取SqlSugar的SqlSugarClient
        /// </summary>
        /// <returns></returns>
        SqlSugarClient DbAccess();
    }
}
