using jfYu.Core.Office.Excel;
using jfYu.Core.Office.Excel.Constant;
using jfYu.Core.Office.Excel.Extensions;
using jfYu.Core.Test.Models;
using Newtonsoft.Json;
using System.Data;

namespace jfYu.Core.Test.Office.Excel
{
    [Collection("Excel")]
    public class JfYuExcelListWriterTests(IJfYuExcel jfYuExcel)
    {
        public readonly IJfYuExcel _jfYuExcel = jfYuExcel;

        [Fact]
        public void ListWriter_WithoutTitles_ReturnCorrectly()
        {
            var filePath = "ListWriterWithoutTitles.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = AllTypeTestModel.GenerateTestList();
            var writeOperation = JfYuExcelWriteOperation.None;
            var callbackCalledCount = 0;
            void callback(int count) => callbackCalledCount++;

            // Act
            _jfYuExcel.Write(source.AsQueryable(), filePath, null, writeOperation, callback);

            // Assert
            var data = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);
            Assert.NotNull(data);
            Assert.Equal(6, data.Count);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeIlist_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeIlist.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = AllTypeTestModel.GenerateTestList();
            // Act
            _jfYuExcel.Write((IList<AllTypeTestModel>)source, filePath);

            // Assert
            var data = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeList_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeList.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = AllTypeTestModel.GenerateTestList();
            // Act
            _jfYuExcel.Write(source.ToList(), filePath);

            // Assert
            var data = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeQueryable_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeQueryable.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = AllTypeTestModel.GenerateTestList();
            // Act
            _jfYuExcel.Write(source.AsQueryable(), filePath);

            // Assert
            var data = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeEnumerable_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeEnumerable.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);

            var source = AllTypeTestModel.GenerateTestList();
            // Act
            _jfYuExcel.Write(source.AsEnumerable(), filePath);

            // Assert
            var data = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithAllType_ReturnCorrectly()
        {
            var filePath = "ListWriterWithAllType.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = AllTypeTestModel.GenerateTestList();
            // Act
            _jfYuExcel.Write(source.AsQueryable(), filePath);

            // Assert
            var data = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithMultipleSheet_ReturnCorrectly()
        {
            var filePath = "ListWriterWithMultipleSheet.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new TestModelFaker().Generate(26);
            source.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            // Act
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 10);
            var workbook = _jfYuExcel.Write(source.AsQueryable(), filePath);

            // Assert
            var data = _jfYuExcel.Read<List<TestModel>>(filePath);
            Assert.NotNull(data);
            Assert.Equal(10, data.Count);

            data.AddRange(_jfYuExcel.Read<List<TestModel>>(filePath, 1, 1)!);
            Assert.NotNull(data);
            Assert.Equal(20, data.Count);

            data.AddRange(_jfYuExcel.Read<List<TestModel>>(filePath, 1, 2)!);
            Assert.NotNull(data);
            Assert.Equal(26, data.Count);

            Assert.Equal(JsonConvert.SerializeObject(source.OrderBy(q => q.Name)), JsonConvert.SerializeObject(data.OrderBy(q => q.Name)));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithUnsupportedType_ReturnCorrectly()
        {
            var filePath = "ListWriterUnsupportedType.xlsx";

            var ex = Record.Exception(() => _jfYuExcel.Write(new HashSet<int>(), filePath, JfYuExcelExtension.GetTitles<AllTypeTestModel>()));
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void ListWriter_WithUnsupportedType1_ReturnCorrectly()
        {
            var filePath = "ListWriterUnsupportedType1.xlsx";

            var ex = Record.Exception(() => _jfYuExcel.Write(new AllTypeTestModel(), filePath, JfYuExcelExtension.GetTitles<AllTypeTestModel>()));
            Assert.IsAssignableFrom<Exception>(ex);
        }

        #region Tuple

        [Fact]
        public void ListWriter_WithTypeTuple1_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple1.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var source = new Tuple<List<AllTypeTestModel>>(d1);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));

