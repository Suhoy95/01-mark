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
        public static void return_simple_input_string_in_paragraph()
        {
            Assert.AreEqual("<p>Hello</p>", a.Parse("Hello"));
        }

        [Test]
        public static void replace_underline_tag()
        {
            Assert.AreEqual("<p><em>Hello</em></p>", a.Parse("_Hello_"));
        }

        [Test]
        public static void noreplace_one_underline()
        {
            Assert.AreEqual("<p>_Hello</p>", a.Parse("_Hello"));
        }

        [Test]
        public static void replace_twice_underline()
        {   
            Assert.AreEqual("<p><strong>Hello</strong></p>", a.Parse("__Hello__"));
        }

        [Test]
        public static void replace_twice_in_ones_underline()
        {
            Assert.AreEqual("<p><em> <strong>Hello</strong> </em></p>", a.Parse("_ __Hello__ _"));
        }
        
        [Test]
        public static void replace_ones_in_twice_underline()
        {
            Assert.AreEqual("<p><strong> <em>Hello</em> </strong></p>", a.Parse("__ _Hello_ __"));
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
            Assert.AreEqual("<p>Подчерки_внутри_текста__и__цифр_12_3</p>", a.Parse("Подчерки_внутри_текста__и__цифр_12_3"));
        }

        [Test]
        public static void ignore_backslash_character()
        {
            Assert.AreEqual("<p>_Hello_</p>", a.Parse("\\_Hello\\_"));
        }
        //new test
        [Test]
        public static void ignore_single_underline_and_escaped_underline()
        {
            Assert.AreEqual("<p>_Hello_</p>", a.Parse("_Hello\\_"));
        }

        [Test]
        public static void ignore_double_underline_and_escaped_double_underline()
        {
            Assert.AreEqual("<p>__Hello__</p>", a.Parse("__Hello\\__"));
        }

        [Test]
        public static void replace_double_underline_in_nesting()
        {
            Assert.AreEqual("<p><strong> _Hello</strong> _</p>", a.Parse("__ _Hello__ _"));
        }

        [Test]
        public static void replace_single_underline_in_nesting()
        {
            Assert.AreEqual("<p><em> __Hello</em> __</p>", a.Parse("_ __Hello_ __"));
        }

        [Test]
        public static void replace_code_in_nesting()
        {
            Assert.AreEqual("<p>_ </p><code>Hello_ </code>", a.Parse("_ `Hello_ `"));
        }

        [Test]
        public static void escape_html()
        {
            Assert.AreEqual("<p>&lt;em&gt;Hello&lt;/em&gt;</p>", a.Parse("<em>Hello</em>"));
        }
        [Test]
        public static void ignore_backslash_backticks()
        {
            Assert.AreEqual("<p>`Hello`</p>", a.Parse("`Hello\\`"));
        }

        [Test]
        public static void slashed_slash()
        {
            Assert.AreEqual("<p>\\Hello</p>", a.Parse("\\\\Hello"));
        }

    }
}
