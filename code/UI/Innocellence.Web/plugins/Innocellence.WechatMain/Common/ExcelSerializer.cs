using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Reflection;
using Infrastructure.Utility.IO;
using System.ComponentModel;
//using NPOI.HSSF.UserModel;

namespace Innocellence.WeChatMain.Common
{
    /// <summary>
    /// 列表导出Excel文件工具类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelSerializer<T> where T : new()
    {
        private bool _ignoreReferenceTypesExceptString = true;

        private List<PropertyInfo> _properties = new List<PropertyInfo>();

        public bool IgnoreReferenceTypesExceptString 
        { 
            get
            {
                return _ignoreReferenceTypesExceptString;
            }
            set
            {
                _ignoreReferenceTypesExceptString = value;
            }
        }

        /// <summary>
        /// 导出Excel文件流
        /// </summary>
        /// <param name="data">列表数据</param>
        /// <param name="propertyNames">需要导出的属性名</param>
        /// <returns></returns>
        public Stream SerializeStream(IList<T> data, string[] propertyNames)
        {
            IWorkbook workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var headerRow = sheet.CreateRow(0);

            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty).AsQueryable();
            if (IgnoreReferenceTypesExceptString)
	        {
                props = props.Where(p => p.PropertyType.IsValueType || p.PropertyType.Name == "String");
	        }
            this._properties = props.Where(a => a.GetCustomAttributes(typeof(CsvIgnoreAttribute), false) == (object)null || a.GetCustomAttributes(typeof(CsvIgnoreAttribute), false).Length == 0).ToList();
            int headerIndex = 0;
            foreach (var item in GetHeader(propertyNames))
	        {
                headerRow.CreateCell(headerIndex).SetCellValue(item);
                headerIndex++;
	        }
            int index = 1;
            foreach (var item in data)
            {
                var row = sheet.CreateRow(index);
                index++;
                int cellIndex = 0;
                foreach (var propertyInfo in propertyNames == null ? (IEnumerable<PropertyInfo>)this._properties : this.OrderProperties((IEnumerable<PropertyInfo>)this._properties, propertyNames))
                {
                    var obj = propertyInfo.GetValue(item, null);
                    var cell = row.CreateCell(cellIndex, CellType.String);
                    if (obj != null)
                    {
                        cell.SetCellValue(obj.ToString());
                    }
                    else
                    {
                        cell.SetCellValue(string.Empty);
                    }
                    cellIndex++;
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return new MemoryStream(memoryStream.ToArray());
            }
        }

        private IEnumerable<string> GetHeader(string[] propertyNames)
        {
          IEnumerable<string> result = this.GetHeaders(propertyNames == null ? (IEnumerable<PropertyInfo>) this._properties : this.OrderProperties((IEnumerable<PropertyInfo>) this._properties, propertyNames));
          return result;
          
        }

        public IEnumerable<string> GetHeaders(IEnumerable<PropertyInfo> properties)
        {
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    DescriptionAttribute descriptionAttribute = attributes[0] as DescriptionAttribute;
                    string description = descriptionAttribute.Description;
                    yield return description;
                }
                else
                    yield return propertyInfo.Name;
            }
        }


        private IEnumerable<PropertyInfo> OrderProperties(IEnumerable<PropertyInfo> properties, string[] propertyNames)
        {
            foreach (string str in propertyNames)
            {
                string propertyName = str;
                PropertyInfo property = properties.SingleOrDefault(p => p.Name == propertyName);
                if (property != null)
                    yield return property;
            }
        }
    }
}