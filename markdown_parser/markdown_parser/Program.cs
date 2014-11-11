using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdown_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser a = new Parser();
            a.Parse("_Hello_");
        }
    }
}
