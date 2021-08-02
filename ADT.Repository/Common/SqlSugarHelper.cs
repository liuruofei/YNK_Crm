using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ADT.Repository
{
    public class SqlSugarHelper
    {
        /// <summary>
        /// 初始化SqlSugar
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetInstance()
        {
            //0-MySql、1-SqlServer、2-Sqlite、3-Oracle、4-PostgreSQL
            //设置数据库类型
            DbType dbtype = (DBConfigHelper.GetConfig("SqlName") ?? "SqlServer") == "SqlServer" ? DbType.SqlServer : DbType.MySql;
            //获取数据库连接字符串名称
            var dbdatabase = DBConfigHelper.GetConfig("SqlConn");
            var connectionConfig = new ConnectionConfig();
            connectionConfig.ConnectionString = DBConfigHelper.GetConfig($"ConnectionStrings:{dbdatabase}");
            connectionConfig.DbType = dbtype;
            connectionConfig.IsAutoCloseConnection = true;
            //获取读取数据库连接组
            var slaveConnectionConfigs = DBConfigHelper.GetSlaveConnectionStrings($"SlaveConnectionStrings");
            //var slavelist = JsonConvert.DeserializeObject<List<SlaveConnectionConfig>>(slaveConnectionConfigs);
            if (slaveConnectionConfigs != null && slaveConnectionConfigs.Count > 0)
                connectionConfig.SlaveConnectionConfigs = slaveConnectionConfigs;
            SqlSugarClient db = new SqlSugarClient(connectionConfig);
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (!string.IsNullOrEmpty(DBConfigHelper.Evn) && DBConfigHelper.Evn != "Production")
                {
                    Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    Console.WriteLine();
                }
            };
            return db;
        }
    }
}
