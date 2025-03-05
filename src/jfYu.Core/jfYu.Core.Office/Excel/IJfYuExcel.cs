using jfYu.Core.Office.Excel.Constant;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace jfYu.Core.Office.Excel
{
    /// <summary>
    /// IJfYuExcel
    /// </summary>
    public interface IJfYuExcel
    {
        /// <summary>
        /// Create excel
        /// </summary> 
        /// <returns>Excel object</returns>
        IWorkbook CreateExcel(JfYuExcelVersion excelVersion = JfYuExcelVersion.Xlsx);

        /// <summary>
        /// Read one sheet of excel
        /// </summary>
        /// <param name="stream">File stream</param>
        /// <param name="firstRow">Start index,default:1,0 is title</param>
        /// <param name="sheetIndex">Sheet index</param>
        /// <returns>The list of sheet</returns>
        T Read<T>(Stream stream, int firstRow = 1, int sheetIndex = 0);

        /// <summary>
        /// Read one sheet of excel
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="filePath">Excel file path</param>
        /// <param name="firstRow">Start index,default:1,0 is title</param>
        /// <param name="sheetIndex">Sheet index</param>
        /// <returns>The list of sheet</returns>
        T Read<T>(string filePath, int firstRow = 1, int sheetIndex = 0);

        /// <summary>
        /// Update the options for the Excel
        /// </summary>
        /// <param name="updateAction">Action to update the options</param>
        void UpdateOption(Action<JfYuExcelOption> updateAction);

        /// <summary>
        /// Export data as xlsx
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filePath"></param>
        /// <param name="titles"></param>
        /// <param name="writeOperation"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IWorkbook Write<T>(T source, string filePath, Dictionary<string, string>? titles = null, JfYuExcelWriteOperation writeOperation = JfYuExcelWriteOperation.None, Action<int>? callback = null);

        /// <summary>
        /// Export data as CSV
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="source">Data to export</param>
        /// <param name="filePath">File path to save the CSV</param>
        /// <param name="titles">Title mapping for the CSV columns</param>
        /// <param name="callback">Export progress callback</param>
        void WriteCSV<T>(List<T> source, string filePath, Dictionary<string, string>? titles = null, Action<int>? callback = null);

        /// <summary>
        /// Read CSV file
        /// </summary>
        /// <param name="filePath">CSV file path</param>
        /// <param name="firstRow">Start index, default: 1, 0 is title</param>
        /// <returns>The list of CSV rows</returns>
        List<dynamic> ReadCSV(string filePath, int firstRow = 1);
    }
}