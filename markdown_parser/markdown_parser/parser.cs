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
         
            input = ' ' + input + ' ';            
            for (var i = 1; i < input.Length-1; i++)
            {
                if (input[i] == '\\')
                {
                    output += input[++i];
                    continue;
                }

                string replacing = "";
                replacing += tryReplaceTwiceUnderline(input, ref i);
                replacing += tryReplaceOnesUnderline(input, i);
                replacing += tryReplaceBackticks(input, ref i);
                replacing += tryEscapeHTML(input, i);
                
                
                if (string.IsNullOrEmpty(replacing))
                    output += input[i];
                else
                    output += replacing;
            }
          
            return output;
        }

        private string tryEscapeHTML(string input, int i)
        {
            if (input[i] == '<')
                return "&lt;";
            if (input[i] == '>')
                return "&gt;";

            return "";
        }

        private string tryReplaceBackticks(string input, ref int i)
        {
            if (input[i] == '`')
            {
                string code = "<code>";
                for (var j = i + 1; j < input.Length - 1; j++)
                {
                    if (input[j] == '`')
                    {
                        i = j;
                        return code + "</code>";
                    }

                    code += input[j];
                }
            }
            return "";
        }

        private string tryReplaceOnesUnderline(string input, int i)
        {
            if (char.IsWhiteSpace(input[i-1]) && input[i] == '_')
                for(var j = i+1; j < input.Length-1; j ++)
                    if(tryReplaceOnesUnderline(input, j) == "</em>")
                        return "<em>";

            if(input[i-1] != '_' && input[i] =='_' &&
                char.IsWhiteSpace(input[i+1]))
                return "</em>";
            
            return "";
        }

        private string tryReplaceTwiceUnderline(string input, ref int i)
        {
            if (char.IsWhiteSpace(input[i - 1]) && input[i] == '_' && input[i+1] == '_' )
                for (var j = i + 1; j < input.Length - 1; j++)
                    if (tryReplaceTwiceUnderline(input,ref j) == "</strong>")
                    {
                        i++;
                        return "<strong>";
                    }

            if (input[i] == '_' && input[i+1] == '_' &&
                char.IsWhiteSpace(input[i + 2]))
            {
                i++;
                return "</strong>";
            }
                
            return "";
        }
    }
}
