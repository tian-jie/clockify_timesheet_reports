using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Linq;
using Infrastructure.Core.Logging;

namespace Innocellence.WeChat.Domain.Common
{
    public static class EnumUtil
    {
        private static ILogger _logger = LogManager.GetLogger(typeof(EnumUtil));
        /// <summary>
        /// get all information of enum,include value,name and description
        /// </summary>
        /// <param name="enumName">the type of enumName</param>
        /// <returns></returns>
        public static List<dynamic> GetAllItems(this Type enumName)
        {
            var list = new List<dynamic>();
            // get enum fileds
            FieldInfo[] fields = enumName.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                {
                    continue;
                }
                // get enum value
                int value = (int)enumName.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                string text = field.Name;
                string description = string.Empty;
                object[] array = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (array.Length > 0)
                {
                    description = ((DescriptionAttribute)array[0]).Description;
                }
                else
                {
                    description = ""; //none description,set empty
                }
                //add to list
                dynamic obj = new ExpandoObject();
                obj.Value = value;
                obj.Text = text;
                obj.Description = description;
                list.Add(obj);
            }
            return list;
        }
        /// <summary>
        /// get enum description by name
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="enumItemName">the enum name</param>
        /// <returns></returns>
        public static string GetDescriptionByName<T>(this T enumItemName)
        {
            try
            {
                if (null != enumItemName)
                {
                    FieldInfo fi = enumItemName.GetType().GetField(enumItemName.ToString());

                    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                        typeof(DescriptionAttribute), false);

                    if (attributes != null && attributes.Length > 0)
                    {
                        return attributes[0].Description;
                    }
                    else
                    {
                        return enumItemName.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return string.Empty;
        }
    }
}
