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
        static Parser a = new Parser();
        [Test]
        public static void return_simple_input_string()
        {
            Assert.AreEqual("Hello", a.Parse("Hello"));
        }

        [Test]
        public static void replace_underline_tag()
        {
            Assert.AreEqual("<em>Hello</em>", a.Parse("_Hello_"));
        }

        [Test]
        public static void noreplace_one_underline()
        {
            Assert.AreEqual("_Hello", a.Parse("_Hello"));
        }

        [Test]
        public static void replace_twice_underline()
        {   
            Assert.AreEqual("<strong>Hello</strong>", a.Parse("__Hello__"));
        }

        [Test]
        public static void replace_twice_in_ones_underline()
        {
            Assert.AreEqual("<em> <strong>Hello</strong> </em>", a.Parse("_ __Hello__ _"));
        }
        
        [Test]
        public static void replace_ones_in_twice_underline()
        {
            Assert.AreEqual("<strong> <em>Hello</em> </strong>", a.Parse("__ _Hello_ __"));
        }

        [Test]
        public static void replace_backticks()
        {
            Assert.AreEqual("<code>Hello</code>", a.Parse("`Hello`"));
        }

        [Test]
        public static void ignore_underline_in_backticks()
        {
            Assert.AreEqual("<code> _Hello_ </code>", a.Parse("` _Hello_ `"));
        }

        [Test]
        public static void ignore_underline_in_words()
        {
            Assert.AreEqual("Подчерки_внутри_текста__и__цифр_12_3", a.Parse("Подчерки_внутри_текста__и__цифр_12_3"));
        }

        [Test]
        public static void ignore_backslash_character()
        {
            Assert.AreEqual("_Hello_", a.Parse("\\_Hello\\_"));
        }
        //new test
        [Test]
        public static void ignore_single_underline_and_escaped_underline()
        {
            var actual = a.Parse("_Hello\\_");
            Assert.AreEqual("_Hello_", actual);
        }

        [Test]
        public static void ignore_double_underline_and_escaped_double_underline()
        {
            var actual = a.Parse("__Hello\\__");
            Assert.AreEqual("__Hello__", actual);
        }

        [Test]
        public static void replace_double_underline_in_nesting()
        {
            Assert.AreEqual("<strong> _Hello</strong> _", a.Parse("__ _Hello__ _"));
        }

        [Test]
        public static void replace_single_underline_in_nesting()
        {
            Assert.AreEqual("<em> __Hello</em> __", a.Parse("_ __Hello_ __"));
        }

        [Test]
        public static void replace_code_in_nesting()
        {
            Assert.AreEqual("<em> `Hello</em> `", a.Parse("_ `Hello_ `"));
        }

        [Test]
        public static void escape_html()
        {
            Assert.AreEqual("&lt;em&gt;Hello&lt;/em&gt;", a.Parse("<em>Hello</em>"));
        }

        [Test]
        public static void replace_paragraph()
        {
            Assert.AreEqual("<p>fisrt paragraph</p><p>second paragraph</p>", a.Parse("\n\nfisrt paragraph\n\nsecond paragraph"));
        }
    }
}
