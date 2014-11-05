using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace markdown_parser
{ 
    class Parser
    { 

        public string toParse(string input)
        {
            input = ' ' + input + ' ';
            string output = "";
            string replacing = "";
            for (var i = 1; i < input.Length-1; i++)
            {
                replacing = "";
                replacing += tryReplaceTwiceUnderline(input, i);
                replacing += tryReplaceOnesUnderline(input, i);
                replacing += tryReplaceBackticks(input, i);
                if (string.IsNullOrEmpty(replacing))
                    output += input[i];
                else
                    output += replacing;
            }
          
            return output;
        }

        private string tryReplaceBackticks(string input, int i)
        {
            if (input[i] != '`')
                return "";
            return "<code>";
        }

        private string tryReplaceOnesUnderline(string input, int i)
        {
            if (string.IsNullOrWhiteSpace(input[i-1].ToString()) && input[i] == '_')
                for(var j = i+1; j < input.Length-1; j ++)
                    if(tryReplaceOnesUnderline(input, j) == "</em>")
                        return "<em>";

            if(input[i-1] != '_' && input[i] =='_' &&
                String.IsNullOrWhiteSpace(input[i+1].ToString()))
                return "</em>";
            
            return "";
        }

        private string tryReplaceTwiceUnderline(string input, int i)
        {
            return "";
        }
    }
}
