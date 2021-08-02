using System;
using System.Collections.Generic;
using System.Reflection;

namespace ADT.Common
{
    public static class EnumHelper
    {
        /// <summary>
        /// 根据值获取枚举的描述 
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>枚举值的描述</returns>
        public static string GetDescription<TEnum>(object @value)
        {
            List<System.Collections.DictionaryEntry> lists = GetBindData<TEnum>();
            var a = lists.Find(item => Convert.ToInt32(item.Value) == Convert.ToInt32(@value));
            return a.Key + "";
        }

        /// <summary>  
        /// 获取自定义属性的值 
        /// </summary>  
        /// <param name="value">值</param>  
        /// <returns>枚举值的描述</returns>  
        public static string GetCustomAttribute<T>(object @value)
        {
            Type type = @value.GetType();
            MemberInfo[] memInfos = type.GetMember(@value.ToString());
            if (memInfos != null && memInfos.Length > 0)
            {
                object[] customEnumDesc = memInfos[0].GetCustomAttributes(typeof(T), false);
                if (customEnumDesc != null && customEnumDesc.Length > 0)
                {
                    return ((T)customEnumDesc[0]).GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0].GetValue(customEnumDesc[0]).ToString();
                }
            }
            return "";
        }
        /// <summary>
        /// 获取类类型的枚举描述
        /// </summary>
        /// <typeparam name="T1">需要转换的类型</typeparam>
        /// <typeparam name="T2">需要获取的类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static T1 GetAttributeObj<T1, T2>(object @value)
        {
            Type type = @value.GetType();
            MemberInfo[] memInfos = type.GetMember(@value.ToString());
            if (memInfos != null && memInfos.Length > 0)
            {
                object[] customEnumDesc = memInfos[0].GetCustomAttributes(typeof(T2), false);
                if (customEnumDesc != null && customEnumDesc.Length > 0)
                {
                    return (T1)((T2)customEnumDesc[0]).GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0].GetValue(customEnumDesc[0]);
                }
            }
            return default(T1);
        }

        public static string GetControlNameById(string ControlId) { return ControlId.Replace("_", "$"); }

        /// <summary>
        /// 获取某个枚举类型的数据源，数据源是描述和值的组合，返回字段是 Key(description)/Value(int)
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns>集合</returns>
        public static List<System.Collections.DictionaryEntry> GetBindData<TEnum>()
        {
            var obj = ResolveEnum<TEnum>();
            return obj;
        }

        static List<System.Collections.DictionaryEntry> ResolveEnum<TEnum>()
        {
            Type type = typeof(TEnum);
            if (!type.IsEnum) { throw new ArgumentException("TEnum requires a Enum", "TEnum"); }
            FieldInfo[] fields = type.GetFields();
            List<System.Collections.DictionaryEntry> col = new List<System.Collections.DictionaryEntry>();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (objs != null && objs.Length > 0)
                {
                    System.ComponentModel.DescriptionAttribute attr = objs[0] as System.ComponentModel.DescriptionAttribute;
                    col.Add(new System.Collections.DictionaryEntry(attr.Description, ((int)Enum.Parse(type, field.Name)).ToString()));
                }
            }
            return col;
        }

        public static System.Collections.DictionaryEntry GetEnumDict<TEnum>(Enum enumItem)
        {
            Type type = typeof(TEnum);
            if (!type.IsEnum) { throw new ArgumentException("TEnum requires a Enum", "TEnum"); }
            FieldInfo[] fields = type.GetFields();
            System.Collections.DictionaryEntry dict = new System.Collections.DictionaryEntry();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (objs != null && objs.Length > 0 && field.Name == enumItem.ToString())
                {
                    System.ComponentModel.DescriptionAttribute attr = objs[0] as System.ComponentModel.DescriptionAttribute;
                    dict = new System.Collections.DictionaryEntry(attr.Description, ((int)Enum.Parse(type, field.Name)).ToString());
                    break;
                }
            }
            return dict;
        }

        /// <summary>
        /// 获取所有枚举项
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItem<TEnum>()
        {
            Type type = typeof(TEnum);
            if (!type.IsEnum) { throw new ArgumentException("TEnum requires a Enum", "TEnum"); }

            FieldInfo[] fields = type.GetFields();
            List<EnumItem> col = new List<EnumItem>();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (objs != null && objs.Length > 0)
                {
                    System.ComponentModel.DescriptionAttribute attr = objs[0] as System.ComponentModel.DescriptionAttribute;
                    col.Add(new EnumItem { Description = attr.Description, Key = Enum.Parse(type, field.Name).ToString(), Value = (int)Enum.Parse(type, field.Name) });
                }
            }
            return col;
        }

        /// <summary>
        /// GetEnumItem
        /// </summary>
        /// <param name="type">typeof(TEnum)</param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumItem(Type type)
        {
            if (!type.IsEnum) { throw new ArgumentException("TEnum requires a Enum", "TEnum"); }
            FieldInfo[] fields = type.GetFields();
            List<EnumItem> col = new List<EnumItem>();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (objs != null && objs.Length > 0)
                {
                    System.ComponentModel.DescriptionAttribute attr = objs[0] as System.ComponentModel.DescriptionAttribute;
                    col.Add(new EnumItem { Description = attr.Description, Key = Enum.Parse(type, field.Name).ToString(), Value = (int)Enum.Parse(type, field.Name) });
                }
            }
            return col;
        }
    }

    /// <summary>
    /// 枚举项
    /// </summary>
    [Serializable]
    public class EnumItem
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get; set; }
    }
}