using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        /// <summary>
        /// 获取SqlSugar的SqlSugarClient
        /// </summary>
        /// <returns></returns>
        SqlSugarClient DbAccess();
    }
}
