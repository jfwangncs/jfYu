using Autofac;
using jfYu.Core.Excel;
using Microsoft.Data.Sqlite;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using Xunit;

namespace xUnitTestCore.Excel
{
    public class ExcelTest
    {
        public string name { get; set; }

        public int age { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }
    }

    public class ExcelTestNoDisplayName
    {
        public string name { get; set; }

        public int age { get; set; }

        public string Address { get; set; }
    }


    public class ExcelTestMore
    {
        public string name { get; set; }

        public int age { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }

        public int? Num { get; set; }
    }

    public class ExcelTestLess
    {
        public string name { get; set; }

        public int age { get; set; }
    }
    public class TestExcleCore
    {      

        #region 基础测试
        [Fact]
        public void TestExcle()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            Assert.NotNull(excel);
        }

        [Fact]

        public void TestCreateExcel()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var workbook = excel.CreateWorkbook();
            excel.CreateSheetAndHeader(workbook, new List<string>() { "name", "age", "Address" });
            excel.CreateSheetAndHeader(workbook, new List<string>() { "name", "age", "Address" });
            excel.Save(workbook, "exceltest/new.xlsx");
            Assert.True(File.Exists("exceltest/new.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/new.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/new.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 0);
            Assert.True(dtsourceH.Rows.Count == 1);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "age");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "Address");
            File.Delete("exceltest/new.xlsx");

        }
        #endregion

        #region 基本导出
        [Fact]
        public void TestModelExportWithoutHeader()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });
            excel.ToExcel(source, "exceltest/source.xlsx");
            Assert.True(File.Exists("exceltest/source.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/source.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/source.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 3);
            Assert.True(dtsourceH.Rows.Count == 4);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "age");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "地址");
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/source.xlsx");


        }

        [Fact]
        public void TestModelExportWithHeader()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });
            var dir = new Dictionary<string, string>
                {
                    { "age", "年龄" }
                };
            excel.ToExcel(source, "exceltest/sourceh.xlsx", dir); ;
            Assert.True(File.Exists("exceltest/sourceh.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/sourceh.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/sourceh.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 3);
            Assert.True(dtsourceH.Rows.Count == 4);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "年龄");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "地址");
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/sourceh.xlsx");

        }

        [Fact]
        public void TestQueryableExport()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });
            var dir = new Dictionary<string, string>
                {
                    { "age", "年龄" }
                };
            excel.ToExcel(source.AsQueryable(), "exceltest/queryable.xlsx", dir); ;
            Assert.True(File.Exists("exceltest/queryable.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/queryable.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/queryable.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 3);
            Assert.True(dtsourceH.Rows.Count == 4);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "年龄");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "地址");
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/queryable.xlsx");

        }

        [Fact]
        public void TestDtExportWithHeader()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
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
            var dir = new Dictionary<string, string>
                {
                    { "id", "编号" },
                    { "sex", "性别" },
                    { "age", "年龄" }
                };
            excel.ToExcel(dt, "exceltest/dth.xlsx", dir);
            Assert.True(File.Exists("exceltest/dth.xlsx"));
            var dtwoh = excel.GetDataTable("exceltest/dth.xlsx");
            var dth = excel.GetDataTable("exceltest/dth.xlsx", 0);
            Assert.True(dtwoh.Rows.Count == 5);
            Assert.True(dth.Rows.Count == 6);
            Assert.True(dth.Rows[0][0].ToString() == "编号");
            Assert.True(dth.Rows[0][1].ToString() == "name");
            Assert.True(dth.Rows[0][2].ToString() == "性别");
            Assert.True(dth.Rows[0][3].ToString() == "年龄");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Assert.True(dt.Rows[i][j].ToString() == dtwoh.Rows[i][j].ToString());
                }
            }
            File.Delete("exceltest/dth.xlsx");

        }

        [Fact]
        public void TestDtExportWithoutHeader()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
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

            excel.ToExcel(dt, "exceltest/dtwoh.xlsx");
            Assert.True(File.Exists("exceltest/dtwoh.xlsx"));
            var dtwoh = excel.GetDataTable("exceltest/dtwoh.xlsx");
            var dth = excel.GetDataTable("exceltest/dtwoh.xlsx", 0);
            Assert.True(dtwoh.Rows.Count == 5);
            Assert.True(dth.Rows.Count == 6);
            Assert.True(dth.Rows[0][0].ToString() == "id");
            Assert.True(dth.Rows[0][1].ToString() == "name");
            Assert.True(dth.Rows[0][2].ToString() == "sex");
            Assert.True(dth.Rows[0][3].ToString() == "age");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Assert.True(dt.Rows[i][j].ToString() == dtwoh.Rows[i][j].ToString());
                }
            }
            File.Delete("exceltest/dtwoh.xlsx");

        }

        [Fact]
        public void TestDbDataReaderExportWithHeader()
        {
            string datasource = "Data Source = exceltest/tmp.db";

            using var conn = new SqliteConnection(datasource);
            conn.Open();
            //创建表
            SqliteCommand cmd = new SqliteCommand();
            string sql = "CREATE TABLE test(name varchar(20),age int,Address varchar(20) )";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });

            foreach (var item in source)
            {
                //插入数据
                sql = $"INSERT INTO test VALUES('{item.name}',{item.age},'{item.Address}')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            sql = "SELECT * FROM test";
            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader();

            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var dir = new Dictionary<string, string>
                {
                    { "age", "年龄" }
                };
            excel.ToExcel(reader, "exceltest/dtreader.xlsx", dir);
            conn.Dispose();
            Assert.True(File.Exists("exceltest/dtreader.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/dtreader.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/dtreader.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 3);
            Assert.True(dtsourceH.Rows.Count == 4);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "年龄");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "Address");
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/dtreader.xlsx");
            File.Delete("exceltest/tmp.db");

        }

        [Fact]
        public void TestDbDataReaderExportWithOutHeader()
        {
            string datasource = "Data Source = exceltest/tmp1.db";
            using var conn = new SqliteConnection(datasource);
            conn.Open();
            //创建表
            SqliteCommand cmd = new SqliteCommand();
            string sql = "CREATE TABLE test(name varchar(20),age int,Address varchar(20) )";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });

            foreach (var item in source)
            {
                //插入数据
                sql = $"INSERT INTO test VALUES('{item.name}',{item.age},'{item.Address}')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            sql = "SELECT * FROM test";
            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader();

            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            excel.ToExcel(reader, "exceltest/reader.xlsx");
            conn.Dispose();
            Assert.True(File.Exists("exceltest/reader.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/reader.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/reader.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 3);
            Assert.True(dtsourceH.Rows.Count == 4);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "age");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "Address");
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/reader.xlsx");
            File.Delete("exceltest/tmp1.db");

        }

        [Fact]
        public void TestDbDataReaderTExport()
        {
            string datasource = "Data Source =:memory:";
            using var conn = new SqliteConnection(datasource);
            conn.Open();
            //创建表
            SqliteCommand cmd = new SqliteCommand();
            string sql = "CREATE TABLE test(name varchar(20),age int,Address varchar(20) )";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });

            foreach (var item in source)
            {
                //插入数据
                sql = $"INSERT INTO test VALUES('{item.name}',{item.age},'{item.Address}')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            sql = "SELECT * FROM test";
            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader();

            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            excel.ToExcel<ExcelTest>(reader, "exceltest/reader.xlsx");
            Assert.True(File.Exists("exceltest/reader.xlsx"));
            var dtsource = excel.GetDataTable("exceltest/reader.xlsx");
            var dtsourceH = excel.GetDataTable("exceltest/reader.xlsx", 0);
            Assert.True(dtsource.Rows.Count == 3);
            Assert.True(dtsourceH.Rows.Count == 4);
            Assert.True(dtsourceH.Rows[0][0].ToString() == "name");
            Assert.True(dtsourceH.Rows[0][1].ToString() == "age");
            Assert.True(dtsourceH.Rows[0][2].ToString() == "地址");
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/reader.xlsx");

        }
        #endregion

        #region 模板导出

        [Fact]

        public void TestModelExportByTemplate()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            Assert.Throws<FileNotFoundException>(() => excel.ToExcelByTemplate(null, "xxx", "exceltest/em.xlsx"));

            var workbook = excel.CreateWorkbook("exceltest/new.xlsx");

            excel.CreateSheetAndHeader(workbook, new List<string>() { "name", "age", "Address" });
            excel.CreateSheetAndHeader(workbook, new List<string>() { "name", "age", "Address" });
            excel.Save(workbook, "exceltest/new.xlsx");

            var source = new List<ExcelTest>();
            source.Add(new ExcelTest() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTest() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTest() { name = "C", age = 20, Address = "地址3" });

            var source1 = new List<ExcelTest>();
            source1.Add(new ExcelTest() { name = "A1", age = 18, Address = "地址11" });
            source1.Add(new ExcelTest() { name = "B1", age = 19, Address = "地址12" });
            source1.Add(new ExcelTest() { name = "C1", age = 20, Address = "地址13" });

            excel.ToExcelByTemplate(source, "exceltest/new.xlsx", "exceltest/tmp1.xlsx", 0);
            excel.ToExcelByTemplate(source1, "exceltest/tmp1.xlsx", "exceltest/tmp1.xlsx", 1);

            var dtsource = excel.GetDataTable("exceltest/tmp1.xlsx", 0, 0);
            var dtsourceH = excel.GetDataTable("exceltest/tmp1.xlsx", 0, 1);
            Assert.True(dtsource.Rows.Count == 4);
            Assert.True(dtsourceH.Rows.Count == 4);
            var pops = typeof(ExcelTest).GetProperties();
            for (int i = 1; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsource.Rows[i][j].ToString() == pops[j].GetValue(source[i - 1]).ToString());
                }
            }

            for (int i = 1; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(dtsourceH.Rows[i][j].ToString() == pops[j].GetValue(source1[i - 1]).ToString());
                }
            }
            File.Delete("exceltest/tmp1.xlsx");
            File.Delete("exceltest/new.xlsx");

        }

        [Fact]

        public void TestDtExportByTemplate()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            Assert.Throws<FileNotFoundException>(() => excel.ToExcelByTemplate(null, "xxx", "exceltest/em.xlsx"));

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

            var workbook = excel.CreateWorkbook();

            excel.CreateSheetAndHeader(workbook, new List<string>() { "id", "name", "sex", "age" });
            excel.CreateSheetAndHeader(workbook, new List<string>() { "id", "name", "sex", "age" });
            excel.Save(workbook, "exceltest/new.xlsx");

            excel.ToExcelByTemplate(dt, "exceltest/new.xlsx", "exceltest/tmp2.xlsx", 1);
            var dtsourceH = excel.GetDataTable("exceltest/tmp2.xlsx", 0, 1);
            Assert.True(dtsourceH.Rows.Count == 6);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Assert.True(dt.Rows[i][j].ToString() == dtsourceH.Rows[i + 1][j].ToString());
                }
            }
            File.Delete("exceltest/tmp2.xlsx");
            File.Delete("exceltest/new.xlsx");

        }
        #endregion

        #region 导入

        [Fact]
        public void TestImportFileNotFound()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            Assert.Throws<FileNotFoundException>(() => excel.GetDataTable("exceltest/FileNotFound.xlsx"));
            Assert.Throws<FileNotFoundException>(() => excel.GetList<ExcelTest>("exceltest/FileNotFound.xlsx"));
        }

        [Fact]
        public void TestImportErrorFile()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var sw = File.CreateText("exceltest/2.txt");
            sw.Close();
            Assert.Throws<Exception>(() => excel.GetDataTable("exceltest/2.txt"));
            Assert.Throws<Exception>(() => excel.GetList<ExcelTest>("exceltest/2.txt"));
            File.Delete("exceltest/2.txt");
        }

        [Fact]
        public void TestImportSteamNull()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            Assert.Throws<ArgumentNullException>(() => excel.GetDataTable((Stream)null));
            Assert.Throws<ArgumentNullException>(() => excel.GetList<ExcelTest>((Stream)null));
        }
        [Fact]
        public void TestImporErrorSteam()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var sw = File.CreateText("exceltest/1.txt").BaseStream;
            Assert.Throws<Exception>(() => excel.GetDataTable(sw));
            Assert.Throws<Exception>(() => excel.GetList<ExcelTest>(sw));
            File.Delete("exceltest/1.txt");
        }

        [Fact]
        public void TestImportDataTableFile()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
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

            excel.ToExcel(dt, "exceltest/ImportDataTableFile.xlsx");

            var dtwoh = excel.GetDataTable("exceltest/ImportDataTableFile.xlsx");
            var dth = excel.GetDataTable("exceltest/ImportDataTableFile.xlsx", 0);
            Assert.True(dtwoh.Rows.Count == 5);
            Assert.True(dth.Rows.Count == 6);
            Assert.True(dth.Rows[0][0].ToString() == "id");
            Assert.True(dth.Rows[0][1].ToString() == "name");
            Assert.True(dth.Rows[0][2].ToString() == "sex");
            Assert.True(dth.Rows[0][3].ToString() == "age");
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
        public void TestImportList()
        {

            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTestNoDisplayName>();
            source.Add(new ExcelTestNoDisplayName() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTestNoDisplayName() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTestNoDisplayName() { name = "C", age = 20, Address = "地址3" });

            excel.ToExcel(source, "exceltest/ImportList.xlsx");
            var dtsource = excel.GetList<ExcelTestNoDisplayName>("exceltest/ImportList.xlsx");

            Assert.True(dtsource.Count == 3);

            var pops = typeof(ExcelTestNoDisplayName).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(pops[j].GetValue(dtsource[i]).ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/ImportList.xlsx");

        }

        [Fact]
        //model多字段处理
        public void TestImportListMoreFiled()
        {

            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTestNoDisplayName>();
            source.Add(new ExcelTestNoDisplayName() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTestNoDisplayName() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTestNoDisplayName() { name = "C", age = 20, Address = "地址3" });

            excel.ToExcel(source, "exceltest/ImportListMoreFiled.xlsx");
            var dtsource = excel.GetList<ExcelTestMore>("exceltest/ImportListMoreFiled.xlsx");

            Assert.True(dtsource.Count == 3);


            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource[i].name == source[i].name);
                Assert.True(dtsource[i].age == source[i].age);
                Assert.True(dtsource[i].Address == source[i].Address);

                Assert.Null(dtsource[i].Phone);
                Assert.Null(dtsource[i].Num);
            }
            File.Delete("exceltest/ImportListMoreFiled.xlsx");
        }

        [Fact]
        //model少字段处理
        public void TestImportListLessFiled()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTestNoDisplayName>();
            source.Add(new ExcelTestNoDisplayName() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTestNoDisplayName() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTestNoDisplayName() { name = "C", age = 20, Address = "地址3" });

            excel.ToExcel(source, "exceltest/ImportListLessFiled.xlsx");
            var dtsource = excel.GetList<ExcelTestLess>("exceltest/ImportListLessFiled.xlsx");

            Assert.True(dtsource.Count == 3);


            for (int i = 0; i < source.Count; i++)
            {
                Assert.True(dtsource[i].name == source[i].name);
                Assert.True(dtsource[i].age == source[i].age);
            }
            File.Delete("exceltest/ImportListLessFiled.xlsx");
        }



        [Fact]
        public void TestImportDataTableStream()
        {
            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
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

            excel.ToExcel(dt, "exceltest/ImDtStream.xlsx");

            var dtwoh = excel.GetDataTable(File.Open("exceltest/ImDtStream.xlsx", FileMode.Open));
            var dth = excel.GetDataTable(File.Open("exceltest/ImDtStream.xlsx", FileMode.Open), 0);
            Assert.True(dtwoh.Rows.Count == 5);
            Assert.True(dth.Rows.Count == 6);
            Assert.True(dth.Rows[0][0].ToString() == "id");
            Assert.True(dth.Rows[0][1].ToString() == "name");
            Assert.True(dth.Rows[0][2].ToString() == "sex");
            Assert.True(dth.Rows[0][3].ToString() == "age");
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
        public void TestImportListSteam()
        {

            var cb = new ContainerBuilder();
            cb.AddJfYuExcel();
            var excel = cb.Build().Resolve<JfYuExcel>();
            var source = new List<ExcelTestNoDisplayName>();
            source.Add(new ExcelTestNoDisplayName() { name = "A", age = 18, Address = "地址1" });
            source.Add(new ExcelTestNoDisplayName() { name = "B", age = 19, Address = "地址2" });
            source.Add(new ExcelTestNoDisplayName() { name = "C", age = 20, Address = "地址3" });

            excel.ToExcel(source, "exceltest/ImListStream.xlsx");
            var dtsource = excel.GetList<ExcelTestNoDisplayName>(File.Open("exceltest/ImListStream.xlsx", FileMode.Open));

            Assert.True(dtsource.Count == 3);

            var pops = typeof(ExcelTestNoDisplayName).GetProperties();
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < pops.Length; j++)
                {
                    Assert.True(pops[j].GetValue(dtsource[i]).ToString() == pops[j].GetValue(source[i]).ToString());
                }
            }
            File.Delete("exceltest/ImListStream.xlsx");

        }


        #endregion
    }

}
