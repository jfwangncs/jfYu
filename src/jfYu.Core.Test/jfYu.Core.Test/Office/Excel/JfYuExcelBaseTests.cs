﻿using jfYu.Core.Office;
using jfYu.Core.Office.Excel;
using jfYu.Core.Office.Excel.Extensions;
using jfYu.Core.Office.Excel.Write.Interface;
using jfYu.Core.Test.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Data;

namespace jfYu.Core.Test.Office.Excel
{
    [Collection("Excel")]
    public class JfYuWordBaseTests
    {
        #region AddService

        [Fact]
        public void AddJfYuExcel_WithoutOptions_Registers()
        {
            var services = new ServiceCollection();
            services.AddJfYuExcel();

            var serviceProvider = services.BuildServiceProvider();

            var jfYuExcel = serviceProvider.GetService<IJfYuExcel>();
            var dataWrite = serviceProvider.GetService<IJfYuExcelWrite<DataTable>>();

            Assert.NotNull(jfYuExcel);
            Assert.NotNull(dataWrite);
        }

        [Fact]
        public void AddJfYuExcel_WithOptions_RegistersExcel()
        {
            var services = new ServiceCollection();
            services.AddJfYuExcel(q => { q.RowAccessSize = 22; q.SheetMaxRecord = 33; });

            var serviceProvider = services.BuildServiceProvider();

            var jfYuExcel = serviceProvider.GetService<IJfYuExcel>();
            var dataWrite = serviceProvider.GetService<IJfYuExcelWrite<DataTable>>();
            var options = serviceProvider.GetRequiredService<IOptions<JfYuExcelOption>>();
            Assert.NotNull(jfYuExcel);
            Assert.NotNull(dataWrite);
            Assert.Equal(22, options.Value.RowAccessSize);
            Assert.Equal(33, options.Value.SheetMaxRecord);
        }

        #endregion AddService

        [Fact]
        public void GetTitles_ShouldReturnCorrectTitles()
        {
            // Arrange
            var expected = new Dictionary<string, string> { { "Id", "Id" }, { "Name", "Name" }, { "Age", "Age" }, { "Address", "地址" }, { "DateTime", "DateTime" } };

            var result = JfYuExcelExtension.GetTitles<TestModel>();

            // Assert
            Assert.Equal(expected.Count, result.Count);
            foreach (var pair in expected)
            {
                Assert.Equal(pair.Value, result[pair.Key]);
            }
        }
    }
}