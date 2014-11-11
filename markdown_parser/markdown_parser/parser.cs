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
            string output = "";
         
            for (var i = 0; i < input.Length; i++)
            {
               string codeStr = TryGetCodeStr(input, ref i);

                if (string.IsNullOrEmpty(codeStr))
                    output += input[i];
                else
                    return GenerateParagraph(output) + codeStr + Parse(GetTailString(input, i));
            }
          
            return GenerateParagraph(output);
        }

        private string TryGetCodeStr(string input, ref int i)
        {
            if (input[i] == '`')
            {
                for(int j = i+1; j < input.Length; j++)
                    if (input[j] == '`')
                    {
                        string codeStr = input.Substring(i + 1, j - i - 1);
                        i = j+1;
                        return "<code>"+EscapeHTML(codeStr)+"</code>";
                    }
            }

            return "";
        }

        private string GenerateParagraph(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != '\n')
                {
                    output += input[i];
                }
                else
                {
                    int j = i + 1;
                    while (char.IsWhiteSpace(input[j])) { j++;}

                    if (input[j] == '\n')
                        return "<p>" + GenerateBoldTags(output) + "</p>" + GenerateParagraph(GetTailString(input, j + 1));
                }
            }

            if(!string.IsNullOrEmpty(output))
                return "<p>" + GenerateBoldTags(output) + "</p>";

            return "";
        }

        private string GenerateBoldTags(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    output += input[++i];
                    continue;
                }
                string strInBoldTag;

                strInBoldTag = tryGetStrongTag(input, ref i);
                if (!string.IsNullOrEmpty(strInBoldTag))
                    return EscapeHTML(output) + strInBoldTag + GenerateBoldTags(GetTailString(input,i));

                strInBoldTag = tryGetEmTag(input, ref i);
                if (!string.IsNullOrEmpty(strInBoldTag))
                {
                    return EscapeHTML(output) + strInBoldTag + GenerateBoldTags(GetTailString(input, i));
                }

                output += input[i];
            }

            return EscapeHTML(output);
        }

        private string GetTailString(string input, int i)
        {
            if (i >= input.Length)
                return "";

            return input.Substring(i);
        }

        private string tryGetEmTag(string input, ref int i)
        {
            if ( WhiteSpaceOrNothing(input, i-1) && input[i] == '_')
            {
                for (var j = i + 1; j < input.Length; j++)
                {
                    if (input[j - 1] != '_' && input[j] == '_' && WhiteSpaceOrNothing(input, j+1))
                    {
                        string emContent = input.Substring(i + 1, j-i-1);
                        i = j + 1;
                        return "<em>" + GenerateBoldTags(emContent)+"</em>";
                    }
                }
            }

            return "";
        }

        private string tryGetStrongTag(string input, ref int i)
        {
            if ( WhiteSpaceOrNothing(input, i-1) && input[i] == '_' && input[i + 1] == '_')
            {
                for (var j = i + 2; j < input.Length; j++)
                {
                    if (input[j - 1] == '_' && input[j] == '_' && WhiteSpaceOrNothing(input, j+1))
                    {
                        string strongContent = input.Substring(i + 2, j - i - 3);
                        i = j + 2;
                        return "<strong>"+GenerateBoldTags(strongContent) + "</strong>";
                    }
                }
            }

            return "";
        }

        private bool WhiteSpaceOrNothing(string input, int i)
        {
            if (i < 0 || i >=input.Length)
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
    }
}
