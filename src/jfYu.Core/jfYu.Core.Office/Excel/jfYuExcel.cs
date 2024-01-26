using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace jfYu.Core.Office.Excel
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class JfYuExcel : IJfYuExcel
    {
        /// <summary>
        /// SXSSF row count in memory
        /// </summary>
        public int RowAccessSize { get; set; } = 1000;

        /// <summary>
        /// one sheet max count default:1000000
        /// </summary>
        public int SheetMaxCount { get; set; } = 1000000;

        #region export    

        private void ToExcel(string filePath, Dictionary<string, string> titles, Action<IWorkbook, ISheet> main)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new NullReferenceException($"{filePath} is null");

            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".xlsx";
            else
                filePath = filePath.Replace(ext, ".xlsx");
            var workbook = CreateWorkbook();
            var sheet = workbook.CreateSheetWithTitles(titles);
            main.Invoke(workbook, sheet);
            workbook.Save(filePath);

        }

        /// <summary>
        /// export    
        /// </summary>
        /// <typeparam name="T">model</typeparam> 
        /// <param name="source">data</param>
        /// <param name="filePath">file path</param>
        /// <param name="titles">titles</param>
        /// <param name="callback">export progress callback</param>
        public void ToExcel<T>(List<T> source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null) where T : new()
        {
            titles ??= ExcelExtension.GetTitles<T>();
            ToExcel(filePath, titles, (workbook, sheet) =>
            {
                //current row index
                int sheetWriteRowIndex = 1;
                //writed row count
                int writedCount = 0;
                //start write
                foreach (var item in source)
                {
                    var dataRow = sheet.CreateRow(sheetWriteRowIndex);
                    var columnIndex = 0;
                    foreach (var title in titles)
                    {
                        var cell = dataRow.CreateCell(columnIndex);
                        try
                        {
                            var value = typeof(T).GetProperty(title.Key)?.GetValue(item)?.ToString() ?? "";
                            cell.SetCellValue(value);
                        }
                        catch (Exception)
                        {
                            cell.SetCellValue("");
                        }
                        columnIndex++;
                    }
                    sheetWriteRowIndex++;
                    if (sheetWriteRowIndex > SheetMaxCount)
                    {
                        sheet = workbook.CreateSheetWithTitles(titles);
                        sheetWriteRowIndex = 1;
                    }
                    writedCount++;
                    callback?.Invoke(writedCount);
                }
            });
        }

        /// <summary>
        /// export      
        /// </summary>
        /// <param name="source">data</param> 
        /// <param name="filePath">file path</param>
        /// <param name="titles">titles</param>
        /// <param name="callback">export progress callback</param>
        public void ToExcel(DataTable source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null)
        {
            if (titles == null)
            {
                titles = [];
                //获取表头字段
                foreach (DataColumn item in source.Columns)
                    titles.Add(item.ColumnName, item.ColumnName);
            }

            ToExcel(filePath, titles, (workbook, sheet) =>
            {
                //current row index
                int sheetWriteRowIndex = 1;
                //writed row count
                int writedCount = 0;
                //start write
                foreach (DataRow item in source.Rows)
                {
                    var dataRow = sheet.CreateRow(sheetWriteRowIndex);
                    var columnIndex = 0;
                    foreach (var title in titles)
                    {
                        var cell = dataRow.CreateCell(columnIndex);
                        try
                        {
                            cell.SetCellValue(item[title.Key]?.ToString() ?? "");
                        }
                        catch (Exception)
                        {
                            cell.SetCellValue("");
                        }
                        columnIndex++;
                    }
                    sheetWriteRowIndex++;
                    if (sheetWriteRowIndex > SheetMaxCount)
                    {
                        sheet = workbook.CreateSheetWithTitles(titles);
                        sheetWriteRowIndex = 1;
                    }
                    writedCount++;
                    callback?.Invoke(writedCount);
                }
            });
        }

        /// <summary>
        /// export    
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">data</param
        /// <param name="filePath">file path</param>
        /// <param name="titles">titles</param>
        /// <param name="callback">export progress callback</param>
        public void ToExcel<T>(IQueryable<T> source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null) where T : new()
        {
            titles ??= ExcelExtension.GetTitles<T>();
            ToExcel(filePath, titles, (workbook, sheet) =>
            {
                //current row index
                int sheetWriteRowIndex = 1;
                //writed row count
                int writedCount = 0;
                //start write
                var enumerator = source.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var dataRow = sheet.CreateRow(sheetWriteRowIndex);
                    var columnIndex = 0;
                    foreach (var title in titles)
                    {
                        var cell = dataRow.CreateCell(columnIndex);
                        try
                        {
                            var value = typeof(T).GetProperty(title.Key)?.GetValue(enumerator.Current)?.ToString() ?? "";
                            cell.SetCellValue(value);
                        }
                        catch (Exception)
                        {
                            cell.SetCellValue("");
                        }
                        columnIndex++;
                    }
                    sheetWriteRowIndex++;
                    if (sheetWriteRowIndex > SheetMaxCount)
                    {
                        sheet = workbook.CreateSheetWithTitles(titles);
                        sheetWriteRowIndex = 1;
                    }
                    writedCount++;
                    callback?.Invoke(writedCount);
                }
            });
        }

        /// <summary>
        /// export
        /// </summary>
        /// <param name="sqlDataReader">data source</param>
        /// <param name="filePath">file path</param>
        /// <param name="titles">title</param>
        /// <param name="callback">export progress callback</param>
        public void ToExcel(DbDataReader sqlDataReader, string filePath, Dictionary<string, string> titles, Action<int> callback = null)
        {
            if (sqlDataReader.IsClosed)
                throw new Exception("data reader is closed.");
            if (titles is null || titles.Count <= 0)
                throw new NullReferenceException("title is null.");
            ToExcel(filePath, titles, (workbook, sheet) =>
            {
                //current row index
                int sheetWriteRowIndex = 1;
                //writed row count
                int writedCount = 0;
                //start write
                while (sqlDataReader.Read())
                {
                    var dataRow = sheet.CreateRow(sheetWriteRowIndex);
                    var columnIndex = 0;
                    foreach (var title in titles)
                    {
                        var cell = dataRow.CreateCell(columnIndex);
                        try
                        {
                            cell.SetCellValue(sqlDataReader[title.Key]?.ToString() ?? "");
                        }
                        catch (Exception)
                        {
                            cell.SetCellValue("");
                        }
                        columnIndex++;
                    }
                    sheetWriteRowIndex++;
                    if (sheetWriteRowIndex > 1000000)
                    {
                        sheet = workbook.CreateSheetWithTitles(titles);
                        sheetWriteRowIndex = 1;
                    }
                    writedCount++;
                    callback?.Invoke(writedCount);
                }
                sqlDataReader.Close();
            });
        }

        /// <summary>
        /// export csv
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">data</param>
        /// <param name="filePath">file path</param>
        /// <param name="titles">title</param>
        /// <param name="callback">export progress callback</param>
        public void ToCSV<T>(List<T> source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new NullReferenceException($"{filePath} is null");

            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
                filePath += ".csv";
            else
                filePath = filePath.Replace(ext, ".csv");

            using FileStream fs = File.Create(filePath);
            using StreamWriter sw = new(fs, Encoding.UTF8);
            StringBuilder str = new();
            titles ??= ExcelExtension.GetTitles<T>();
            //title
            foreach (var item in titles)
            {
                str.Append(item.Value + ",");
            }
            sw.WriteLine(str.ToString().Trim(','));
            //writed row count
            int writedCount = 0;
            //start write
            foreach (var item in source)
            {
                string rowStr = "";
                var columnIndex = 0;
                foreach (var title in titles)
                {
                    var value = typeof(T).GetProperty(title.Key)?.GetValue(item)?.ToString() ?? "";
                    if (value.Contains(","))
                        rowStr += value.Replace(",", "\",\"") + ",";
                    else
                        rowStr += value + ",";
                    columnIndex++;
                }
                sw.WriteLine(rowStr.ToString().Trim(','));
                writedCount++;
                callback?.Invoke(writedCount);
            }
        }

        #endregion

        #region excel import

        /// <summary>
        /// import to datatable
        /// </summary>
        /// <param name="filePath">excel file path</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        /// <returns></returns>
        public DataTable GetDataTable(string filePath, int firstRow = 1, int sheetIndex = 0)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"{filePath} file not found");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IWorkbook wb;
            using FileStream file = new(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                wb = WorkbookFactory.Create(file);
                ISheet sheet = wb.GetSheetAt(sheetIndex);
                return GetDataTable(sheet, firstRow);
            }
            catch (Exception ex)
            {
                throw new Exception($"import failed,{ex.Message},{ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// import to datatable
        /// </summary>
        /// <param name="stream">excel file stream</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        /// <returns></returns>
        public DataTable GetDataTable(Stream stream, int firstRow = 1, int sheetIndex = 0)
        {
            if (stream == null)
                throw new ArgumentNullException("steam is null");
            try
            {
                DataTable dt = new();
                IWorkbook wb;
                using (stream)
                    wb = WorkbookFactory.Create(stream);
                ISheet sheet = wb.GetSheetAt(sheetIndex);
                return GetDataTable(sheet, firstRow);
            }
            catch (Exception ex)
            {
                throw new Exception($"import failed,{ex.Message},{ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// import to list
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="filePath">excel file path</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        /// <returns></returns>
        public List<T> GetList<T>(string filePath, int firstRow = 1, int sheetIndex = 0) where T : new()
        {
            return ExcelExtension.ToModel<T>(GetDataTable(filePath, firstRow, sheetIndex));
        }

        /// <summary>
        /// import to list
        /// </summary>
        /// <param name="stream">excel file stream</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        /// <returns></returns>
        public List<T> GetList<T>(Stream stream, int firstRow = 1, int sheetIndex = 0) where T : new()
        {
            return ExcelExtension.ToModel<T>(GetDataTable(stream, firstRow, sheetIndex));
        }

        /// <summary>
        /// get data from sheeet
        /// </summary>
        /// <param name="sheet">sheet</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <returns></returns>
        private DataTable GetDataTable(ISheet sheet, int firstRow)
        {
            DataTable table = new();
            IRow headerRow;
            int cellCount;
            try
            {
                //title
                headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    return table;
                cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                //content
                for (int i = firstRow; i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        IRow row = sheet.GetRow(i);
                        if (sheet.GetRow(i) != null)
                        {
                            DataRow dataRow = table.NewRow();

                            for (int j = row.FirstCellNum; j <= cellCount; j++)
                            {
                                try
                                {
                                    if (row.GetCell(j) != null)
                                    {
                                        switch (row.GetCell(j).CellType)
                                        {
                                            case CellType.Numeric:
                                                if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                                    dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                                else
                                                    dataRow[j] = Convert.ToDecimal(row.GetCell(j).NumericCellValue);
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
                                                    case CellType.String:
                                                        string strFORMULA = row.GetCell(j).StringCellValue;
                                                        dataRow[j] = strFORMULA?.ToString();
                                                        break;
                                                }
                                                break;
                                            default:
                                            case CellType.String:
                                                string str = row.GetCell(j).StringCellValue;
                                                dataRow[j] = str?.ToString();
                                                break;
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                            table.Rows.Add(dataRow);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"import failed,{ex.Message},{ex.InnerException?.Message}");
            }
            return table;
        }

        #endregion

        #region create

        /// <summary>
        /// create excel
        /// </summary> 
        /// <returns>SXSSFWorkbook</returns>
        public SXSSFWorkbook CreateWorkbook()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var workbook = new XSSFWorkbook();
            return new SXSSFWorkbook(workbook, RowAccessSize);
        }
        #endregion
    }
}
