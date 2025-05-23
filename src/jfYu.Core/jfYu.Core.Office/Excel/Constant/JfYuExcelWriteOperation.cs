﻿namespace jfYu.Core.Office.Excel.Constant
{
    /// <summary>
    /// Enum representing the different write operations for Excel.
    /// </summary>
    public enum JfYuExcelWriteOperation
    {
        /// <summary>
        /// No operation.If file exist throw error.
        /// </summary>
        None,

        /// <summary>
        /// Append data to the existing content.
        /// </summary>
        Append
    }
}