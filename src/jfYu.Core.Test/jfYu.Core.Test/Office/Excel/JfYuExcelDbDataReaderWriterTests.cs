using jfYu.Core.Office.Excel;
using jfYu.Core.Office.Excel.Constant;
using jfYu.Core.Office.Excel.Extensions;
using jfYu.Core.Test.MemoryDB;
using jfYu.Core.Test.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Data;

namespace jfYu.Core.Test.Office.Excel
{
    [Collection("Excel")]
    public class JfYuExcelDbDataReaderWriterTests
    {
        public IJfYuExcel _jfYuExcel;
        public TestDbContext _db;

        public JfYuExcelDbDataReaderWriterTests(IJfYuExcel jfYuExcel, TestDbContext db)
        {
            _jfYuExcel = jfYuExcel;
            _db = db;
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();
        }

        [Fact]
        public void DbDataReaderWriter_NullSource_ThrowException()
        {
            var filePath = "DbDataReaderWriter_NullSource_ThrowException.xlsx";

            IDataReader? dataReader = null;
            var ex = Record.Exception(() => _jfYuExcel.Write(dataReader, filePath, JfYuExcelExtension.GetTitles<AllTypeTestModel>()));
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void DbDataReaderWriter_NullTitles_ThrowException()
        {
            var filePath = "DbDataReaderWriter_NullTitles_ThrowException.xlsx";
            using var command = _db.Database.GetDbConnection().CreateCommand();
            _db.Database.OpenConnection();
            command.CommandText = "SELECT * FROM sqlite_master";
            using var reader = command.ExecuteReader();
            var ex = Record.Exception(() => _jfYuExcel.Write(reader, filePath));
            Assert.IsAssignableFrom<NullReferenceException>(ex);
        }

        [Fact]
        public void DbDataReaderWriter_EmptyTitles_ThrowException()
        {
            var filePath = "DbDataReaderWriter_EmptyTitles_ThrowException.xlsx";
            using var command = _db.Database.GetDbConnection().CreateCommand();
            _db.Database.OpenConnection();
            command.CommandText = "SELECT * FROM sqlite_master";
            using var reader = command.ExecuteReader();
            var ex = Record.Exception(() => _jfYuExcel.Write(reader, filePath, []));
            Assert.IsAssignableFrom<NullReferenceException>(ex);
        }


        [Fact]
        public void DbDataReaderWriter_ClosedSource_ThrowException()
        {
            var filePath = "DbDataReaderWriter_ClosedSource_ThrowException.xlsx";
            using var command = _db.Database.GetDbConnection().CreateCommand();
            _db.Database.OpenConnection();
            command.CommandText = "SELECT * FROM sqlite_master";

            var reader = command.ExecuteReader();
            _db.Database.CloseConnection();
            var ex = Record.Exception(() => _jfYuExcel.Write(reader, filePath, JfYuExcelExtension.GetTitles<AllTypeTestModel>()));
            Assert.IsAssignableFrom<Exception>(ex);
        }


        [Fact]
        public void DbDataReaderWriter_ReturnCorrectly()
        {
            var filePath = "DbDataReaderWriter_ReturnCorrectly.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var callbackCalledCount = 0;
            void callback(int count) => callbackCalledCount++;

            _db.TestModels.RemoveRange(_db.TestModels);
            var source = new TestModelFaker().Generate(166);
            source.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            _db.AddRange(source);
            _db.SaveChanges();
            // Act
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 1000);
            using var command = _db.Database.GetDbConnection().CreateCommand();

            _db.Database.OpenConnection();
            command.CommandText = "SELECT * FROM TestModels";
            using var reader = command.ExecuteReader();
            var workbook = _jfYuExcel.Write(reader, filePath, JfYuExcelExtension.GetTitles<TestModel>(), JfYuExcelWriteOperation.None, callback);

            // Assert
            var data = _jfYuExcel.Read<List<TestModel>>(filePath);
            Assert.Equal(JsonConvert.SerializeObject(_db.TestModels.ToList()), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }

        [Fact]
        public void DbDataReaderWriter_WithMultipleSheet_ReturnCorrectly()
        {
            var filePath = "DbDataReaderWriter_WithMultipleSheet_ReturnCorrectly.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new TestModelFaker().Generate(18);
            _db.TestModels.RemoveRange(_db.TestModels);
            source.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            _db.AddRange(source);
            _db.SaveChanges();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 10);
            // Act 
            using var command = _db.Database.GetDbConnection().CreateCommand();

            _db.Database.OpenConnection();
            command.CommandText = "SELECT * FROM TestModels";
            using var reader = command.ExecuteReader();
            var workbook = _jfYuExcel.Write(reader, filePath, JfYuExcelExtension.GetTitles<TestModel>());

            // Assert
            var data = _jfYuExcel.Read<List<TestModel>>(filePath);
            data!.AddRange(_jfYuExcel.Read<List<TestModel>>(filePath, 1, 1)!);
            Assert.Equal(JsonConvert.SerializeObject(_db.TestModels.ToList()), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }
    }
}