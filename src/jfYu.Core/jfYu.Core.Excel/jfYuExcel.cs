using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace jfYu.Core.Excel
{
    public sealed class JfYuExcel
    {

        /// <summary>
        /// SXSSF内存保存行数
        /// </summary>
        public int RowAccessSize { get; set; } = 1000;

        #region 导出到excel
        /// <summary>
        /// 导出到excel    
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToExcel<T>(List<T> source, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            //创建工作簿
            var workbook = Getworkbook<T>(filePath);
            //获取默认sheet
            var sheet = workbook.GetSheetAt(0);
            //获取model字段
            var pops = typeof(T).GetProperties();
            //总行数
            int RowCount = source.Count;
            //该sheet当前写入行
            int SheetWriteRowIndex = 1;
            //所有写入行
            int AllWriteRowIndex = 0;
            //写入数据
            foreach (var item in source)
            {
                #region 填充内容
                var dataRow = sheet.CreateRow(SheetWriteRowIndex);
                for (int i = 0; i < pops.Length; i++)
                {
                    var cell = dataRow.CreateCell(i);
                    try
                    {
                        cell.SetCellValue(pops[i].GetValue(item)?.ToString() ?? "");
                    }
                    catch (Exception)
                    {
                        cell.SetCellValue("");
                    }

                }
                #endregion

                SheetWriteRowIndex++;
                //数据量大于100万 另外开起一个sheet
                if (SheetWriteRowIndex > 1000000)
                {
                    sheet = workbook.CreateSheet();
                    CreateHeader<T>(workbook, sheet);
                    SheetWriteRowIndex = 1;
                }
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);
            }
            using FileStream fs = File.OpenWrite(filePath);
            workbook.Write(fs);
            fs.Close();
            //释放以便于删除缓存文件
            workbook.Dispose();
        }

        /// <summary>
        /// 导出到excel    
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">源数据表</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToExcel<T>(DataTable source, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            //创建工作簿
            var workbook = Getworkbook<T>(filePath);
            //获取默认sheet
            var sheet = workbook.GetSheetAt(0);
            //获取model字段
            var pops = typeof(T).GetProperties();
            //总行数
            int RowCount = source.Rows.Count;
            //该sheet当前写入行
            int SheetWriteRowIndex = 1;
            //所有写入行
            int AllWriteRowIndex = 0;

            //写入数据
            foreach (DataRow item in source.Rows)
            {
                #region 填充内容

                var dataRow = sheet.CreateRow(SheetWriteRowIndex);
                for (int i = 0; i < pops.Length; i++)
                {
                    var cell = dataRow.CreateCell(i);
                    try
                    {
                        cell.SetCellValue(item[pops[i].Name]?.ToString() ?? "");

                    }
                    catch (Exception)
                    {
                        cell.SetCellValue("");
                    }
                }
                #endregion

                SheetWriteRowIndex++;
                //数据量大于100万 另外开起一个sheet
                if (SheetWriteRowIndex > 1000000)
                {
                    sheet = workbook.CreateSheet();
                    CreateHeader<T>(workbook, sheet);
                    SheetWriteRowIndex = 1;
                }
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);
            }
            using FileStream fs = File.OpenWrite(filePath);
            workbook.Write(fs);
            fs.Close();
            //释放以便于删除缓存文件
            workbook.Dispose();
        }

        /// <summary>
        /// 导出到excel   
        /// </summary>
        /// <param name="source">源数据表</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToExcel(DataTable source, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            List<string> pops = new List<string>();
            //获取表头字段
            foreach (DataColumn item in source.Columns)
                pops.Add(item.ColumnName);
            //创建工作簿
            var workbook = Getworkbook(filePath, pops);
            //获取默认sheet
            var sheet = workbook.GetSheetAt(0);
            //总行数
            int RowCount = source.Rows.Count;
            //该sheet当前写入行
            int SheetWriteRowIndex = 1;
            //所有写入行
            int AllWriteRowIndex = 0;
            //写入数据
            foreach (DataRow item in source.Rows)
            {
                #region 填充内容

                var dataRow = sheet.CreateRow(SheetWriteRowIndex);
                for (int i = 0; i < pops.Count; i++)
                {
                    var cell = dataRow.CreateCell(i);
                    try
                    {
                        cell.SetCellValue(item[pops[i]]?.ToString() ?? "");
                    }
                    catch (Exception)
                    {
                        cell.SetCellValue("");
                    }
                }
                #endregion

                SheetWriteRowIndex++;
                //数据量大于100万 另外开起一个sheet
                if (SheetWriteRowIndex > 1000000)
                {
                    sheet = workbook.CreateSheet();
                    CreateHeader(pops, workbook, sheet);
                    SheetWriteRowIndex = 1;
                }
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);
            }
            using FileStream fs = File.OpenWrite(filePath);
            workbook.Write(fs);
            fs.Close();
            //释放以便于删除缓存文件
            workbook.Dispose();
        }

        /// <summary>
        /// 导出到excel    
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">IQueryable数据</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToExcel<T>(IQueryable<T> souce, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            //创建工作簿
            var workbook = Getworkbook<T>(filePath);
            //获取默认sheet
            var sheet = workbook.GetSheetAt(0);
            //获取model字段
            var pops = typeof(T).GetProperties();

            int SheetWriteRowIndex = 1;
            int AllWriteRowIndex = 0;

            //写入数据
            var enumerator = souce.GetEnumerator();
            while (enumerator.MoveNext())
            {
                #region 填充内容

                var dataRow = sheet.CreateRow(SheetWriteRowIndex);
                for (int i = 0; i < pops.Length; i++)
                {
                    var cell = dataRow.CreateCell(i);
                    try
                    {
                        cell.SetCellValue(pops[i].GetValue(enumerator.Current)?.ToString() ?? "");
                    }
                    catch (Exception)
                    {
                        cell.SetCellValue("");
                    }
                }
                #endregion

                SheetWriteRowIndex++;
                //数据量大于100万 另外开起一个sheet
                if (SheetWriteRowIndex > 1000000)
                {
                    sheet = workbook.CreateSheet();
                    CreateHeader<T>(workbook, sheet);
                    SheetWriteRowIndex = 1;
                }
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);
            }
            using FileStream fs = File.OpenWrite(filePath);
            workbook.Write(fs);
            fs.Close();
            //释放以便于删除缓存文件
            workbook.Dispose();
        }

        /// <summary>
        /// 导出到excel    
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">sqldatareader源</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToExcel<T>(DbDataReader sqlDataReader, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            //创建工作簿
            var workbook = Getworkbook<T>(filePath);
            //获取默认sheet
            var sheet = workbook.GetSheetAt(0);
            //获取model字段
            var pops = typeof(T).GetProperties();

            int SheetWriteRowIndex = 1;
            int AllWriteRowIndex = 0;

            //写入数据
            while (sqlDataReader.Read())
            {
                #region 填充内容

                var dataRow = sheet.CreateRow(SheetWriteRowIndex);
                for (int i = 0; i < pops.Length; i++)
                {
                    var cell = dataRow.CreateCell(i);

                    try
                    {
                        cell.SetCellValue(sqlDataReader[pops[i].Name]?.ToString() ?? "");
                    }
                    catch (Exception)
                    {
                        cell.SetCellValue("");
                    }

                    //Type valueType = pops[i].PropertyType;
                }
                #endregion

                SheetWriteRowIndex++;
                //数据量大于100万 另外开起一个sheet
                if (SheetWriteRowIndex > 1000000)
                {
                    sheet = workbook.CreateSheet();
                    CreateHeader<T>(workbook, sheet);
                    SheetWriteRowIndex = 1;
                }
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);

            }
            using FileStream fs = File.OpenWrite(filePath);
            workbook.Write(fs);
            fs.Close();
            //释放以便于删除缓存文件
            workbook.Dispose();
        }

        /// <summary>
        /// 导出到excel
        /// </summary>
        /// <param name="sqlDataReader">sqldatareader源</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToExcel(DbDataReader sqlDataReader, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            //获取表头字段
            var pops = sqlDataReader.GetColumnSchema().Select(q => q.ColumnName).ToList();
            //创建工作簿
            var workbook = Getworkbook(filePath, pops);
            //获取默认sheet
            var sheet = workbook.GetSheetAt(0); ;

            int SheetWriteRowIndex = 1;
            int AllWriteRowIndex = 0;

            //写入数据
            while (sqlDataReader.Read())
            {
                #region 填充内容

                var dataRow = sheet.CreateRow(SheetWriteRowIndex);
                for (int i = 0; i < pops.Count; i++)
                {
                    var cell = dataRow.CreateCell(i);
                    try
                    {
                        cell.SetCellValue(sqlDataReader[pops[i]]?.ToString() ?? "");
                    }
                    catch (Exception)
                    {
                        cell.SetCellValue("");
                    }
                }
                #endregion

                SheetWriteRowIndex++;
                //数据量大于100万 另外开起一个sheet
                if (SheetWriteRowIndex > 1000000)
                {
                    sheet = workbook.CreateSheet();
                    CreateHeader(pops, workbook, sheet);
                    SheetWriteRowIndex = 1;
                }
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);

            }
            using FileStream fs = File.OpenWrite(filePath);
            workbook.Write(fs);
            fs.Close();
            //释放以便于删除缓存文件
            workbook.Dispose();
        }
        #endregion

        #region 导出到CSV
        /// <summary>
        /// 导出到csv
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">源数据表</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToCSV<T>(List<T> source, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".csv";
            else
                filePath = filePath.Replace(ext, ".csv");
            FileStream fs = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            StringBuilder str = new StringBuilder();
            //获取model字段
            var pops = typeof(T).GetProperties();

            //写出列名称
            foreach (var item in pops)
            {
                str.Append(item.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? item.Name + ",");

            }
            sw.WriteLine(str.ToString().Trim(','));


            //总行数
            int RowCount = source.Count;
            //所有写入行
            int AllWriteRowIndex = 0;
            //写入数据
            foreach (var item in source)
            {
                #region 填充内容
                string rowStr = "";
                for (int i = 0; i < pops.Length; i++)
                {
                    var value = pops[i].GetValue(item)?.ToString() ?? "";
                    if (value.Contains(","))
                        rowStr += (value.Replace(",", "\",\"") + ",");
                    else
                        rowStr += (value + ",");
                }
                #endregion
                sw.WriteLine(rowStr.ToString().Trim(','));
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 导出到csv
        /// </summary>
        /// <param name="source">源数据表</param>
        /// <param name="filePath">保存地址</param>
        /// <param name="callback">进度回调</param>
        public void ToCSV(DataTable source, string filePath, Action<int> callback = null)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".csv";
            else
                filePath = filePath.Replace(ext, ".csv");
            FileStream fs = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            StringBuilder str = new StringBuilder();
            //获取model字段
            List<string> pops = new List<string>();
            //获取表头字段
            foreach (DataColumn item in source.Columns)
                pops.Add(item.ColumnName);

            //写出列名称
            foreach (var item in pops)
            {
                str.Append(item + ",");

            }
            sw.WriteLine(str.ToString().Trim(','));

            //总行数
            int RowCount = source.Rows.Count;
            //所有写入行
            int AllWriteRowIndex = 0;
            //写入数据
            foreach (DataRow item in source.Rows)
            {
                #region 填充内容
                string rowStr = "";
                for (int i = 0; i < pops.Count; i++)
                {
                    var value = item[pops[i]]?.ToString() ?? "";
                    if (value.Contains(","))
                        rowStr += (value.Replace(",", "\",\"") + ",");
                    else
                        rowStr += (value + ",");
                }
                #endregion

                sw.WriteLine(rowStr.ToString().Trim(','));
                AllWriteRowIndex++;
                callback?.Invoke(AllWriteRowIndex);
            }
            sw.Close();
            fs.Close();
        }
        #endregion

        #region excel导入

        /// <summary>
        /// Excel导入到DataTable
        /// </summary>
        /// <param name="filePath">excel文件</param>
        /// <param name="firstRowIsHeader">首行是否为表头</param>
        /// <param name="sheetindex">读取的sheet</param>
        /// <returns></returns>
        public DataTable ToDataTable(string filePath, bool firstRowIsHeader = true, int sheetindex = 0)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"找不到{filePath}文件");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IWorkbook wb;
            using FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            wb = WorkbookFactory.Create(file);
            try
            {
                ISheet sheet = wb.GetSheetAt(sheetindex);
                return GetDataTable(sheet, firstRowIsHeader);
            }
            catch (Exception)
            {
                throw new Exception($"导出失败，检查Excel文件是否存在数据");
            }
        }

        /// <summary>
        /// Excel导入到DataTable
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        /// <returns></returns>
        public DataTable ToDataTable(Stream stream, bool firstRowIsHeader = true, int sheetindex = 0)
        {
            try
            {
                DataTable dt = new DataTable();
                IWorkbook wb;
                using (stream)
                {
                    wb = WorkbookFactory.Create(stream);
                }
                ISheet sheet = wb.GetSheetAt(sheetindex);
                return GetDataTable(sheet, firstRowIsHeader);
            }
            catch (Exception)
            {
                throw new Exception($"导出失败，检查Excel文件是否存在数据");
            }
        }


        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        DataTable GetDataTable(ISheet sheet, bool firstRowIsHeader)
        {
            DataTable table = new DataTable();
            IRow headerRow;
            int cellCount;
            try
            {
                //表头
                headerRow = sheet.GetRow(0);
                cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }


                int firstRow = 0;
                if (firstRowIsHeader)
                    firstRow = 1;

                //内容
                int rowCount = sheet.LastRowNum;
                for (int i = firstRow; i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        IRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i);
                        }
                        else
                        {
                            row = sheet.GetRow(i);
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = null;
                                            }
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula:
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("导如失败");
            }
            return table;
        }

        #endregion

        #region 工具方法
        public SXSSFWorkbook Getworkbook<T>(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs;
            try
            {
                var dir = filePath.Replace(Path.GetFileName(filePath), "");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                fs = File.Create(filePath);
            }
            catch (Exception ex)
            {

                throw ex;
            }


            //创建工作簿
            IWorkbook workbook = new XSSFWorkbook();

            //创建第一个sheet
            ISheet sheet = workbook.CreateSheet();

            // 创建表头
            CreateHeader<T>(workbook, sheet);

            workbook.Write(fs);


            return new SXSSFWorkbook(workbook as XSSFWorkbook, RowAccessSize);
        }

        public SXSSFWorkbook Getworkbook(string filePath, List<string> cols)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs;
            try
            {
                var dir = filePath.Replace(Path.GetFileName(filePath), "");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                fs = File.Create(filePath);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //创建工作簿
            var workbook = new XSSFWorkbook();

            //创建第一个sheet
            var sheet = workbook.CreateSheet();

            //创建表头
            CreateHeader(cols, workbook, sheet);

            workbook.Write(fs);

            return new SXSSFWorkbook(workbook as XSSFWorkbook, RowAccessSize);

        }
        public void CreateHeader<T>(IWorkbook workbook, ISheet sheet)
        {

            //表头格式
            var headerRow = sheet.CreateRow(0);
            var headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            headStyle.SetFont(font);
            //设置表头
            var pops = typeof(T).GetProperties();
            for (int i = 0; i < pops.Length; i++)
            {
                //获取displayname如无则字段名称
                var colName = pops[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? pops[i].Name;
                var colLength = Encoding.UTF8.GetBytes(colName).Length;
                headerRow.CreateCell(i).SetCellValue(colName);
                headerRow.GetCell(i).CellStyle = headStyle;
                sheet.SetColumnWidth(i, colLength > 100 ? 100 * 256 : colLength < 20 ? 10 * 256 : colLength * 256);
            }

        }
        public void CreateHeader(List<string> cols, IWorkbook workbook, ISheet sheet)
        {
            //表头格式
            IRow headerRow = sheet.CreateRow(0);
            ICellStyle headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            headStyle.SetFont(font);
            //设置表头

            for (int i = 0; i < cols.Count; i++)
            {
                //获取displayname如无则字段名称
                var colName = cols[i];
                headerRow.CreateCell(i).SetCellValue(colName);
                var colLength = Encoding.UTF8.GetBytes(colName).Length;
                sheet.SetColumnWidth(i, colLength > 100 ? 100 * 256 : colLength < 20 ? 10 * 256 : colLength * 256);
                headerRow.GetCell(i).CellStyle = headStyle;
            }
        }
        /// <summary>
        /// 获取sheet数
        /// </summary>
        /// <param name="filePath">excel文件</param>
        /// <returns>sheetnumber</returns>
        public int GetSheetNumber(string filePath)
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
            catch (Exception ex)
            {
                throw ex;
            }
            return number;
        }

        /// <summary>
        /// 获取sheet名称
        /// </summary>
        /// <param name="outputFile">excel文件</param>
        /// <returns>sheet集合</returns>
        public List<string> GetSheetName(string filePath)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            List<string> arrayList = new List<string>();
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
            catch (Exception ex)
            {
                throw ex;
            }
            return arrayList;
        }

        /// <summary>
        ///是否数字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool IsNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// 验证Excel是否有数据
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public bool HasData(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
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
            }
            return false;
        }
        #endregion

    }
}
