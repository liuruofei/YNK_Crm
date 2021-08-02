using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace ADT.Repository
{
    public abstract class BaseRepository<T> where T : class, new()
    {
        #region Implementation of IRepository<T>

        /// <summary>
        /// 根据主值查询单条数据
        /// </summary>
        /// <param name="pkValue">主键值</param>
        /// <returns>泛型实体</returns>
        public T FindById(object pkValue)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var entity = db.Queryable<T>().InSingle(pkValue);
                return entity;
            }
        }
        /// <summary>
        /// 根据主键查询单条数据（自定义返回值）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="pkValue">主键值</param>
        /// <returns></returns>
        public TResult FindById<TResult>(Expression<Func<T, TResult>> columns, object pkValue)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                return list.Select(columns).First();
            }
        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FindAll()
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>().ToList();
                return list;
            }
        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用，自定义返回值)
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <returns></returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<T, TResult>> columns)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>().Select(columns).ToList();
                return list;
            }
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        public IEnumerable<T> FindListByClause(Expression<Func<T, bool>> predicate, string orderBy)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var entities = db.Queryable<T>();
                if (predicate != null)
                    entities = entities.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    entities = entities.OrderBy(orderBy);
                return entities.ToList();
            }
        }

        /// <summary>
        /// 根据条件查询数据(自定义返回值)
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        public IEnumerable<TResult> FindListByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, string orderBy)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var entities = db.Queryable<T>();
                if (predicate != null)
                    entities = entities.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    entities = entities.OrderBy(orderBy);
                return entities.Select(columns).ToList();
            }
        }
        /// <summary>
        /// 根据条件查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public int FindCount(Expression<Func<T, bool>> predicate)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var count = db.Queryable<T>();
                if (predicate != null)
                    count = count.Where(predicate);
                return count.Count();
            }
        }

        /// <summary>
        /// 根据条件分页
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <returns></returns>
        public IEnumerable<T> FindPageListByClause(Expression<Func<T, bool>> predicate, int page, int row, string orderBy)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.ToPageList(page, row);
            }
        }

        /// <summary>
        /// 根据条件分页
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <returns></returns>
        public IEnumerable<TResult> FindPageListByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, int page, int row, string orderBy)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.Select(columns).ToPageList(page, row);
            }
        }

        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        public IEnumerable<T> FindPageListByClause(Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.ToPageList(page, row, ref totalNumber);
            }
        }

        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        public IEnumerable<TResult> FindPageListByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.Select(columns).ToPageList(page, row, ref totalNumber);
            }
        }

        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        public DataTable FindPageListByClauseData(Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.ToDataTablePage(page, row, ref totalNumber);
            }
        }

        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        public DataTable FindPageListByClauseData<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var list = db.Queryable<T>();
                if (predicate != null)
                    list = list.Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                    list = list.OrderBy(orderBy);
                return list.Select(columns).ToDataTablePage(page, row, ref totalNumber);
            }
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public T FindByClause(Expression<Func<T, bool>> predicate)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var entity = db.Queryable<T>().First(predicate);
                return entity;
            }
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public TResult FindByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var entity = db.Queryable<T>().Where(predicate).Select(columns).First();
                return entity;
            }
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public long Insert(T entity)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                //返回插入数据的标识字段值
                var i = db.Insertable(entity).ExecuteCommand();
                return i;
            }
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entitylist">实体类集合</param>
        /// <returns></returns>
        public long InsertBatch(List<T> entitylist)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Insertable(entitylist.ToArray()).ExecuteCommand();
                return i;
            }
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                //这种方式会以主键为条件
                var i = db.Updateable(entity).ExecuteCommand();
                return i > 0;
            }
        }
        /// <summary>
        /// 更新自定义字段数据(不更新为NULL的列)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">修改字段表达式树</param>
        /// <param name="isNullColumn">是否不更新为NULL的字段</param>
        /// <returns></returns>
        public bool Update(T entity, Expression<Func<T, object>> updateColumns, bool isNullColumn = false)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Updateable(entity).UpdateColumns(updateColumns).IgnoreColumns(ignoreAllNullColumns: isNullColumn).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 根据条件修改自定义字段数据（没有主键的情况下更新）
        /// </summary>
        /// <param name="entity">修改的实体</param>
        /// <param name="updateColumns">需要修改的字段</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public bool UpdateWhere(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, bool>> predicate)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Updateable(entity).UpdateColumns(updateColumns).Where(predicate).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Deleteable(entity).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> @where)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Deleteable<T>(@where).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(object id)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Deleteable<T>(id).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds(object[] ids)
        {
            using (var db = SqlSugarHelper.GetInstance())
            {
                var i = db.Deleteable<T>().In(ids).ExecuteCommand();
                return i > 0;
            }
        }

        #endregion
    }
}
