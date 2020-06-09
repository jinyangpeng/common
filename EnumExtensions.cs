using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Basic.Infrastructure.Common
{
    public static class EnumExtensions
    {
        //public interface ISelectListEnum { }

        /// <summary>
        /// 枚举子项实体
        /// </summary>
        public class EnumItem
        {
            /// <summary>
            /// 子项名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 子项 int 值
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// 子项描述标注
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 是否选中
            /// </summary>
            public bool Selected { get; set; }
        }

        public class EnumItem<T> : EnumItem where T : Attribute
        {
            /// <summary>
            /// 自定义扩展
            /// </summary>
            public T Attribute { get; set; }
        }

        public class CustomEnumAttribute : Attribute
        {
            public CustomEnumAttribute() { }

            public CustomEnumAttribute(bool selected) 
            {
                this.Selected = selected;
            }

            public virtual bool Selected { get; set; }
        }

        //public static Di

        /// <summary>
        /// 获取枚举子项列表
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<EnumItem> GetEnumListItem(this Type enumType)
        {
            List<EnumItem> result = new List<EnumItem>();

            //获取枚举类型的所有子项
            //排除 名为 value__ 的项，这个是系统保留项，不处理。
            FieldInfo[] fields = enumType.GetFields().Where(s => s.Name != "value__").ToArray();

            foreach (FieldInfo field in fields)
            {
                EnumItem item = new EnumItem();
                //子项名字
                item.Name = field.Name;
                //获取 int 值
                item.Value = (int)System.Enum.Parse(enumType, field.Name);
                //获取标注
                DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (EnumAttributes != null && EnumAttributes.Length > 0)
                    item.Description = EnumAttributes[0].Description;

                CustomEnumAttribute[] customEnumAttributes = (CustomEnumAttribute[])field.GetCustomAttributes(typeof(CustomEnumAttribute), false);
                if (customEnumAttributes != null && customEnumAttributes.Length > 0)
                    item.Selected = customEnumAttributes[0].Selected;

                result.Add(item);
            }

            return result;
        }

        public static List<EnumItem<T>> GetEnumListItem<T>(this Type enumType) where T : Attribute
        {
            List<EnumItem<T>> result = new List<EnumItem<T>>();

            //获取枚举类型的所有子项
            //排除 名为 value__ 的项，这个是系统保留项，不处理。
            FieldInfo[] fields = enumType.GetFields().Where(s => s.Name != "value__").ToArray();
            foreach (FieldInfo field in fields)
            {
                EnumItem<T> item = new EnumItem<T>();
                //子项名字
                item.Name = field.Name;
                //获取 int 值
                item.Value = (int)System.Enum.Parse(enumType, field.Name);
                //获取标注
                DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (EnumAttributes != null && EnumAttributes.Length > 0)
                    item.Description = EnumAttributes[0].Description;

                //获取自定义标注
                T[] customAttributes = (T[])field.GetCustomAttributes(typeof(T), false);
                if (customAttributes != null && customAttributes.Length > 0)
                    item.Attribute = customAttributes as T;

                result.Add(item);
            }

            return result;
        }

        public static EnumItem<T> GetEnum<T>(this Type enumType, string nameOrDescript) where T : Attribute
        {
            EnumItem<T> result = null;
            var list = GetEnumListItem<T>(enumType);
            foreach (var item in list)
            {
                if (item.Name == nameOrDescript || item.Description == nameOrDescript)
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        public static EnumItem<T> GetEnum<T>(this Type enumType, int value) where T : Attribute
        {
            EnumItem<T> result = null;
            var list = GetEnumListItem<T>(enumType);
            foreach (var item in list)
            {
                if (item.Value == value)
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        public static EnumItem GetEnum(this Type enumType, string nameOrDescript)
        {
            EnumItem result = null;
            var list = GetEnumListItem(enumType);
            foreach (var item in list)
            {
                if (item.Name == nameOrDescript || item.Description == nameOrDescript)
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        public static EnumItem GetEnum(this Type enumType, int value)
        {
            EnumItem result = null;
            var list = GetEnumListItem(enumType);
            foreach (var item in list)
            {
                if (item.Value == value)
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enum_item_name"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Type enumType, string enum_item_name)
        {
            string result = string.Empty;
            if (ExistsEnumItem(enumType, enum_item_name))
            {
                //直接通过子项名字找出子项的反射类型。
                FieldInfo field = enumType.GetField(enum_item_name);
                //反射类型获取出子项上的标注
                DescriptionAttribute[] Descriptions = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (Descriptions != null && Descriptions.Length > 0)
                {
                    result = Descriptions[0].Description;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enum_item_value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Type enumType, int enum_item_value)
        {
            string result = string.Empty;
            FieldInfo[] fields = enumType.GetFields().Where(s => s.Name != "value__").ToArray();

            foreach (FieldInfo field in fields)
            {
                if ((int)System.Enum.Parse(enumType, field.Name) == enum_item_value)
                {
                    DescriptionAttribute[] Descriptions = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (Descriptions != null && Descriptions.Length > 0)
                    {
                        result = Descriptions[0].Description;
                    }
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 判断给定的名字是否是枚举里的子项
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enum_item_name"></param>
        /// <returns></returns>
        public static bool ExistsEnumItem(this Type enumType, string enum_item_name)
        {
            return enumType.GetField(enum_item_name) != null;
        }

        /// <summary>
        /// 判断给定的值是否是枚举里的子项的值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enum_item_value"></param>
        /// <returns></returns>
        public static bool ExistsEnumItem(this Type enumType, int enum_item_value)
        {
            bool result = false;
            FieldInfo[] fields = enumType.GetFields().Where(s => s.Name != "value__").ToArray();
            foreach (FieldInfo field in fields)
            {
                if ((int)System.Enum.Parse(enumType, field.Name) == enum_item_value)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
