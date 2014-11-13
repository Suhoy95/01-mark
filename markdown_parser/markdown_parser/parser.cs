using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mono.Web;

namespace markdown_parser
{ 
    class Parser
    { 

        public string Parse(string input)
        {
            int startIndex = IndexOfNoSlashedChar(input, 0, '`');

            if (startIndex >= 0)
            {
                int endIndex = IndexOfNoSlashedChar(input, startIndex + 1, '`');
                if (endIndex >= 0)
                    return  ParseParagraph(input.Substring(0, startIndex)) +
                            "<code>" + 
                                input.Substring(startIndex + 1, endIndex-startIndex-1) + 
                            "</code>" +
                            Parse(TailString(input, endIndex + 1));
            }
            
            return ParseParagraph(input);
        }
        
        private string ParseParagraph(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            int indexNextLine = IndexOfNoSlashedChar(input, 0, '\n');

            if (indexNextLine >= 0)
            {
                int j = indexNextLine + 1;
                while (char.IsWhiteSpace(input[j])) { j++; }

                if (IsNoSlashed(input, j, '\n'))
                    return "<p>"+
                                ParseBoldTag(input.Substring(0, indexNextLine)) +
                           "</p>"+
                           ParseParagraph(TailString(input, j + 1));
            }
        
            return "<p>" + ParseBoldTag(input) + "</p>";
        }

        private string ParseBoldTag(string input)
        {
            int indexUnderline = IndexOfNoSlashedChar(input, 0, '_');
            if (indexUnderline < 0)
                return EscapeHTML(input);

            string output = WhiteSpaceOrNothing(input, indexUnderline - 1) &&
                            IsNoSlashed(input, indexUnderline + 1, '_')
                ? TryCloseStrongTag(input, indexUnderline)
                : TryCloseEmTag(input, indexUnderline);

            if (!string.IsNullOrEmpty(output))
                return output;

            return EscapeHTML(input.Substring(0, indexUnderline+1)) + ParseBoldTag(TailString(input, indexUnderline+1));
        }

        private string TryCloseEmTag(string input, int indexUnderline)
        {
            for(int closeUnderline = indexUnderline+1; 
                   (closeUnderline = IndexOfNoSlashedChar(input, closeUnderline, '_')) >= 0; closeUnderline++)
                if (input[closeUnderline - 1] != '_' && WhiteSpaceOrNothing(input, closeUnderline + 1))
                    return EscapeHTML(input.Substring(0, indexUnderline)) +
                           "<em>" +
                           ParseBoldTag(input.Substring(indexUnderline + 1, closeUnderline - indexUnderline - 1)) +
                           "</em>" +
                           ParseBoldTag(TailString(input, closeUnderline + 1));

            return "";
        }

        private string TryCloseStrongTag(string input, int startIndex)
        {
            for(int closeUnderline = startIndex+1; 
                   (closeUnderline = IndexOfNoSlashedChar(input, closeUnderline, '_')) >= 0; closeUnderline++)
                if (IsNoSlashed(input, closeUnderline + 1, '_') && WhiteSpaceOrNothing(input, closeUnderline + 2))
                    return EscapeHTML(input.Substring(0, startIndex)) +
                           "<strong>" +
                           ParseBoldTag(input.Substring(startIndex + 2, closeUnderline - startIndex - 2)) +
                           "</strong>" +
                           ParseBoldTag(TailString(input, closeUnderline + 2));

            return "";
        }

        private bool WhiteSpaceOrNothing(string input, int i)
        {
            return i < 0 || i >= input.Length || char.IsWhiteSpace(input[i]);
        }

        private string EscapeHTML(string input)
        {
            string output = string.Join("",input.Select((x,i) => { return IsNoSlashed(input, i, '\\') ? "" : x.ToString(); }).ToArray());
            return HttpUtility.HtmlEncode(output);
        }

        private int IndexOfNoSlashedChar(string input, int startIndex, char с)
        {
            //Ищет не экранированный символ, и возвращает его позицию
            for(int i = startIndex; (i = input.IndexOf(с, i)) >= 0 && i < input.Length; i++)
                if (IsNoSlashed(input, i, с))
                    return i;

            return -1;
        }

        private string TailString(string input, int i)
        {
            if (i >= input.Length)
                return "";

            return input.Substring(i);
        }

        private bool IsNoSlashed(string input, int i, char x)
        {
            if (i >= input.Length)
                return false;
            
            return i == 0 ? input[i] == x : 
                            input[i - 1] != '\\' && input[i] == x;
        }
    }
}
