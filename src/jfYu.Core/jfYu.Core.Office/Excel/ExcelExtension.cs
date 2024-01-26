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
        /// 创建sheet和表头
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="cols">列</param>
        /// <param name="titles">表头</param>
        public static SXSSFSheet CreateSheetWithTitles(this IWorkbook workbook, Dictionary<string, string> titles)
        {
            //创建sheet
            var sheet = workbook.CreateSheet();
            //表头格式
            IRow headerRow = sheet.CreateRow(0);
            ICellStyle headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            headStyle.SetFont(font);
            //设置表头 
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
            return sheet as SXSSFSheet;
        }

        /// <summary>
        /// 创建sheet和表头
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="cols">列</param>
        /// <param name="titles">表头</param>
        public static SXSSFSheet CreateSheetWithTitles<T>(this IWorkbook workbook)
        {
            var titles = GetTitles<T>();
            return workbook.CreateSheetWithTitles(titles);
        }

        /// <summary>
        /// 保存excel
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="filePath">保存地址</param>
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
        /// 获取标题
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <returns></returns>
        public static Dictionary<string, string> GetTitles<T>()
        {
            var titles = new Dictionary<string, string>();
            var pops = typeof(T).GetProperties();
            for (int i = 0; i < pops.Length; i++)
            {
                //获取displayname如无则字段名称
                var colName = pops[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? pops[i].Name;
                titles.Add(pops[i].Name, colName);
            }
            return titles;
        }

        /// <summary>
        /// 获取sheet数
        /// </summary>
        /// <param name="filePath">excel文件</param>
        /// <returns>sheetnumber</returns>
        public static int GetSheetNumber(string filePath)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            int number = 0;
            try
            {
                if (!File.Exists(filePath))
                    return number;
                using FileStream fs = File.OpenRead(filePath);
                if (filePath.IndexOf(".xlsx") > 0) // 2007版本
                    return new XSSFWorkbook(fs).NumberOfSheets;
                else if (filePath.IndexOf(".xls") > 0) // 2003版本
                    return new HSSFWorkbook(fs).NumberOfSheets;
            }
            catch (Exception)
            {
                throw;
            }
            return number;
        }

        /// <summary>
        /// 获取sheet名称
        /// </summary>
        /// <param name="outputFile">excel文件</param>
        /// <returns>sheet集合</returns>
        public static List<string> GetSheetName(string filePath)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> arrayList = [];
            try
            {
                if (!File.Exists(filePath))
                    return arrayList;
                using FileStream fs = File.OpenRead(filePath);
                IWorkbook workbook = null;
                if (filePath.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (filePath.IndexOf(".xls") > 0) // 2003版本
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
        /// 验证Excel是否有数据
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public static bool HasData(string filePath)
        {
            using FileStream fs = File.OpenRead(filePath);
            IWorkbook workbook = null;
            if (filePath.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook(fs);
            else if (filePath.IndexOf(".xls") > 0) // 2003版本
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
        /// 转化Model
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public static List<T> ToModel<T>(DataTable dt) where T : new()
        {
            List<T> list = [];
            foreach (DataRow r in dt.Rows)
            {
                list.Add(ToModel<T>(r));
            }
            return list;

        }

        /// <summary>
        /// 转化Model
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="row">数据行</param>
        /// <returns></returns>
        public static T ToModel<T>(DataRow row) where T : new()
        {
            T item = (T)Activator.CreateInstance(typeof(T));
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = typeof(T).GetProperty(c.ColumnName);
                try
                { // if exists, set the value
                    if (p != null && row[c] != DBNull.Value)
                    {
                        if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(item, int.Parse(row[c].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(long))
                        {
                            p.SetValue(item, long.Parse(row[c].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(double))
                        {
                            p.SetValue(item, double.Parse(row[c].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(decimal))
                        {
                            p.SetValue(item, decimal.Parse(row[c].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            p.SetValue(item, float.Parse(row[c].ToString()), null);
                        }
                        else
                        {
                            p.SetValue(item, row[c], null);
                        }
                    }
                }
                catch (ArgumentException)
                {

                    throw new Exception($"{p.Name}格式和数据库不一致,数据库格式{c.DataType.Name}，Model格式{p.PropertyType.Name}");
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
