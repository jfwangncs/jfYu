﻿using jfYu.Core.Office.Excel.Extensions;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace jfYu.Core.Office.Excel.Write.Implementation
{

    public class ListWriter<T>(IOptionsMonitor<JfYuExcelOption> configuration) : JfYuWriterBase<T> where T : notnull
    {
        private readonly JfYuExcelOption _configuration = configuration.CurrentValue;

        protected override void WriteDataToWorkbook(IWorkbook workbook, T source, Dictionary<string, string>? titles = null, Action<int>? callback = null)
        {
            if (!source.GetType().IsConstructedGenericType)
                throw new InvalidOperationException($"Unsupported data type {typeof(T)}.");
            var tType = source.GetType().GetGenericArguments()[0];
            IQueryable? data = null;
            if (typeof(T).GetGenericTypeDefinition() == typeof(IQueryable<>))
                data = (IQueryable)source;
            else if (typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
                data = ((IEnumerable)source).AsQueryable();
            else if (typeof(T).GetGenericTypeDefinition() == typeof(IList<>))
                data = ((IList)source).AsQueryable();
            else if (typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                data = ((IList)source).AsQueryable();
            else if (source.GetType().GetGenericTypeDefinition().Name.StartsWith("Tuple"))
            {
                var newData = ((ITuple)source);
                for (int i = 0; i < newData.Length; i++)
                {
                    if (newData[i] != null)
                    {
                        if (newData[i] is IList newItemData)
                        {
                            if (newItemData.Count > _configuration.SheetMaxRecord)
                                throw new NotSupportedException($"For write multiple sheets each sheet count need less than SheetMaxRecord:{_configuration.SheetMaxRecord}");
                            tType = newItemData.GetType().GetGenericArguments()[0];
                            Write(newItemData.AsQueryable(), workbook, tType, titles, callback);
                        }
                    }
                }
                return;
            }
            else
                throw new InvalidOperationException($"Unsupported data type {typeof(T)}.");

            Write(data, workbook, tType, titles, callback);
        }

        protected void Write(IQueryable data, IWorkbook workbook, Type tType, Dictionary<string, string>? titles, Action<int>? callback = null, bool needAutoCreateSheet = true)
        {
            if (tType.IsSimpleType())
                titles ??= new Dictionary<string, string>() { { "A", "A" } };
            titles ??= JfYuExcelExtension.GetTitles(tType);

            var sheetName = $"sheet{workbook.NumberOfSheets}";
            var sheet = workbook.CreateSheet(sheetName);
            sheet.AddTitle(titles);
            int sheetWriteRowIndex = 1;
            int writedCount = 0;
            var enumerator = data.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var dataRow = sheet.CreateRow(sheetWriteRowIndex);
                var columnIndex = 0;
                foreach (var title in titles)
                {
                    var cell = dataRow.CreateCell(columnIndex);
                    var valueType = tType.GetProperty(title.Key);
                    if (tType.IsSimpleType())
                        SetValue(tType, enumerator.Current, cell);
                    else if (valueType != null)
                        SetValue(valueType.PropertyType, valueType.GetValue(enumerator.Current), cell);
                    else
                        throw new InvalidOperationException($"Can't find title {title.Key}'s value");
                    columnIndex++;
                }
                sheetWriteRowIndex++;
                if (needAutoCreateSheet)
                {
                    if (sheetWriteRowIndex > _configuration.SheetMaxRecord)
                    {
                        sheetName = $"sheet{workbook.NumberOfSheets}";
                        sheet = workbook.CreateSheet(sheetName);
                        sheet.AddTitle(titles);
                        sheetWriteRowIndex = 1;
                    }
                }
                writedCount++;
                callback?.Invoke(writedCount);
            }
        }
    }
}
