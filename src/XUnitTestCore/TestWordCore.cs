using jfYu.Core.Word;
using System.IO;
using Xunit;
using Autofac;
using NPOI.XWPF.UserModel;

namespace xUnitTestCore.Word
{
    public class TestWordCore
    {

        [Fact]
        public void CreateWord()
        {
            var ContainerBuilder = new ContainerBuilder();
            ContainerBuilder.AddJfYuWord();
            var c = ContainerBuilder.Build();
            var ms = c.Resolve<jfYuWord>();
            var x = new System.Collections.Generic.Dictionary<string, object>
            {
                { "x", "²âÊÔÅ¶" }
            };

            var doc = ms.GenerateWord();
            var paragraph = doc.CreateParagraph();
            paragraph.Alignment = ParagraphAlignment.CENTER; //×ÖÌå¾ÓÖÐ
            var run = paragraph.CreateRun();
            run.IsBold = true;
            run.SetText("${x}²âÊÔ²âÊÔ${y}");
            run.FontSize = 28;
            run.SetFontFamily("ºÚÌå", FontCharRange.None); //ÉèÖÃºÚÌå
            paragraph.SpacingBeforeLines = 20;//ÉÏ·½¾àÀë
            paragraph.SpacingAfterLines = 20;//ÏÂ·½¾àÀë
            Directory.CreateDirectory("doctest");
            FileStream fs = new FileStream("doctest/1.docx", FileMode.Create);
            doc.Write(fs);            
            fs.Close();
            x.Add("y", "xxxÌì³Ó");
            ms.GenerateWordByTemplate("doctest/1.docx", x, "doctest/2.docx");
            Assert.True(File.Exists("doctest/2.docx"));
            var fst = File.Open("doctest/2.docx", FileMode.Open);
            Assert.True(fst.Length > 0);
            fst.Close();
            Directory.Delete("doctest",true);
        }

    }
}
