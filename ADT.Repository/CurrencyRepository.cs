using ADT.Repository.IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        /// <summary>
        /// 获取SqlSugar的SqlSugarClient
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient DbAccess()
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                return db;
            }
        }
    }
}
