using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.WordProcessing;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace jfYu.Core.Office.Word
{
    public class JfYuWord : IJfYuWord
    {
        /// <summary>
        /// Generate Word By Templat
        /// </summary>
        /// <param name="TemplatePath">file path</param>
        /// <param name="bookmarks">marks</param>
        /// <param name="filename">save file path</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void GenerateWordByTemplate(string TemplatePath, Dictionary<string, object> bookmarks, string filename)
        {
            if (!File.Exists(TemplatePath))
                throw new FileNotFoundException("can't find template file.");

            using FileStream stream = File.OpenRead(TemplatePath);
            using XWPFDocument doc = new(stream);
            foreach (var para in doc.Paragraphs)
            {
                ReplaceKey(para, bookmarks);
            }
            var tables = doc.Tables;
            foreach (var table in tables)
            {
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.GetTableCells())
                    {
                        foreach (var para in cell.Paragraphs)
                        {
                            ReplaceKey(para, bookmarks);
                        }
                    }
                }
            }
            using FileStream fs = new(filename, FileMode.Create);
            doc.Write(fs);
            fs.Close();
        }

        /// <summary>
        /// Generate Word doc
        /// </summary>
        /// <returns></returns>
        public XWPFDocument GenerateWord()
        {
            var doc = new XWPFDocument();
            doc.Document.body.sectPr = new CT_SectPr();
            CT_SectPr m_SectPr = doc.Document.body.sectPr;
            m_SectPr.pgSz.h = (ulong)16838;
            m_SectPr.pgSz.w = (ulong)11906;
            m_SectPr.pgMar.left = (ulong)800;
            m_SectPr.pgMar.right = (ulong)800;
            m_SectPr.pgMar.top = (ulong)850;
            m_SectPr.pgMar.bottom = (ulong)850;
            return doc;
        }

        /// <summary>
        /// Replace Key
        /// </summary>
        /// <param name="para">doc</param>
        /// <param name="model">model</param>
        private static void ReplaceKey(XWPFParagraph para, Dictionary<string, object> model)
        {
            string text = para.ParagraphText;
            var runs = para.Runs;
            int length = runs.Count;
            string styleid = para.Style;
            text = string.Join("", runs.Select(x => x.Text));
            foreach (var p in model)
            {
                if (text.Contains("${" + p.Key + "}"))
                {
                    if (p.Value.GetType().Name.Equals("String"))
                        text = text.Replace("${" + p.Key + "}", p.Value.ToString());
                    else
                    {
                        text = text.Replace("${" + p.Key + "}", "");
                        var gr = para.CreateRun();
                        FileStream fs = (FileStream)p.Value;
                        var picID = para.Document.AddPictureData(fs, (int)PictureType.JPEG);
                        CreatePicture(para, picID, 150, 200);
                    }
                }
            }
            for (int j = (length - 1); j >= 0; j--)
            {
                para.RemoveRun(j);
            }
            //直接调用XWPFRun的setText()方法设置文本时，在底层会重新创建一个XWPFRun，把文本附加在当前文本后面，
            //所以我们不能直接设值，需要先删除当前run,然后再自己手动插入一个新的run。
            para.InsertNewRun(0).SetText(text, 0);

        }

        /// <summary>
        /// pic
        /// </summary>
        /// <param name="para">doc</param>
        /// <param name="id">pic id</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        private static void CreatePicture(XWPFParagraph para, string id, int width, int height)
        {
            int EMU = 9525;
            width *= EMU;
            height *= EMU;

            string picXml = ""
                    + "      <pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">"
                    + "         <pic:nvPicPr>" + "            <pic:cNvPr id=\""
                    + "0"
                    + "\" name=\"Generated\"/>"
                    + "            <pic:cNvPicPr/>"
                    + "         </pic:nvPicPr>"
                    + "         <pic:blipFill>"
                    + "            <a:blip r:embed=\""
                    + id
                    + "\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"/>"
                    + "            <a:stretch>"
                    + "               <a:fillRect/>"
                    + "            </a:stretch>"
                    + "         </pic:blipFill>"
                    + "         <pic:spPr>"
                    + "            <a:xfrm>"
                    + "               <a:off x=\"0\" y=\"0\"/>"
                    + "               <a:ext cx=\""
                    + width
                    + "\" cy=\""
                    + height
                    + "\"/>"
                    + "            </a:xfrm>"
                    + "            <a:prstGeom prst=\"rect\">"
                    + "               <a:avLst/>"
                    + "            </a:prstGeom>"
                    + "         </pic:spPr>"
                    + "      </pic:pic>";
            var run = para.CreateRun();
            CT_Inline inline = run.GetCTR().AddNewDrawing().AddNewInline();

            inline.graphic = new CT_GraphicalObject
            {
                graphicData = new CT_GraphicalObjectData()
            };
            inline.graphic.graphicData.uri = "http://schemas.openxmlformats.org/drawingml/2006/picture";

            try
            {
                inline.graphic.graphicData.AddPicElement(picXml);

            }
            catch (XmlException)
            {

            }

            NPOI.OpenXmlFormats.Dml.WordProcessing.CT_PositiveSize2D extent = inline.AddNewExtent();
            extent.cx = width;
            extent.cy = height;

            NPOI.OpenXmlFormats.Dml.WordProcessing.CT_NonVisualDrawingProps docPr = inline.AddNewDocPr();
            docPr.id = 1;
            docPr.name = "Image" + id;
        }
    }
}
