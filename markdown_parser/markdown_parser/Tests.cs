using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace markdown_parser
{
    [TestFixture]
    class Parser_should
    {
        [Test]
        public static void return_simple_input_string()
        {
            Parser a = new Parser();
            Assert.AreEqual("Hello", a.toParse("Hello"));
        }

        [Test]
        public static void replace_underline_tag()
        {
            Parser a = new Parser();
            Assert.AreEqual("<em>Hello</em>", a.toParse("_Hello_"));
        }

        [Test]
        public static void noreplace_one_underline()
        {
            Parser a = new Parser();
            Assert.AreEqual("_Hello", a.toParse("_Hello"));
        }
    }
}
