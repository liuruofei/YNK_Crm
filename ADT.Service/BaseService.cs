using ADT.Repository;
using ADT.Service.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace ADT.Service
{
    public class BaseService<T> : IBaseService<T> where T : class, new()
    {
        public IBaseRepository<T> _baseRepository;
        //public IBaseRepository<T> _baseRepository = ServiceResolve.Resolve<IBaseRepository<T>>();

        public BaseService(IBaseRepository<T> repository)
        {
            _baseRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        /// <summary>
        /// 根据主值查询单条数据
        /// </summary>
        /// <param name="pkValue">主键值</param>
        /// <returns>泛型实体</returns>
        public T FindById(object pkValue)
        {
            return _baseRepository.FindById(pkValue);
        }
        /// <summary>
        /// 根据主键查询单条数据（自定义返回值）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="pkValue"></param>
        /// <returns></returns>
        public TResult FindById<TResult>(Expression<Func<T, TResult>> columns, object pkValue)
        {
            return _baseRepository.FindById<TResult>(columns, pkValue);
        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FindAll()
        {
            return _baseRepository.FindAll();
        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用，自定义返回值)
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <returns></returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<T, TResult>> columns)
        {
            return _baseRepository.FindAll<TResult>(columns);
        }
        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        public IEnumerable<T> FindListByClause(Expression<Func<T, bool>> predicate, string orderBy)
        {
            return _baseRepository.FindListByClause(predicate, orderBy);
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
            return _baseRepository.FindListByClause<TResult>(columns, predicate, orderBy);
        }
        /// <summary>
        /// 根据条件查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public int FindCount(Expression<Func<T, bool>> predicate)
        {
            return _baseRepository.FindCount(predicate);
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
            return FindPageListByClause(predicate, page, row, orderBy);
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
            return _baseRepository.FindPageListByClause<TResult>(columns, predicate, page, row, orderBy);
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
            return _baseRepository.FindPageListByClause(predicate, page, row, orderBy, ref totalNumber);
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
            return _baseRepository.FindPageListByClause<TResult>(columns, predicate, page, row, orderBy, ref totalNumber);
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
            return _baseRepository.FindPageListByClauseData(predicate, page, row, orderBy, ref totalNumber);
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
            return _baseRepository.FindPageListByClauseData<TResult>(columns, predicate, page, row, orderBy, ref totalNumber);
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public T FindByClause(Expression<Func<T, bool>> predicate)
        {
            return _baseRepository.FindByClause(predicate);
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public TResult FindByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate)
        {
            return _baseRepository.FindByClause<TResult>(columns, predicate);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public long Insert(T entity)
        {
            return _baseRepository.Insert(entity);
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entitylist">实体类集合</param>
        /// <returns></returns>
        public long InsertBatch(List<T> entitylist)
        {
            return _baseRepository.InsertBatch(entitylist);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            return _baseRepository.Update(entity);
        }
        /// <summary>
        /// 更新自定义字段数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">修改字段表达式树</param>
        /// <param name="isNullColumn">是否不更新为NULL的字段</param>
        /// <returns></returns>
        public bool Update(T entity, Expression<Func<T, object>> updateColumns, bool isNullColumn = false)
        {
            return _baseRepository.Update(entity, updateColumns, isNullColumn);
        }

        /// <summary>
        /// 根据条件修改自定义字段数据
        /// </summary>
        /// <param name="entity">修改的实体</param>
        /// <param name="updateColumns">需要修改的字段</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public bool UpdateWhere(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, bool>> predicate)
        {
            return _baseRepository.UpdateWhere(entity, updateColumns, predicate);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            return _baseRepository.Delete(entity);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> @where)
        {
            return _baseRepository.Delete(where);
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(object id)
        {
            return _baseRepository.DeleteById(id);
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds(object[] ids)
        {
            return _baseRepository.DeleteByIds(ids);
        }
    }
}
