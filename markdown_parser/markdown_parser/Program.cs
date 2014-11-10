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
            string test = "\n             \n";
            if (test[0] == '\n')
            {
                var j = 1;
                while (test[j++] == ' ') { }

                if (test[j-1] == '\n')
                    Console.WriteLine("yes");
            }
            Console.Write("no");
        }
    }
}
