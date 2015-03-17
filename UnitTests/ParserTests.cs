﻿using System;
using CSSParser;
using CSSParser.ContentProcessors.StringProcessors;
using Xunit;

namespace UnitTests
{
    public class LessParserTests
    {
        [Fact]
        public void NullString()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Parser.ParseLESS((string)null);
            });
        }

        [Fact]
        public void BlankContent()
        {
            Assert.Equal(
                new CategorisedCharacterString[0],
                Parser.ParseLESS(""),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void EmptyBodyTagOnSingleLine()
        {
            var content = "body { }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("body", 0),
                CSS.Whitespace(" ", 4),
                CSS.OpenBrace(5),
                CSS.Whitespace(" ", 6),
                CSS.CloseBrace(7)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void TerminatingLineReturnIsPartOfSingleLineComment()
        {
            var content = "// Comment\nbody { }";
            var expected = new CategorisedCharacterString[]
			{
                CSS.Comment("// Comment\n", 0),
                CSS.SelectorOrStyleProperty("body", 11),
                CSS.Whitespace(" ", 15),
                CSS.OpenBrace(16),
                CSS.Whitespace(" ", 17),
                CSS.CloseBrace(18)
			};
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void NonContainedLineReturnIsNotPartOfMultiLineComment()
        {
            var content = "/* Comment */\nbody { }";
            var expected = new CategorisedCharacterString[]
			{
                CSS.Comment("/* Comment */", 0),
                CSS.Whitespace("\n", 13),
                CSS.SelectorOrStyleProperty("body", 14),
                CSS.Whitespace(" ", 18),
                CSS.OpenBrace(19),
                CSS.Whitespace(" ", 20),
                CSS.CloseBrace(21)
			};
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void PseudoClassesShouldNotBeIdentifiedAsPropertyValues()
        {
            var content = "a:hover { color: blue; }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("a:hover", 0),
                CSS.Whitespace(" ", 7),
                CSS.OpenBrace(8),
                CSS.Whitespace(" ", 9),
                CSS.SelectorOrStyleProperty("color", 10),
                CSS.StylePropertyColon(15),
                CSS.Whitespace(" ", 16),
                CSS.Value("blue", 17),
                CSS.SemiColon(21),
                CSS.Whitespace(" ", 22),
                CSS.CloseBrace(23)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void AttributeSelectorsShouldNotBeIdentifiedAsPropertyValues()
        {
            var content = "a[href] { }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("a[href]", 0),
                CSS.Whitespace(" ", 7),
                CSS.OpenBrace(8),
                CSS.Whitespace(" ", 9),
                CSS.CloseBrace(10)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void AttributeSelectorsWithQuotedContentShouldNotBeIdentifiedAsPropertyValues()
        {
            var content = "input[type=\"text\"] { color: blue; }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("input[type=\"text\"]", 0),
                CSS.Whitespace(" ", 18),
                CSS.OpenBrace(19),
                CSS.Whitespace(" ", 20),
                CSS.SelectorOrStyleProperty("color", 21),
                CSS.StylePropertyColon(26),
                CSS.Whitespace(" ", 27),
                CSS.Value("blue", 28),
                CSS.SemiColon(32),
                CSS.Whitespace(" ", 33),
                CSS.CloseBrace(34)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void LESSMixinArgumentDefaultsShouldNotBeIdentifiedAsPropertyValues()
        {
            var content = ".RoundedCorners (@radius: 4px) { border-radius: @radius; }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty(".RoundedCorners", 0),
                CSS.Whitespace(" ", 15),
                CSS.SelectorOrStyleProperty("(@radius: 4px)", 16),
                CSS.Whitespace(" ", 30),
                CSS.OpenBrace(31),
                CSS.Whitespace(" ", 32),
                CSS.SelectorOrStyleProperty("border-radius", 33),
                CSS.StylePropertyColon(46),
                CSS.Whitespace(" ", 47),
                CSS.Value("@radius", 48),
                CSS.SemiColon(55),
                CSS.Whitespace(" ", 56),
                CSS.CloseBrace(57)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void PseudoClassesShouldNotBeIdentifiedAsPropertyValuesWhenMinified()
        {
            var content = "a:hover{color:blue}";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("a:hover", 0),
                CSS.OpenBrace(7),
                CSS.SelectorOrStyleProperty("color", 8),
                CSS.StylePropertyColon(13),
                CSS.Value("blue", 14),
                CSS.CloseBrace(18)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void PseudoClassesShouldNotBeIdentifiedAsPropertyValuesWhenWhitespaceIsPresentAroundTheColon()
        {
            var content = "a : hover{}";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("a", 0),
                CSS.Whitespace(" ", 1),
                CSS.SelectorOrStyleProperty(":", 2),
                CSS.Whitespace(" ", 3),
                CSS.SelectorOrStyleProperty("hover", 4),
                CSS.OpenBrace(9),
                CSS.CloseBrace(10)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void EndOfQuotedStylePropertyMayNotBeEndOfEntryStyleProperty()
        {
            var content = "body { font-family: \"Segoe UI\", Verdana; }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("body", 0),
                CSS.Whitespace(" ", 4),
                CSS.OpenBrace(5),
                CSS.Whitespace(" ", 6),
                CSS.SelectorOrStyleProperty("font-family", 7),
                CSS.StylePropertyColon(18),
                CSS.Whitespace(" ", 19),
                CSS.Value("\"Segoe UI\",", 20),
                CSS.Whitespace(" ", 31),
                CSS.Value("Verdana", 32),
                CSS.SemiColon(39),
                CSS.Whitespace(" ", 40),
                CSS.CloseBrace(41)
            };

            Assert.Equal(
                expected,

                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

        [Fact]
        public void MediaQueryCriteriaShouldBeIdentifiedAsSelectorContent()
        {
            var content = "@media screen and (min-width: 600px) { body { background: white url(\"awesomecats.png\") no-repeat; } }";
            var expected = new CategorisedCharacterString[]
            {
                CSS.SelectorOrStyleProperty("@media", 0),
                CSS.Whitespace(" ", 6),
                CSS.SelectorOrStyleProperty("screen", 7),
                CSS.Whitespace(" ", 13),
                CSS.SelectorOrStyleProperty("and", 14),
                CSS.Whitespace(" ", 17),
                CSS.SelectorOrStyleProperty("(min-width:", 18),
                CSS.Whitespace(" ", 29),
                CSS.SelectorOrStyleProperty("600px)", 30),
                CSS.Whitespace(" ", 36),
                CSS.OpenBrace(37),
                CSS.Whitespace(" ", 38),
                CSS.SelectorOrStyleProperty("body", 39),
                CSS.Whitespace(" ", 43),
                CSS.OpenBrace(44),
                CSS.Whitespace(" ", 45),
                CSS.SelectorOrStyleProperty("background", 46),
                CSS.StylePropertyColon(56),
                CSS.Whitespace(" ", 57),
                CSS.Value("white", 58),
                CSS.Whitespace(" ", 63),
                CSS.Value("url(\"awesomecats.png\")", 64),
                CSS.Whitespace(" ", 86),
                CSS.Value("no-repeat", 87),
                CSS.SemiColon(96),
                CSS.Whitespace(" ", 97),
                CSS.CloseBrace(98),
                CSS.Whitespace(" ", 99),
                CSS.CloseBrace(100)
            };
            Assert.Equal(
                expected,
                Parser.ParseLESS(content),
                new CategorisedCharacterStringComparer()
            );
        }

    }
}
