using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JoyfulTools.VSExtension
{
    class MultipleBlankLinesRemover
    {
        internal static string GetReplacedText(string text)
        {
            return Regex.Replace(text, @"\n\s*\n\s*\n", "\n\n", RegexOptions.Multiline);
        }
    }
}
