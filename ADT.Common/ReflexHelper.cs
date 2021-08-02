using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ADT.Common
{
    public class ReflexHelper
    {
        /// <summary>
        /// 获取 PropertyInfo 集合
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_bindingFlags"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos(Type _type, BindingFlags _bindingFlags = (BindingFlags.Instance | BindingFlags.Public))
        {
            return _type.GetProperties(_bindingFlags);
        }

        /// <summary>
        /// 获取 PropertyInfo 对象
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Type _type, string _name)
        {
            return _type.GetProperty(_name);
        }

        /// <summary>
        /// 获取 PropertyInfo 集合
        /// </summary>
        /// <param name="_type"></param>
        public static PropertyInfo[] GetPropertyInfos<T>(Expression<Func<T, dynamic>> _expr, BindingFlags _bindingFlags = (BindingFlags.Instance | BindingFlags.Public))
        {
            var body = _expr.Body;
            if (body.NodeType == ExpressionType.Parameter)
            {
                return (body as ParameterExpression).Type.GetProperties(_bindingFlags);
            }
            else if (body.NodeType == ExpressionType.New)
            {
                return (body as NewExpression).Members.Select(m => m as PropertyInfo).ToArray();
            }
            else if (body.NodeType == ExpressionType.Convert)
            {
                return ((body as UnaryExpression).Operand as ParameterExpression).Type.GetProperties(_bindingFlags);
            }
            return null;
        }

        /// <summary>
        /// 获取指定属性信息（非String类型存在装箱与拆箱）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, dynamic>> _expr)
        {
            var body = _expr.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取指定属性信息（需要明确指定属性类型，但不存在装箱与拆箱）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T, TR>(Expression<Func<T, TR>> _expr)
        {
            var body = _expr.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 对象 转换为 MemberInitExpression
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static MemberInitExpression MemberInit<T>(T _entity)
        {
            var proInfo = ReflexHelper.GetPropertyInfos(typeof(T));

            var list = new List<MemberBinding>();

            foreach (var item in proInfo)
            {
                list.Add(Expression.Bind(item, Expression.Constant(item.GetValue(_entity), item.PropertyType)));
            }

            var newExpr = Expression.New(typeof(T));

            return Expression.MemberInit(newExpr, list);
        }

        /// <summary>
        /// 获取 值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_entity"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static object GetValue<T>(T _entity, string _name)
        {
            return ReflexHelper.GetPropertyInfo(typeof(T), _name).GetValue(_entity);
        }

        /// <summary>
        /// 设置 值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_entity"></param>
        /// <param name="_name"></param>
        /// <param name="_val"></param>
        public static void SetValue<T>(T _entity, string _name, object _val)
        {
            ReflexHelper.GetPropertyInfo(typeof(T), _name).SetValue(_entity, _val);
        }

        /// <summary>
        /// 获取 对象 属性上 标记 集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_type"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(Type _type, string _name) where T : Attribute
        {
            return ReflexHelper.GetPropertyInfo(_type, _name).GetCustomAttributes<T>(false).ToList();
        }

        /// <summary>
        /// 获取 对象 中 某个属性得 标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_type"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(Type _type, string _name) where T : Attribute
        {
            return ReflexHelper.GetPropertyInfo(_type, _name).GetCustomAttribute(typeof(T)) as T;
        }

        /// <summary>
        /// 创建 对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>() where T : class, new()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
