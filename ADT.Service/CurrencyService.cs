using ADT.Service.IService;
using System;
using System.Collections.Generic;
using System.Text;
using ADT.Repository.IRepository;
using SqlSugar;

namespace ADT.Service
{
    public class CurrencyService : ICurrencyService
    {
        private ICurrencyRepository _currencyRepository;
        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }
        /// <summary>
        /// 获取SqlSugar的SqlSugarClient
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient DbAccess()
        {
            return _currencyRepository.DbAccess();
        }
    }
}
