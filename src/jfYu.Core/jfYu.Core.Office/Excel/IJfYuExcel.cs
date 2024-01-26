using NPOI.XSSF.Streaming;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace jfYu.Core.Office.Excel
{
    public interface IJfYuExcel
    {
        int RowAccessSize { get; set; }
        int SheetMaxCount { get; set; }

        /// <summary>
        /// create excel
        /// </summary> 
        /// <returns>SXSSFWorkbook</returns>
        SXSSFWorkbook CreateWorkbook();

        /// <summary>
        /// import to datatable
        /// </summary>
        /// <param name="stream">excel file stream</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        /// <returns></returns>
        DataTable GetDataTable(Stream stream, int firstRow = 1, int sheetIndex = 0);

        /// <summary>
        /// import to datatable
        /// </summary>
        /// <param name="filePath">excel file path</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        /// <returns></returns>
        DataTable GetDataTable(string filePath, int firstRow = 1, int sheetIndex = 0);

        /// <summary>
        /// import to list
        /// </summary>
        /// <param name="stream">excel file stream</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        List<T> GetList<T>(Stream stream, int firstRow = 1, int sheetIndex = 0) where T : new();

        /// <summary>
        /// import to list
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="filePath">excel file path</param>
        /// <param name="firstRow">srart index,default:1,0 is title</param>
        /// <param name="sheetIndex">sheet index</param>
        List<T> GetList<T>(string filePath, int firstRow = 1, int sheetIndex = 0) where T : new();


        /// <summary>
        /// export csv
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">data</param>
        /// <param name="filePath">file path</param>
        /// <param name="titles">title</param>
        /// <param name="callback">export progress callback</param>
        void ToCSV<T>(List<T> source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null);

        /// <summary>
        /// export      
        /// </summary>
        /// <param name="source">data</param> 
        /// <param name="filePath">file path</param>
        /// <param name="titles">titles</param>
        /// <param name="callback">export progress callback</param>
        void ToExcel(DataTable source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null);

        /// <summary>
        /// export
        /// </summary>
        /// <param name="sqlDataReader">data source</param>
        /// <param name="filePath">file path</param>
        /// <param name="titles">title</param>
        /// <param name="callback">export progress callback</param>
        void ToExcel(DbDataReader sqlDataReader, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null);

        /// <summary>
        /// export    
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">data</param
        /// <param name="filePath">file path</param>
        /// <param name="titles">titles</param>
        /// <param name="callback">export progress callback</param>
        void ToExcel<T>(IQueryable<T> source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null) where T : new();

        /// <summary>
        /// export    
        /// </summary>
        /// <typeparam name="T">model</typeparam> 
        /// <param name="source">data</param>
        /// <param name="filePath">file path</param>
        /// <param name="titles">titles</param>
        /// <param name="callback">export progress callback</param>
        void ToExcel<T>(List<T> source, string filePath, Dictionary<string, string> titles = null, Action<int> callback = null) where T : new();
    }
}