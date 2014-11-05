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
            Assert.AreEqual("Hello", a.toParse("Hello"));
        }

        [Test]
        public static void replace_underline_tag()
        {
            Assert.AreEqual("<em>Hello</em>", a.toParse("_Hello_"));
        }

        [Test]
        public static void noreplace_one_underline()
        {
            Assert.AreEqual("_Hello", a.toParse("_Hello"));
        }

        [Test]
        public static void replace_twice_underline()
        {   
            Assert.AreEqual("<strong>Hello</strong>", a.toParse("__Hello__"));
        }

        [Test]
        public static void replace_twice_in_ones_underline()
        {
            Assert.AreEqual("<em> <strong>Hello</strong> </em>", a.toParse("_ __Hello__ _"));
        }
        
        [Test]
        public static void replace_ones_in_twice_underline()
        {
            Assert.AreEqual("<strong> <em>Hello</em> </strong>", a.toParse("__ _Hello_ __"));
        }

        [Test]
        public static void replace_backticks()
        {
            Assert.AreEqual("<code>Hello</code>", a.toParse("`Hello`"));
        }

        [Test]
        public static void ignore_underline_in_backticks()
        {
            Assert.AreEqual("<code> _Hello_ </code>", a.toParse("` _Hello_ `"));
        }

        [Test]
        public static void ignore_underline_in_words()
        {
            Assert.AreEqual("Подчерки_внутри_текста__и__цифр_12_3", a.toParse("Подчерки_внутри_текста__и__цифр_12_3"));
        }

        [Test]
        public static void ignore_backslash_character()
        {
            Assert.AreEqual("_Hello_", a.toParse("\\_Hello\\_"));
        }

    }
}
