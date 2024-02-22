using jfYu.Core.Office;
using jfYu.Core.Office.Word;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using System.IO;
using Xunit;


namespace xUnitTestCore
{
    public class WordTests(IJfYuWord jfYuWord)
    {
        private readonly IJfYuWord _jfYuWord = jfYuWord;

        [Fact]
        public void IOCTest()
        {
            var services = new ServiceCollection();
            services.AddJfYuWord();
            var serviceProvider = services.BuildServiceProvider();
            var _jfYuWord = serviceProvider.GetService<IJfYuWord>();
            Assert.NotNull(_jfYuWord);
        }

        [Fact]
        public void TestCreateWord()
        {
            var x = new System.Collections.Generic.Dictionary<string, object>
            {
                { "x", "测试哦" }
            };

            var doc = _jfYuWord.GenerateWord();
            var paragraph = doc.CreateParagraph();
            paragraph.Alignment = ParagraphAlignment.CENTER; //字体居中
            var run = paragraph.CreateRun();
            run.IsBold = true;
            run.SetText("${x}测试测试${y}");
            run.FontSize = 28;
            run.SetFontFamily("黑体", FontCharRange.None); //设置黑体
            paragraph.SpacingBeforeLines = 20;//上方距离
            paragraph.SpacingAfterLines = 20;//下方距离
            Directory.CreateDirectory("doctest");
            FileStream fs = new("doctest/1.docx", FileMode.Create);
            doc.Write(fs);
            fs.Close();
            x.Add("y", "xxx天秤");
            _jfYuWord.GenerateWordByTemplate("doctest/1.docx", x, "doctest/2.docx");
            Assert.True(File.Exists("doctest/2.docx"));
            var fst = File.Open("doctest/2.docx", FileMode.Open);
            Assert.True(fst.Length > 0);
            fst.Close();
            Directory.Delete("doctest", true);
        }

    }
}
