using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace ADT.Repository
{
    /// <summary>
    /// 仓储通用接口类
    /// </summary>
    /// <typeparam name="T">泛型实体类</typeparam>
    public interface IBaseRepository<T> where T : class, new()
    {
        /// <summary>
        /// 根据主值查询单条数据
        /// </summary>
        /// <param name="pkValue">主键值</param>
        /// <returns>泛型实体</returns>
        T FindById(object pkValue);
        /// <summary>
        /// 根据主键查询单条数据（自定义返回值）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="pkValue"></param>
        /// <returns></returns>
        TResult FindById<TResult>(Expression<Func<T, TResult>> columns, object pkValue);

        /// <summary>
        /// 查询所有数据(无分页,请慎用)
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> FindAll();

        /// <summary>
        /// 查询所有数据(无分页,请慎用，自定义返回值)
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <returns></returns>
        IEnumerable<TResult> FindAll<TResult>(Expression<Func<T, TResult>> columns);
        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        IEnumerable<T> FindListByClause(Expression<Func<T, bool>> predicate, string orderBy);

        /// <summary>
        /// 根据条件查询数据(自定义返回值)
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        IEnumerable<TResult> FindListByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, string orderBy);
        /// <summary>
        /// 根据条件查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        int FindCount(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据条件分页
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <returns></returns>
        IEnumerable<T> FindPageListByClause(Expression<Func<T, bool>> predicate, int page, int row, string orderBy);

        /// <summary>
        /// 根据条件分页
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <returns></returns>
        IEnumerable<TResult> FindPageListByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, int page, int row, string orderBy);
        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        IEnumerable<T> FindPageListByClause(Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber);

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
        IEnumerable<TResult> FindPageListByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber);

        /// <summary>
        /// 根据条件分页及查询总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="page">第几页</param>
        /// <param name="row">每页显示多少条</param>
        /// <param name="orderBy">排序规则</param>
        /// <param name="totalNumber">总数（返回值）</param>
        /// <returns></returns>
        DataTable FindPageListByClauseData(Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber);

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
        DataTable FindPageListByClauseData<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate, int page, int row, string orderBy, ref int totalNumber);

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        T FindByClause(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="columns">自定义返回值表达式数</param>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        TResult FindByClause<TResult>(Expression<Func<T, TResult>> columns, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        long Insert(T entity);

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entitylist">实体类集合</param>
        /// <returns></returns>
        long InsertBatch(List<T> entitylist);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(T entity);
        /// <summary>
        /// 更新自定义字段数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">修改字段表达式树</param>
        /// <param name="isNullColumn">是否不更新为NULL的字段</param>
        /// <returns></returns>
        bool Update(T entity, Expression<Func<T, object>> updateColumns, bool isNullColumn = false);

        /// <summary>
        /// 根据条件修改自定义字段数据
        /// </summary>
        /// <param name="entity">修改的实体</param>
        /// <param name="updateColumns">需要修改的字段</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        bool UpdateWhere(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Delete(T entity);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> @where);

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteById(object id);

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool DeleteByIds(object[] ids);
    }
}
