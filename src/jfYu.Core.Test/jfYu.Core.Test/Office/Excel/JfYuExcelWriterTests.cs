using jfYu.Core.Office.Excel;
using jfYu.Core.Office.Excel.Constant;
using jfYu.Core.Office.Excel.Extensions;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System.Data;
using System.Text;

namespace jfYu.Core.Test.Office.Excel
{
    [Collection("Excel")]
    public class JfYuExcelWriterTests(IJfYuExcel jfYuExcel)
    {
        private readonly IJfYuExcel _jfYuExcel = jfYuExcel;

        [Theory]
        [InlineData(JfYuExcelVersion.Xlsx)]
        [InlineData(JfYuExcelVersion.Xls)]
        public void CreateExcel_ReturnIWorkBook(JfYuExcelVersion version)
        {
            var wb = _jfYuExcel.CreateExcel(version);
            Assert.NotNull(wb);
        }

        [Fact]
        public void CreateExcel_UnSupportVersion_ThrowException()
        {
            var ex = Record.Exception(() => _jfYuExcel.CreateExcel(JfYuExcelVersion.Csv));
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("!@#$%^&*()_+")]
        public void ExcelWriter_PathIsNullOrEmpty_ThrowException(string path)
        {
            var dt = new DataTable();
            var ex = Record.Exception(() => _jfYuExcel.Write(dt, path));
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void ExcelWriter_NoImplementedProvider_ThrowException()
        {
            var data = new Dictionary<string, string>();
            var ex = Record.Exception(() => _jfYuExcel.Write(data, "1.xlsx"));
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void AddTitle_AddsHeadersToFirstRow()
        {
            // Arrange
            var titles = new Dictionary<string, string> { { "Column1", "Header 1" }, { "Column2", "Header 2" }, { "Column3", "abcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcdeabcde" }, { "Column4", "abcdeabcdeabcdeabcdeabcde" } };

            // Act
            var workbook = _jfYuExcel.CreateExcel();
            var sheet = workbook.CreateSheet();
            sheet.AddTitle(titles);

            // Assert
            IRow headerRow = sheet.GetRow(0);
            Assert.NotNull(headerRow);
            int i = 0;
            foreach (var title in titles)
            {
                ICell cell = headerRow.GetCell(i++);
                Assert.NotNull(cell);
                Assert.Equal(title.Value, cell.StringCellValue);

                ICellStyle style = cell.CellStyle;
                Assert.Equal(HorizontalAlignment.Center, style.Alignment);
                IFont font = style.GetFont(workbook);
                Assert.Equal(10, font.FontHeightInPoints);
                Assert.True(font.IsBold);
            }
            // Verify column widths
            i = 0;
            foreach (var title in titles)
            {
                int colLength = Encoding.UTF8.GetBytes(title.Value).Length;
                int expectedWidth = colLength > 100 ? 100 * 256 : colLength < 20 ? 10 * 256 : colLength * 256;
                Assert.Equal(expectedWidth, sheet.GetColumnWidth(i++));
            }

            workbook.Close();
        }
    }
}