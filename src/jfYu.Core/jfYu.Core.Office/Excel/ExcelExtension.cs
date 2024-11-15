using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace jfYu.Core.Office.Excel
{
    public static class ExcelExtension
    {
        #region Excel          

        /// <summary>
        /// create sheet With titles
        /// </summary>
        /// <param name="workbook">workbook</param> 
        /// <param name="titles">titles</param>
        public static SXSSFSheet CreateSheetWithTitles(this IWorkbook workbook, Dictionary<string, string> titles)
        {
            var sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);
            ICellStyle headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            headStyle.SetFont(font);
            var i = 0;
            foreach (var item in titles)
            {
                var header = item.Value;
                headerRow.CreateCell(i).SetCellValue(header);
                var colLength = Encoding.UTF8.GetBytes(header).Length;
                sheet.SetColumnWidth(i, colLength > 100 ? 100 * 256 : colLength < 20 ? 10 * 256 : colLength * 256);
                headerRow.GetCell(i).CellStyle = headStyle;
                i++;
            }
            return (SXSSFSheet)sheet;
        }

        /// <summary>
        /// create sheet
        /// </summary>
        /// <param name="workbook">workbook</param> 
        public static SXSSFSheet CreateSheetWithTitles<T>(this IWorkbook workbook)
        {
            var titles = GetTitles<T>();
            return workbook.CreateSheetWithTitles(titles);
        }

        /// <summary>
        /// save
        /// </summary>
        /// <param name="workbook">workbook</param>
        /// <param name="filePath">file path</param>
        public static void Save(this IWorkbook workbook, string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs;
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                fs = File.Create(filePath);
                workbook.Write(fs);
            }
            catch (Exception)
            {
                throw;
            }
            workbook.Close();
            workbook.Dispose();
            fs.Close();
            fs.Dispose();
        }
        #endregion

        #region 工具方法

        /// <summary>
        /// get titles
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <returns></returns>
        public static Dictionary<string, string> GetTitles<T>()
        {
            var titles = new Dictionary<string, string>();
            var pops = typeof(T).GetProperties();
            for (int i = 0; i < pops.Length; i++)
            {
                //get displayname if no get property's name
                var colName = pops[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? pops[i].Name;
                titles.Add(pops[i].Name, colName);
            }
            return titles;
        }

        /// <summary>
        /// get count of sheet
        /// </summary>
        /// <param name="filePath">excel file path</param>
        /// <returns></returns>
        public static int GetSheetNumber(string filePath)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int number = 0;
            try
            {
                if (!File.Exists(filePath))
                    return number;
                using FileStream fs = File.OpenRead(filePath);
                if (filePath.IndexOf(".xlsx") > 0) // 2007 
                    return new XSSFWorkbook(fs).NumberOfSheets;
                else if (filePath.IndexOf(".xls") > 0) // 2003
                    return new HSSFWorkbook(fs).NumberOfSheets;
            }
            catch (Exception)
            {
                throw;
            }
            return number;
        }

        /// <summary>
        /// get sheet name
        /// </summary>
        /// <param name="filePath">excel file path</param>
        /// <returns></returns>
        public static List<string> GetSheetName(string filePath)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> arrayList = [];
            try
            {
                if (!File.Exists(filePath))
                    return arrayList;
                using FileStream fs = File.OpenRead(filePath);
                IWorkbook? workbook = null;
                if (filePath.IndexOf(".xlsx") > 0) // 2007
                    workbook = new XSSFWorkbook(fs);
                else if (filePath.IndexOf(".xls") > 0) // 2003
                    workbook = new HSSFWorkbook(fs);
                if (workbook != null)
                {
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                        arrayList.Add(workbook.GetSheetName(i));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return arrayList;
        }

        /// <summary>
        /// verify sheet have data or not
        /// </summary>
        /// <param name="filePath">excel file path</param>
        /// <returns></returns>
        public static bool HasData(string filePath)
        {
            using FileStream fs = File.OpenRead(filePath);
            IWorkbook? workbook = null;
            if (filePath.IndexOf(".xlsx") > 0) // 2007
                workbook = new XSSFWorkbook(fs);
            else if (filePath.IndexOf(".xls") > 0) // 2003
                workbook = new HSSFWorkbook(fs);
            if (workbook != null)
            {
                if (workbook.NumberOfSheets > 0)
                {
                    ISheet sheet = workbook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// to model
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="dt">datatable</param>
        /// <returns></returns>
        public static List<T> ToModel<T>(DataTable dt) where T : new()
        {
            List<T> list = [];
            foreach (DataRow r in dt.Rows)
            {
                var data = ToModel<T>(r);
                if (data != null)
                    list.Add(data);
            }
            return list;

        }

        /// <summary>
        /// to model
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="row">data row</param>
        /// <returns></returns>
        public static T? ToModel<T>(DataRow row) where T : new()
        {
            var obj = Activator.CreateInstance(typeof(T));
            if (obj == null)
                return default;
            T item = (T)obj;
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo? p = typeof(T)?.GetProperty(c.ColumnName);
                try
                { // if exists, set the value
                    if (p != null && row != null && c != null && row[c] != DBNull.Value && row[c] != null)
                    {

                        string value = row[c].ToString() ?? "";
                        if (value != null)
                        {
                            if (p.PropertyType == typeof(int))
                            {
                                p.SetValue(item, int.Parse(value), null);
                            }
                            else if (p.PropertyType == typeof(long))
                            {
                                p.SetValue(item, long.Parse(value), null);
                            }
                            else if (p.PropertyType == typeof(double))
                            {
                                p.SetValue(item, double.Parse(value), null);
                            }
                            else if (p.PropertyType == typeof(decimal))
                            {
                                p.SetValue(item, decimal.Parse(value), null);
                            }
                            else if (p.PropertyType == typeof(float))
                            {
                                p.SetValue(item, float.Parse(value), null);
                            }
                            else
                            {
                                p.SetValue(item, row[c], null);
                            }
                        }
                        else
                        {
                            p.SetValue(item, row[c], null);
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    throw new Exception($"convert {p?.Name} get error,db format:{c.DataType.Name}，model format:{p?.PropertyType.Name},errorMsg:{ex.Message}");
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return item;

        }
        #endregion

    }
}
