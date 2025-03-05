using jfYu.Core.Office.Word.Constant;
using System.Collections.Generic;

namespace jfYu.Core.Office.Word
{
    public interface IJfYuWord
    {
        /// <summary>
        /// Generates a Word document from a template, replacing with provided values.
        /// </summary>
        /// <param name="templatePath">Path to the template file.</param>
        /// <param name="outputPath">Output path.</param>
        /// <param name="replacements">list oftheir replacement values.</param>
        void GenerateWordByTemplate(string templatePath, string outputFilePath, List<JfYuWordReplacement> replacements);
    }
}