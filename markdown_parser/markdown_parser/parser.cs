using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace markdown_parser
{ 
    class Parser
    {
        private enum tag{notag, em, p, strong, code};

        private List< Tuple<tag, int> > tags;

        public string Parse(string input)
        {
            tags = new List<Tuple<tag, int>>();
            List<string> output = new List<string>();
         
            input = ' ' + input + ' ';
            string text ="";
            for (var i = 1; i < input.Length-1; i++)
            {
                if (input[i] == '\\')
                {
                    text += parseChar(input[++i]);
                    continue;
                }

                tag nowTag = isTag(input, ref i);

                if (nowTag == tag.notag)
                    text += parseChar(input[i]);
                else
                {
                    output.Add(text);
                    text = "";
                    if (tags.Any(x => x.Item1 == nowTag))
                    {
                        var j = tags.Count - 1;
                        while(tags[j].Item1 != nowTag)
                        {
                            output[tags[j].Item2] += getMarkdownChar(tags[j].Item1);
                            tags.RemoveAt(j);
                            j--;
                        }
                        output[tags[j].Item2] += getOpenTag(nowTag);
                        output[output.Count - 1] += getCloseTag(nowTag);
                        tags.RemoveAt(j);
                    }
                    else
                    {
                        tags.Add(new Tuple<tag, int>(nowTag, output.Count() - 1));
                    }
                }
            }

            if (!string.IsNullOrEmpty(text))
                output.Add(text);
            for (var j = 0; j < tags.Count(); j++)
                output[tags[j].Item2] += getMarkdownChar(tags[j].Item1);

            string ans = "";
            for (var i = 0; i < output.Count(); i++)
                ans += output[i];
            return ans;
        }

        private string parseChar(char p)
        {
            if (p == '<')
                return "&lt;";
            if (p == '>')
                return "&gt;";

            return p.ToString();
        }

        private tag isTag(string input, ref int i)
        {
            if (input[i] == '`')
                return tag.code;

            if (char.IsWhiteSpace(input[i - 1]) && input[i] == '_' && input[i+1] == '_' ||
                input[i] == '_' && input[i+1] == '_' && char.IsWhiteSpace(input[i + 2]))
            {
                i++;
                return tag.strong;
            }

            if (char.IsWhiteSpace(input[i-1]) && input[i] == '_' ||
                input[i-1] != '_' && input[i] =='_' && char.IsWhiteSpace(input[i+1]))
                return tag.em;

            if (input[i] == '\n')
            {
                var j = i+1;
                while (char.IsWhiteSpace(input[j++])) { }

                if(input[j] == '\n')
                    return tag.p;
            }

            return tag.notag;
        }

        private string getOpenTag(tag a)
        {
            switch (a)
            {
                case tag.code:
                    return "<code>";
                case tag.em:
                    return "<em>";
                case tag.strong:
                    return "<strong>";
                case tag.p:
                    return "<p>";
            }
            return "";
        }
        
        private string getCloseTag(tag a)
        {
            switch (a)
            {
                case tag.code:
                    return "</code>";
                case tag.em:
                    return "</em>";
                case tag.strong:
                    return "</strong>";
                case tag.p:
                    return "</p>";
            }
            return "";
        }

        private string getMarkdownChar(tag a)
        {
            switch (a)
            {
                case tag.code:
                    return "`";
                case tag.em:
                    return "_";
                case tag.strong:
                    return "__";
            }
            return "";
        }
    }
}
