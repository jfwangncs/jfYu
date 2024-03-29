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
                { "x", "����Ŷ" }
            };

            var doc = ms.GenerateWord();
            var paragraph = doc.CreateParagraph();
            paragraph.Alignment = ParagraphAlignment.CENTER; //�������
            var run = paragraph.CreateRun();
            run.IsBold = true;
            run.SetText("${x}���Բ���${y}");
            run.FontSize = 28;
            run.SetFontFamily("����", FontCharRange.None); //���ú���
            paragraph.SpacingBeforeLines = 20;//�Ϸ�����
            paragraph.SpacingAfterLines = 20;//�·�����
            Directory.CreateDirectory("doctest");
            FileStream fs = new FileStream("doctest/1.docx", FileMode.Create);
            doc.Write(fs);            
            fs.Close();
            x.Add("y", "xxx���");
            ms.GenerateWordByTemplate("doctest/1.docx", x, "doctest/2.docx");
            Assert.True(File.Exists("doctest/2.docx"));
            var fst = File.Open("doctest/2.docx", FileMode.Open);
            Assert.True(fst.Length > 0);
            fst.Close();
            Directory.Delete("doctest",true);
        }

    }
}
