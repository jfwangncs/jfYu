using NPOI.XWPF.UserModel;
using System.Collections.Generic;

namespace jfYu.Core.Office.Word
{
    public interface IJfYuWord
    {
        /// <summary>
        /// Generate Word doc
        /// </summary>
        /// <returns></returns>
        XWPFDocument GenerateWord();

        /// <summary>
        /// Generate Word By Templat
        /// </summary>
        /// <param name="TemplatePath">file path</param>
        /// <param name="bookmarks">marks</param>
        /// <param name="filename">save file path</param>
        /// <exception cref="FileNotFoundException"></exception>
        void GenerateWordByTemplate(string TemplatePath, Dictionary<string, object> bookmarks, string filename);
    }
}