            var data1 = _jfYuExcel.Read<List<AllTypeTestModel>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data1));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTuple2_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple2.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60);
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>>(d1, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, IList<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTuple3_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple3.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>>(d1, d2, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item3));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTuple4_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple4.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>>(d1, d2, d2, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item3));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item4));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTuple5_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple5.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>>(d1, d2, d2, d2, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item3));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item4));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item5));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTuple6_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple6.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>>(d1, d2, d2, d2, d2, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item3));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item4));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item5));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item6));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTuple7_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTuple7.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>>(d1, d2, d2, d2, d2, d2, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>, List<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item3));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item4));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item5));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item6));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item7));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithTypeTupleButMoreThanMaxRecord_ThrowException()
        {
            var filePath = "ListWriterWithTypeTupleButMoreThanMaxRecord.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 10);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>>(d1, d2);

            var ex = Record.Exception(() => _jfYuExcel.Write(source, filePath, JfYuExcelExtension.GetTitles<AllTypeTestModel>()));
            Assert.IsAssignableFrom<NotSupportedException>(ex);
        }

        [Fact]
        public void ListWriter_WithTypeTupleAndCallBack_ReturnCorrectly()
        {
            var filePath = "ListWriterWithTypeTupleAndCallBack.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var callbackCalledCount = 0;
            void callback(int count) => callbackCalledCount++;

            var list = AllTypeTestModel.GenerateTestList();
            _jfYuExcel.UpdateOption(q => q.SheetMaxRecord = 100);
            var d1 = list;
            var d2 = new TestModelFaker().Generate(60).ToList();
            d2.ForEach(q => { q.DateTime = DateTime.Parse(q.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")); q.Sub = null; q.Items = []; });
            var source = new Tuple<List<AllTypeTestModel>, List<TestModel>>(d1, d2);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath, null, JfYuExcelWriteOperation.None, callback);

            // Assert
            var data = _jfYuExcel.Read<Tuple<List<AllTypeTestModel>, List<TestModel>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(d1), JsonConvert.SerializeObject(data.Item1));
            Assert.Equal(JsonConvert.SerializeObject(d2), JsonConvert.SerializeObject(data.Item2));
            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_TupleSimpleTypeString_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_TupleSimpleTypeString_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new Tuple<List<string>>(["2", "1Xa1", "3"]);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<Tuple<List<string>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_TupleSimpleTypeInt_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_TupleSimpleTypeInt_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new Tuple<List<int>>([111, 132, 342432]);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<Tuple<List<int>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_TupleSimpleTypeDecimal_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_TupleSimpleTypeDecimal_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new Tuple<List<decimal>>([111.3213M, 132.8939M, 342432.4294M]);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<Tuple<List<decimal>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_TupleSimpleTypeDateTime_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_TupleSimpleTypeDateTime_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new Tuple<List<DateTime>>([DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"))]);
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<Tuple<List<DateTime>>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_SimpleTypeString_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_SimpleTypeString_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new List<string>() { "2", "1Xa1", "3" };
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<List<string>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_SimpleTypeInt_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_SimpleTypeInt_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new List<int>() { 111, 132, 342432 };
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<List<int>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_SimpleTypeDecimal_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_SimpleTypeDecimal_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new List<decimal>() { 111.3213M, 132.8939M, 342432.4294M };
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<List<decimal>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_SimpleTypeDatetime_ReturnCorrectly()
        {
            var filePath = $"{nameof(ListWriter_SimpleTypeDatetime_ReturnCorrectly)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new List<DateTime>() { DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")) };
            // Act
            var workbook = _jfYuExcel.Write(source, filePath);
            // Assert
            var data = _jfYuExcel.Read<List<DateTime>>(filePath);

            Assert.Equal(JsonConvert.SerializeObject(source), JsonConvert.SerializeObject(data));

            File.Delete(filePath);
        }

        [Fact]
        public void ListWriter_WithErrorTitle_ThrowException()
        {
            var filePath = $"{nameof(ListWriter_WithErrorTitle_ThrowException)}.xlsx";
            // Arrange
            if (File.Exists(filePath))
                File.Delete(filePath);
            var source = new List<IJfYuExcel>() { _jfYuExcel };

            // Act
            var ex = Record.Exception(() => _jfYuExcel.Write(source, filePath, new Dictionary<string, string>() { { "x", "y" } }));
            Assert.IsAssignableFrom<InvalidOperationException>(ex);
        }

        #endregion Tuple
    }
}