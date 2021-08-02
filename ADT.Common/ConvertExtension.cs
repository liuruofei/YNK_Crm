using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ADT.Common
{
    public static class ConvertExtension
    {
        /// <summary>
        /// string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStr<T>(this T value)
        {
            try
            {
                if (value != null)
                    return value.ToString();
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt32<T>(this T value)
        {
            if (value == null) return 0;

            int result = 0;

            if (!Int32.TryParse(value.ToStr(), out result))
                return 0;

            return result;
        }

        /// <summary>
        /// float
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToFloat<T>(this T value)
        {
            if (value == null) return 0;

            float result = 0;

            if (!float.TryParse(value.ToStr(), out result))
                return 0;

            return result;
        }

        /// <summary>
        /// double
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble<T>(this T value)
        {
            if (value == null) return 0;

            double result = 0;

            if (!double.TryParse(value.ToStr(), out result))
                return 0;

            return result;
        }

        /// <summary>
        /// decimal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDecimal<T>(this T value)
        {
            if (value == null) return 0;

            decimal result = 0;

            if (!decimal.TryParse(value.ToStr(), out result))
                return 0;

            return result;
        }

        /// <summary>
        /// Guid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ToGuid<T>(this T value)
        {
            if (value == null) return Guid.Empty;

            Guid result = Guid.Empty;

            if (!Guid.TryParse(value.ToStr(), out result))
                return Guid.Empty;

            return result;
        }

        /// <summary>
        /// Guid?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid? ToGuidNull<T>(this T value)
        {
            if (value == null) return null;

            Guid result = Guid.Empty;

            if (!Guid.TryParse(value.ToStr(), out result))
                return null;

            return result;
        }

        /// <summary>
        /// GuidString
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToGuidStr<T>(this T value)
        {
            return value.ToGuid().ToStr();
        }

        /// <summary>
        /// DateTime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime<T>(this T value)
        {
            if (value == null) return DateTime.MinValue;

            DateTime result = DateTime.MinValue;

            if (!DateTime.TryParse(value.ToStr(), out result))
                return DateTime.MinValue;

            return result;
        }

        /// <summary>
        /// DateTime?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeNull<T>(this T value)
        {
            if (value == null) return null;

            DateTime result = DateTime.MinValue;

            if (!DateTime.TryParse(value.ToStr(), out result))
                return null;

            return result;
        }

        /// <summary>
        /// 格式的 时间 字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="FormatStr"></param>
        /// <returns></returns>
        public static string ToDateTimeFormat<T>(this T value, string FormatStr = "yyyy-MM-dd")
        {
            var datetime = value.ToDateTime();
            if (datetime.ToShortDateString() == DateTime.MinValue.ToShortDateString())
                return String.Empty;
            else
                return datetime.ToString(FormatStr);
        }

        /// <summary>
        /// bool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool<T>(this T value)
        {
            if (value == null) return false;

            bool result = false;

            if (!bool.TryParse(value.ToStr(), out result))
                return false;

            return result;
        }

        /// <summary>
        /// byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes<T>(this T value)
        {
            try
            {
                return value as byte[];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <returns></returns>
        public static DateTime ToTime<T>(this int timeStamp)
        {
            var dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public static int ToTimeInt<T>(this DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (int)((time) - startTime).TotalSeconds;
        }

        /// <summary>
        /// 将对象序列化为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将 Json 字符串转换为 指定的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将 Json 字符串转换为 object对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object DeserializeObject(this string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        /// <summary>
        /// DataRow 转换 实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow dr) where T : class, new()
        {
            var _Entity = ReflexHelper.CreateInstance<T>();
            var list = _Entity.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (list.Length == 0) throw new Exception("找不到任何 公共属性！");

            foreach (var item in list)
            {
                string AttrName = item.Name;
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    if (AttrName != dc.ColumnName) continue;
                    if (dr[dc.ColumnName] != DBNull.Value) item.SetValue(_Entity, dr[dc.ColumnName], null);
                }
            }
            return _Entity;
        }

        /// <summary>
        /// 将 datatable 转换为 list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            var list = new List<T>();

            var _Entity = ReflexHelper.CreateInstance<T>();
            var propertyInfo = _Entity.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (DataRow dr in table.Rows)
            {
                _Entity = ReflexHelper.CreateInstance<T>();
                foreach (var item in propertyInfo)
                {
                    string AttrName = item.Name;
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        if (AttrName != dc.ColumnName) continue;
                        if (dr[dc.ColumnName] != DBNull.Value)
                        {
                            item.SetValue(_Entity, dr[dc.ColumnName], null);
                        }
                        else
                            item.SetValue(_Entity, null, null);
                    }
                }
                list.Add(_Entity);
            }
            return list;
        }

        /// <summary>
        /// datatable 转换为 List<Dictionary<string,object>>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ToList(this DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            var dic = new Dictionary<string, object>();
            foreach (DataRow dr in table.Rows)
            {
                if (dic != null) dic = new Dictionary<string, object>();
                foreach (DataColumn dc in table.Columns)
                {
                    if (dc.DataType == typeof(DateTime))
                    {
                        dic.Add(dc.ColumnName, dr[dc.ColumnName].ToDateTimeFormat("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                    }
                }
                list.Add(dic);
            }
            return list;
        }

        /// <summary>
        /// IDataReader 转换为 DataTable
        /// </summary>
        /// <param name="_IDataReader"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IDataReader _IDataReader)
        {
            DataTable dt = new DataTable();
            dt.Load(_IDataReader);
            return dt;
        }

        /// <summary>
        /// 将匿名对象转换为字典
        /// </summary>
        /// <param name="Attribute"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary<T>(this T Attribute) where T : class, new()
        {
            var di = new Dictionary<string, object>();

            Type ty = Attribute.GetType();

            var fields = ty.GetProperties().ToList();

            foreach (var item in fields)
            {
                di.Add(item.Name, item.GetValue(Attribute).ToString());
            }

            return di;
        }


        /************sql****************/
        /// <summary>
        /// 在 拉姆达表达式 where 表达式中使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, List<T> array)
        {
            return true;
        }

        /// <summary>
        /// 在 拉姆达表达式 where 表达式中使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T obj, List<T> array)
        {
            return true;
        }

        /// <summary>
        /// 在 拉姆达表达式 where 表达式中使用 模糊查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="value">例如：%张三%</param>
        /// <returns></returns>
        public static bool Like<T>(this T obj, string value)
        {
            return true;
        }

        /// <summary>
        /// 一般在Where 条件中使用 例如 : w.User_CreateTime.SqlString("convert(varchar(50),@F,23)")   注意： 字符串中的 @F 代表当前 字段字符串 User_CreateTime
        /// 
        /// 一般用来支持这种语法》CONVERT(varchar(100), GETDATE(), 23)    -- 2006-05-16 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SqlString<T>(this T obj, string value)
        {
            return true;
        }

        /// <summary>
        /// 一般在Where 条件中使用 例如 : w.User_CreateTime.SqlString("convert(varchar(50),@F,23)")   注意： 字符串中的 @F 代表当前 字段字符串 User_CreateTime
        /// 
        /// 可以做比较 则使用该函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T SqlStringCompare<T>(this T obj, string value)
        {
            return default(T);
        }
    }
}
