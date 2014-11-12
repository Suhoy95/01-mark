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
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\' && i+1 <input.Length)
                {
                    output += input[++i];
                    continue;
                }
                string strInBoldTag = "";

                strInBoldTag = TryGetStrongTag(input, ref i);
                if (!string.IsNullOrEmpty(strInBoldTag))
                    return EscapeHTML(output) + strInBoldTag + GenerateBoldTags(GetTailString(input,i));

                strInBoldTag = TryGetEmTag(input, ref i);
                if (!string.IsNullOrEmpty(strInBoldTag))
                {
                    return EscapeHTML(output) + strInBoldTag + GenerateBoldTags(GetTailString(input, i));
                }

                output += input[i];
            }

            return EscapeHTML(output);
        }
        
        private string TryGetEmTag(string input, ref int i)
        {
            if ( WhiteSpaceOrNothing(input, i-1) && NoSlashedChar(input, i, '_'))
            {
                for (var j = i + 1; j < input.Length; j++)
                {
                    if (input[j - 1] != '_' && NoSlashedChar(input, j, '_') && WhiteSpaceOrNothing(input, j+1))
                    {
                        string emContent = input.Substring(i + 1, j-i-1);
                        i = j + 1;
                        return "<em>" + GenerateBoldTags(emContent)+"</em>";
                    }
                }
            }

            return "";
        }

        private string TryGetStrongTag(string input, ref int i)
        {
            if ( WhiteSpaceOrNothing(input, i-1) && NoSlashedChar(input, i, '_') && NoSlashedChar(input, i + 1, '_'))
            {
                for (var j = i + 2; j < input.Length; j++)
                {
                    if (NoSlashedChar(input, j - 1, '_') && NoSlashedChar(input, j, '_') && WhiteSpaceOrNothing(input, j+1))
                    {
                        string strongContent = input.Substring(i + 2, j - i - 3);
                        i = j + 1;
                        return "<strong>"+GenerateBoldTags(strongContent) + "</strong>";
                    }
                }
            }

            return "";
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
