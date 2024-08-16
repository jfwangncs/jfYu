using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using Xunit;
using jfYu.Core.Office;
using jfYu.Core.Office.Excel;
using Microsoft.Data.Sqlite;

namespace xUnitTestCore
{
    #region Model
    public class ExcelTest
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        [DisplayName("地址")]
        public string? Address { get; set; }
    }

    public class ExcelTestNoDisplayName
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public string? Address { get; set; }
    }


    public class ExcelTestMore
    {
        public string? Name { get; set; }

        public int Age { get; set; }
        public string? Address { get; set; }

        public string? Phone { get; set; }

        public int? Num { get; set; }
    }

    public class ExcelTestLess
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }
    #endregion

    public class ExcelTests(IJfYuExcel jfYuExcel)
    {
        private readonly IJfYuExcel _jfYuExcel = jfYuExcel;

        #region 基础测试
        [Fact]
        public void IOCTest()
        {
            var services = new ServiceCollection();
            services.AddJfYuExcel();
            var serviceProvider = services.BuildServiceProvider();
            var _jfYuExcel = serviceProvider.GetService<IJfYuExcel>();
            Assert.NotNull(_jfYuExcel);
        }

        [Fact]

        public void CreateTest()
        {
            var workbook = _jfYuExcel.CreateWorkbook();
            workbook.CreateSheetWithTitles<ExcelTestNoDisplayName>();
            workbook.CreateSheetWithTitles<ExcelTest>();
            workbook.Save("exceltest/new.xlsx");
            Assert.True(File.Exists("exceltest/new.xlsx"));
            var dtsource = _jfYuExcel.GetDataTable("exceltest/new.xlsx");
            var dtsourceH = _jfYuExcel.GetDataTable("exceltest/new.xlsx", 0);
            Assert.Equal(0, dtsource.Rows.Count);
            Assert.Equal(1, dtsourceH.Rows.Count);
            Assert.Equal("Name", dtsourceH.Rows[0][0].ToString());
            Assert.Equal("Age", dtsourceH.Rows[0][1].ToString());
            Assert.Equal("Address", dtsourceH.Rows[0][2].ToString());

            dtsourceH = _jfYuExcel.GetDataTable("exceltest/new.xlsx", 0, 1);
            Assert.Equal("Name", dtsourceH.Rows[0][0].ToString());
            Assert.Equal("Age", dtsourceH.Rows[0][1].ToString());
            Assert.Equal("地址", dtsourceH.Rows[0][2].ToString());
            File.Delete("exceltest/new.xlsx");

        }
        #endregion

        #region 基本导出

        [Fact]
        public void CSVExportTest()
        {
            var source = new List<ExcelTest>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToCSV(source, "csv.csv");
            Assert.True(File.Exists("csv.csv"));
            File.Delete("csv.csv");
        }
        [Fact]
        public void ModelExportTest()
        {

            var source = new List<ExcelTest>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToExcel(source, "exceltest/source.xlsx");
            Assert.True(File.Exists("exceltest/source.xlsx"));
            var dir = new Dictionary<string, string>
                {
                    { "Age", "年龄" }
                };
            _jfYuExcel.ToExcel(source, "exceltest/source1.xlsx", dir);
            Assert.True(File.Exists("exceltest/source1.xlsx"));

            var dtsource = _jfYuExcel.GetDataTable("exceltest/source.xlsx");
            var dtsourceH = _jfYuExcel.GetDataTable("exceltest/source.xlsx", 0);

            Assert.Equal(3, dtsource.Rows.Count);
            Assert.Equal(4, dtsourceH.Rows.Count);
            Assert.Equal("Name", dtsourceH.Rows[0][0].ToString());
            Assert.Equal("Age", dtsourceH.Rows[0][1].ToString());

            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i])?.ToString());
                }
            }

            var dtsource1 = _jfYuExcel.GetDataTable("exceltest/source1.xlsx");
            var dtsource1H = _jfYuExcel.GetDataTable("exceltest/source1.xlsx", 0);

            Assert.Equal(3, dtsource1.Rows.Count);
            Assert.Equal(4, dtsource1H.Rows.Count);
            Assert.Equal("年龄", dtsource1H.Rows[0][0].ToString());

            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource1.Rows[i][0].ToString() == pops.FirstOrDefault(q => q.Name == "Age")?.GetValue(source[i])?.ToString());
            }
            File.Delete("exceltest/source.xlsx");
            File.Delete("exceltest/source1.xlsx");

        }

        [Fact]
        public void QueryableExportTest()
        {
            var source = new List<ExcelTest>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToExcel(source.AsQueryable(), "exceltest/queryable.xlsx");
            Assert.True(File.Exists("exceltest/queryable.xlsx"));

            var dir = new Dictionary<string, string>
                {
                    { "Age", "年龄" }
                };
            _jfYuExcel.ToExcel(source, "exceltest/queryable1.xlsx", dir);
            Assert.True(File.Exists("exceltest/queryable1.xlsx"));


            var dtsource = _jfYuExcel.GetDataTable("exceltest/queryable.xlsx");
            var dtsourceH = _jfYuExcel.GetDataTable("exceltest/queryable.xlsx", 0);
            Assert.Equal(3, dtsource.Rows.Count);
            Assert.Equal(4, dtsourceH.Rows.Count);
            Assert.Equal("Name", dtsourceH.Rows[0][0].ToString());
            Assert.Equal("Age", dtsourceH.Rows[0][1].ToString());
            Assert.Equal("地址", dtsourceH.Rows[0][2].ToString());
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i])?.ToString());
                }
            }


            var dtsource1 = _jfYuExcel.GetDataTable("exceltest/queryable1.xlsx");
            var dtsource1H = _jfYuExcel.GetDataTable("exceltest/queryable1.xlsx", 0);
            Assert.Equal(3, dtsource1.Rows.Count);
            Assert.Equal(4, dtsource1H.Rows.Count);
            Assert.Equal("年龄", dtsource1H.Rows[0][0].ToString());
            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource1.Rows[i][0].ToString() == pops.FirstOrDefault(q => q.Name == "Age")?.GetValue(source[i])?.ToString());
            }

            File.Delete("exceltest/queryable.xlsx");
            File.Delete("exceltest/queryable1.xlsx");

        }

        [Fact]
        public void DtExportTest()
        {
            var dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("sex");
            dt.Columns.Add("age");
            dt.Rows.Add("1", "王", "男", "12");
            dt.Rows.Add("2", "wang", "男", "1200000");
            dt.Rows.Add("3", "大的洼地", "女", "12");
            dt.Rows.Add("4", "dwadwadwad", "1", "12");
            dt.Rows.Add("5", "13213213", "0", "1465452");

            _jfYuExcel.ToExcel(dt, "exceltest/dth.xlsx");
            Assert.True(File.Exists("exceltest/dth.xlsx"));

            var dir = new Dictionary<string, string>
                {
                    { "id", "编号" },
                    { "sex", "性别" },
                    { "age", "年龄" }
                };
            _jfYuExcel.ToExcel(dt, "exceltest/dth1.xlsx", dir);
            Assert.True(File.Exists("exceltest/dth1.xlsx"));

            var dtwoh = _jfYuExcel.GetDataTable("exceltest/dth.xlsx");
            var dth = _jfYuExcel.GetDataTable("exceltest/dth.xlsx", 0);
            Assert.Equal(5, dtwoh.Rows.Count);
            Assert.Equal(6, dth.Rows.Count);
            Assert.Equal("id", dth.Rows[0][0].ToString());
            Assert.Equal("name", dth.Rows[0][1].ToString());
            Assert.Equal("sex", dth.Rows[0][2].ToString());
            Assert.Equal("age", dth.Rows[0][3].ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Assert.True(dt.Rows[i][j].ToString() == dtwoh.Rows[i][j].ToString());
                }
            }

            var dtwoh1 = _jfYuExcel.GetDataTable("exceltest/dth1.xlsx");
            var dth1 = _jfYuExcel.GetDataTable("exceltest/dth1.xlsx", 0);
            Assert.Equal(5, dtwoh1.Rows.Count);
            Assert.Equal(6, dth1.Rows.Count);
            Assert.Equal("编号", dth1.Rows[0][0].ToString());
            Assert.Equal("性别", dth1.Rows[0][1].ToString());
            Assert.Equal("年龄", dth1.Rows[0][2].ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dth1.Columns.Count; j++)
                {
                    var j1 = j;
                    if (j == 1)
                        j1 = 2;
                    if (j == 2)
                        j1 = 3;
                    Assert.True(dt.Rows[i][j1].ToString() == dtwoh1.Rows[i][j].ToString());
                }
            }

            File.Delete("exceltest/dth.xlsx");
            File.Delete("exceltest/dth1.xlsx");
        }


        [Fact]
        public void DbDataReaderExportTest()
        {      
            if (File.Exists("exceltest/tmp.db"))
                File.Delete("exceltest/tmp.db");
            string datasource = "Data Source = exceltest/tmp.db";
            using var conn = new SqliteConnection(datasource);
            conn.Open();
            //创建表
            SqliteCommand cmd = new();
            string sql = "CREATE TABLE test(name varchar(20),age int,Address varchar(20) )";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            var source = new List<ExcelTest>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            foreach (var item in source)
            {
                //插入数据
                sql = $"INSERT INTO test VALUES('{item.Name}',{item.Age},'{item.Address}')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            sql = "SELECT * FROM test";
            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader();

            _jfYuExcel.ToExcel(reader, "exceltest/dtreader.xlsx", ExcelExtension.GetTitles<ExcelTest>());
            conn.Dispose();
            SqliteConnection.ClearAllPools();
            Assert.True(File.Exists("exceltest/dtreader.xlsx"));

            conn.Open();
            SqliteCommand cmd1 = new()
            {
                CommandText = sql,
                Connection = conn
            };
            var reader1 = cmd1.ExecuteReader();
            var dir = new Dictionary<string, string>
                {
                    { "Age", "年龄" }
                };
            _jfYuExcel.ToExcel(reader1, "exceltest/dtreader1.xlsx", dir);
            Assert.True(File.Exists("exceltest/dtreader1.xlsx"));
            conn.Dispose();
            SqliteConnection.ClearAllPools();

            var dtsource = _jfYuExcel.GetDataTable("exceltest/dtreader.xlsx");
            var dtsourceH = _jfYuExcel.GetDataTable("exceltest/dtreader.xlsx", 0);
            Assert.Equal(3, dtsource.Rows.Count);
            Assert.Equal(4, dtsourceH.Rows.Count);
            Assert.Equal("Name", dtsourceH.Rows[0][0].ToString());
            Assert.Equal("Age", dtsourceH.Rows[0][1].ToString());
            Assert.Equal("地址", dtsourceH.Rows[0][2].ToString());
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i])?.ToString());
                }
            }


            var dtsource1 = _jfYuExcel.GetDataTable("exceltest/dtreader1.xlsx");
            var dtsource1H = _jfYuExcel.GetDataTable("exceltest/dtreader1.xlsx", 0);
            Assert.Equal(3, dtsource1.Rows.Count);
            Assert.Equal(4, dtsource1H.Rows.Count);
            Assert.Equal("年龄", dtsource1H.Rows[0][0].ToString());
            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource1.Rows[i][0].ToString() == pops.FirstOrDefault(q => q.Name == "Age")?.GetValue(source[i])?.ToString());
            }
            File.Delete("exceltest/dtreader.xlsx");
            File.Delete("exceltest/dtreader1.xlsx");
            File.Delete("exceltest/tmp.db");

        }
        #endregion


        #region 导入

        [Fact]
        public void ImportFileNotFoundTest()
        {

            Assert.Throws<FileNotFoundException>(() => _jfYuExcel.GetDataTable("exceltest/FileNotFound.xlsx"));
            Assert.Throws<FileNotFoundException>(() => _jfYuExcel.GetList<ExcelTest>("exceltest/FileNotFound.xlsx"));
        }

        [Fact]
        public void ImportErrorFileTest()
        {
            var sw = File.CreateText("exceltest/2.txt");
            sw.Close();
            Assert.Throws<Exception>(() => _jfYuExcel.GetDataTable("exceltest/2.txt"));
            Assert.Throws<Exception>(() => _jfYuExcel.GetList<ExcelTest>("exceltest/2.txt"));
            File.Delete("exceltest/2.txt");
        }

        [Theory]
        [InlineData(null)]

        public void ImportSteamNullTest(Stream steam)
        {
            Assert.Throws<ArgumentNullException>(() => _jfYuExcel.GetDataTable(steam));
            Assert.Throws<ArgumentNullException>(() => _jfYuExcel.GetList<ExcelTest>(steam));
        }
        [Fact]
        public void ImporErrorSteamTest()
        {
            var sw = File.CreateText("exceltest/1.txt").BaseStream;
            Assert.Throws<Exception>(() => _jfYuExcel.GetDataTable(sw));
            Assert.Throws<Exception>(() => _jfYuExcel.GetList<ExcelTest>(sw));
            File.Delete("exceltest/1.txt");
        }

        [Fact]
        public void ImportDataTableFileTest()
        {
            var dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("sex");
            dt.Columns.Add("age");
            dt.Rows.Add("1", "王", "男", "12");
            dt.Rows.Add("2", "wang", "男", "1200000");
            dt.Rows.Add("3", "大的洼地", "女", "12");
            dt.Rows.Add("4", "dwadwadwad", "1", "12");
            dt.Rows.Add("5", "13213213", "0", "1465452");

            _jfYuExcel.ToExcel(dt, "exceltest/ImportDataTableFile.xlsx");

            var dtwoh = _jfYuExcel.GetDataTable("exceltest/ImportDataTableFile.xlsx");
            var dth = _jfYuExcel.GetDataTable("exceltest/ImportDataTableFile.xlsx", 0);
            Assert.Equal(5, dtwoh.Rows.Count);
            Assert.Equal(6, dth.Rows.Count);
            Assert.Equal("id", dth.Rows[0][0].ToString());
            Assert.Equal("name", dth.Rows[0][1].ToString());
            Assert.Equal("sex", dth.Rows[0][2].ToString());
            Assert.Equal("age", dth.Rows[0][3].ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Assert.True(dt.Rows[i][j].ToString() == dtwoh.Rows[i][j].ToString());
                }
            }
            File.Delete("exceltest/ImportDataTableFile.xlsx");

        }

        [Fact]
        //model刚好对应
        public void ImportListTest()
        {
            var source = new List<ExcelTestNoDisplayName>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToExcel(source, "exceltest/ImportList.xlsx");
            var dtsource = _jfYuExcel.GetList<ExcelTestNoDisplayName>("exceltest/ImportList.xlsx");

            Assert.Equal(3, dtsource?.Count);

            var pops = typeof(ExcelTestNoDisplayName).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(pops[j].GetValue(dtsource?[i])?.ToString() == pops[j].GetValue(source[i])?.ToString());
                }
            }
            File.Delete("exceltest/ImportList.xlsx");

        }

        [Fact]
        //model多字段处理
        public void ImportListMoreFiledTest()
        {
            var source = new List<ExcelTestNoDisplayName>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToExcel(source, "exceltest/ImportListMoreFiled.xlsx");
            var dtsource = _jfYuExcel.GetList<ExcelTestMore>("exceltest/ImportListMoreFiled.xlsx");

            Assert.Equal(3, dtsource?.Count);


            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource?[i].Name == source[i].Name);
                Assert.True(dtsource?[i].Age == source[i].Age);
                Assert.True(dtsource?[i].Address == source[i].Address);

                Assert.Null(dtsource?[i].Phone);
                Assert.Null(dtsource?[i].Num);
            }

            File.Delete("exceltest/ImportListMoreFiled.xlsx");
        }

        [Fact]
        //model少字段处理
        public void ImportListLessFiledTest()
        {

            var source = new List<ExcelTestNoDisplayName>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToExcel(source, "exceltest/ImportListLessFiled.xlsx");
            var dtsource = _jfYuExcel.GetList<ExcelTestLess>("exceltest/ImportListLessFiled.xlsx");

            Assert.Equal(3, dtsource?.Count);


            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource?[i].Name == source[i].Name);
                Assert.True(dtsource?[i].Age == source[i].Age);
            }
            File.Delete("exceltest/ImportListLessFiled.xlsx");
        }



        [Fact]
        public void ImportDataTableStreamTest()
        {
            var dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("sex");
            dt.Columns.Add("age");
            dt.Rows.Add("1", "王", "男", "12");
            dt.Rows.Add("2", "wang", "男", "1200000");
            dt.Rows.Add("3", "大的洼地", "女", "12");
            dt.Rows.Add("4", "dwadwadwad", "1", "12");
            dt.Rows.Add("5", "13213213", "0", "1465452");

            _jfYuExcel.ToExcel(dt, "exceltest/ImDtStream.xlsx");

            var dtwoh = _jfYuExcel.GetDataTable(File.Open("exceltest/ImDtStream.xlsx", FileMode.Open));
            var dth = _jfYuExcel.GetDataTable(File.Open("exceltest/ImDtStream.xlsx", FileMode.Open), 0);
            Assert.Equal(5, dtwoh.Rows.Count);
            Assert.Equal(6, dth.Rows.Count);
            Assert.Equal("id", dth.Rows[0][0].ToString());
            Assert.Equal("name", dth.Rows[0][1].ToString());
            Assert.Equal("sex", dth.Rows[0][2].ToString());
            Assert.Equal("age", dth.Rows[0][3].ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Assert.True(dt.Rows[i][j].ToString() == dtwoh.Rows[i][j].ToString());
                }
            }
            File.Delete("exceltest/ImDtStream.xlsx");

        }

        [Fact]
        //model刚好对应
        public void ImportListSteamTest()
        {

            var source = new List<ExcelTestNoDisplayName>
            {
                new() { Name = "A", Age = 18, Address = "地址1" },
                new() { Name = "B", Age = 19, Address = "地址2" },
                new() { Name = "C", Age = 20, Address = "地址3" }
            };

            _jfYuExcel.ToExcel(source, "exceltest/ImListStream.xlsx");
            var dtsource = _jfYuExcel.GetList<ExcelTestNoDisplayName>(File.Open("exceltest/ImListStream.xlsx", FileMode.Open));

            Assert.Equal(3, dtsource?.Count);

            var pops = typeof(ExcelTestNoDisplayName).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(pops[j].GetValue(dtsource?[i])?.ToString() == pops[j].GetValue(source[i])?.ToString());
                }
            }
            File.Delete("exceltest/ImListStream.xlsx");
        }

        #endregion
    }

}
