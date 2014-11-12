using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace markdown_parser
{ 
    class Parser
    { 

        public string Parse(string input)
        {

            int indexBacktick = IndexOfNoSlashedChar(input, 0, '`');

            if (indexBacktick >= 0)
            {
                int closeBacktick = IndexOfNoSlashedChar(input, indexBacktick + 1, '`');
                if (closeBacktick >= 0)
                    return GenerateParagraph(input.Substring(0, indexBacktick)) +
                           "<code>" + input.Substring(indexBacktick + 1, closeBacktick-indexBacktick-1) + "</code>" +
                           Parse(GetTailString(input, closeBacktick + 1));
            }
            
          
            return GenerateParagraph(input);
        }
        
        private string GenerateParagraph(string input)
        {
            int indexNextLine = IndexOfNoSlashedChar(input, 0, '\n');

            if (indexNextLine >= 0)
            {
                int j = indexNextLine + 1;
                while (char.IsWhiteSpace(input[j])) { j++; }

                if (NoSlashedChar(input, j, '\n'))
                    return GenerateParagraph(input.Substring(0, indexNextLine)) + 
                           GenerateParagraph(GetTailString(input, j + 1));
            }

            if (string.IsNullOrEmpty(input))
                return "";

            return "<p>" + GenerateBoldTags(input) + "</p>";
        }

        private string GenerateBoldTags(string input)
        {
            int indexUnderline = IndexOfNoSlashedChar(input, 0, '_');
            if (indexUnderline >= 0 && WhiteSpaceOrNothing(input, indexUnderline-1))
            {
                if (NoSlashedChar(input, indexUnderline + 1, '_'))
                {
                    int closeUnderline = IndexOfNoSlashedChar(input, indexUnderline + 1, '_');
                    while (closeUnderline >= 0)
                    {
                        if (NoSlashedChar(input, closeUnderline + 1, '_') &&
                            WhiteSpaceOrNothing(input, closeUnderline + 2))
                            return EscapeHTML(input.Substring(0, indexUnderline))+
                                "<strong>"+GenerateBoldTags(input.Substring(indexUnderline+2, closeUnderline-indexUnderline-2))+"</strong>"+
                                GenerateBoldTags(GetTailString(input, closeUnderline+2));
                        closeUnderline = IndexOfNoSlashedChar(input, closeUnderline + 1, '_');
                    }

                }
                else
                {
                    int closeUnderline = IndexOfNoSlashedChar(input, indexUnderline + 1, '_');
                    while (closeUnderline >= 0)
                    {
                        if (input[closeUnderline-1] != '_' && WhiteSpaceOrNothing(input, closeUnderline + 1))
                            return EscapeHTML(input.Substring(0, indexUnderline)) +
                                "<em>" + GenerateBoldTags(input.Substring(indexUnderline + 1, closeUnderline - indexUnderline - 1)) + "</em>" +
                                GenerateBoldTags(GetTailString(input, closeUnderline + 1));
                        closeUnderline = IndexOfNoSlashedChar(input, closeUnderline + 1, '_');
                    }
                }
            }

            return EscapeHTML(input);
        }

        private bool WhiteSpaceOrNothing(string input, int i)
        {
            if (i < 0 || i >= input.Length)
                return true;

            return char.IsWhiteSpace(input[i]);
        }

        private string EscapeHTML(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case'<':
                        output += "&lt;";
                        break;
                    case '>':
                        output += "&gt;";
                        break;
                    case '\\':
                        break;
                    default:
                        output += input[i];
                        break;
                }
            }
            return output;
        }

        private int IndexOfNoSlashedChar(string input, int startIndex, char x)
        {

            if (startIndex >= input.Length || startIndex < 0)
                return -1;
            int index = input.IndexOf(x, startIndex);
            while (index >= 0)
            {
                if (NoSlashedChar(input, index, x))
                    return index;
                index = input.IndexOf(x, index + 1);
            }

            return -1;
        }

        private string GetTailString(string input, int i)
        {
            if (i >= input.Length)
                return "";

            return input.Substring(i);
        }

        private bool NoSlashedChar(string input, int i, char x)
        {
            if (i == 0)
                return input[i] == x;
            if (i >= input.Length)
                return false;

            return input[i - 1] != '\\' && input[i] == x;
        }
    }
}
