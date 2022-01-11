// MIT License
//
// Copyright(c) 2022 ICARUS Consulting GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xunit;
using Yaapii.Atoms.IO;

namespace Yaapii.Xml.Test
{
    public sealed class XMLCursorTests
    {
        [Fact]
        public void FindsDocumentNodes()
        {
            IXML doc =
                new XMLCursor(
                    new InputOf(
                        "<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>"
                    )
                );

            Assert.True(
                new Atoms.Enumerable.LengthOf(
                    doc.Nodes("//a")
                ).Value() == 2
            );
        }

        [Fact]
        public void RejectsNodesWithInvalidXPath()
        {
            try
            {
                var n = new XMLCursor("<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>").Nodes("//x/[text()")[0];
                Assert.True(false, "expected an Exception, should not get here");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "Invalid XPath expression",
                    ex.Message);
            }
        }

        [Theory]
        [InlineData("UTF-7")]
        [InlineData("UTF-8")]
        [InlineData("UTF-16")]
        [InlineData("UTF-32")]
        public void InputCtorAppliesEncoding(string name)
        {
            var encoding = Encoding.GetEncoding(name);
            var inBytes = encoding.GetBytes("<root>Can I or can't I dö prüper äncöding</root>");

            Assert.Equal(
                "Can I or can't I dö prüper äncöding",
                new XMLCursor(
                    new InputOf(inBytes),
                    encoding
                ).Values("/root/text()")[0]
            );
        }

        [Theory]
        [InlineData("UTF-7")]
        [InlineData("UTF-8")]
        [InlineData("UTF-16")]
        [InlineData("UTF-32")]
        public void StreamCtorAppliesEncoding(string name)
        {
            var encoding = Encoding.GetEncoding(name);
            var inBytes = encoding.GetBytes("<root>Can I or can't I dö prüper äncöding</root>");

            Assert.Equal(
                "Can I or can't I dö prüper äncöding",
                new XMLCursor(
                    new InputOf(inBytes).Stream(),
                    encoding
                ).Values("/root/text()")[0]
            );
        }

        [Theory]
        [InlineData("UTF-7")]
        [InlineData("UTF-8")]
        [InlineData("UTF-16")]
        [InlineData("UTF-32")]
        public void FileCtorAppliesEncoding(string name)
        {
            using (var tmp = new Yaapii.Atoms.IO.TempDirectory())
            {
                var encoding = Encoding.GetEncoding(name);
                var inBytes = encoding.GetBytes("<root>Can I or can't I dö prüper äncöding</root>");
                var path = Path.Combine(tmp.Value().FullName, "encoded.txt");

                File.WriteAllBytes(
                    path,
                    inBytes
                );

                Assert.Equal(
                    "Can I or can't I dö prüper äncöding",
                    new XMLCursor(
                        new Uri("file:///" + path),
                        encoding
                    ).Values("/root/text()")[0]
                );
            }
        }

        [Fact]
        public void RejectsValuesWithInvalidXPath()
        {
            try
            {
                var v = new XMLCursor("<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>").Values("//[x")[0];
                Assert.True(false, "expected an Exception, should not get here");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "Invalid XPath expression",
                    ex.Message);
            }
        }

        [Fact]
        public void RejectsValuesWithNodesXPath()
        {
            try
            {
                var v = new XMLCursor("<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>").Values("//a")[0];
                Assert.True(false, "expected an Exception, should not get here");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "Only text() nodes, CData sections or attributes are retrievable with xpath()",
                    ex.Message);
            }
        }

        [Fact]
        public void FindsDocumentValues()
        {
            IXML doc =
               new XMLCursor(
                   new InputOf(
                       "<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>"
                   )
               );

            Assert.True(
                doc
                .Nodes("/root/a")[0]
                .Values("x/@attr")[0] == "test"
            );
        }

        [Fact]
        public void ValueRejectsWrongSelection()
        {
            IXML doc =
               new XMLCursor(
                   new InputOf(
                       "<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>"
                   )
               );

            Assert.Throws<ArgumentException>(
                () => doc.Values("//a") //illegal because a is a TAG and not a VALUE
            );
        }

        [Fact]
        public void FindsNodesWithBuiltinNamespace()
        {
            IXML doc =
                new XMLCursor(
                    "<html xmlns='http://www.w3.org/1999/xhtml'><div>\u0443\u0440\u0430!</div></html>"
                );

            Assert.Equal(
                1,
                doc.Nodes("/xhtml:html/xhtml:div").Count
            );

            Assert.Equal(
                1,
                doc.Nodes("//xhtml:div[.='\u0443\u0440\u0430!']").Count
            );
        }

        [Fact]
        public void FindsNodesWithCustomNamespace()
        {
            IXML doc =
                new XMLCursor(
                    "<a xmlns='urn:foo'><b>\u0433!</b></a>"
                );


            doc = doc.WithNamespace("f", "urn:foo");

            Assert.Single(
                doc.Nodes("/f:a/f:b[.='\u0433!']")
            );

            Assert.Equal(
                "\u0433!",
                doc.Values("//f:b/text()")[0]
            );
        }

        [Fact]

        public void FindsDocumentNodesWithXpath()
        {
            IXML doc =
                new XMLCursor(
                    new Yaapii.Atoms.IO.InputOf(
                        "<root><a><x>1</x></a><a><x>2</x></a></root>"
                    )
                );

            Assert.Equal(
                2,
                doc.Nodes("//a").Count
            );
            Assert.Equal(
                "1",
                doc.Nodes("/root/a")[0].Values("x/text()")[0]
            );
        }

        [Fact]
        public void ConvertsItselfToXml()
        {
            IXML doc = new XMLCursor("<hello><a/></hello>");

            Assert.Single(
                new XMLCursor(doc.ToString()).Nodes("/hello/a")
            );
        }

        [Fact]
        public void RetrievesDomNode()
        {
            IXML doc = new XMLCursor("<root><simple>hello</simple></root>");

            Assert.Equal(
                "<simple>hello</simple>",
                doc.Nodes("/root/simple")[0].AsNode().ToString()
            );

            Assert.Equal<XmlNodeType>(
                XmlNodeType.Element,
                doc.Nodes("//simple")[0].AsNode().NodeType
            );
        }

        [Fact]
        public void RejectsXPathWithNoResult()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new XMLCursor("<root/>").Values("/absent-node/text()")[0]
            );

            //TODO: Check for Exception containing detailed informations. 
            //This needs method FetchedValues to be updated, see ListWrapper implementation of jcabi.
        }

        [Fact]
        public void RejectsBrokenXPathQuery()
        {
            try
            {
                var v = new XMLCursor("<root-99/>").Values("/*/hello()")[0];
                Assert.True(false, "expected an Exception, should not get here");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "Invalid XPath expression",
                    ex.Message);
            }
        }

        [Fact]
        public void PreservesProcessingInstructions()
        {
            Assert.Contains(
                "<?x test?>",
                new XMLCursor("<?xml version='1.0'?><?x test?><a/>").ToString()
            );
        }

        [Fact]
        public void PreservesDomStructureWhenXpath()
        {
            IXML doc =
                new XMLCursor(
                    "<root><item1/><item2/><item3/></root>"
                );

            IXML item = doc.Nodes("/root/item2")[0];
            Assert.Equal(
                "root",
                item.Nodes("..")[0].Values("name()")[0]);
        }

        [Fact]
        public void PrintsWithAndWithoutXmlHeader()
        {
            IXML doc = new XMLCursor("<hey/>");
            Assert.Contains(
                "<?xml",
                doc.ToString());

            Assert.StartsWith(
                "<hey",
                doc.Nodes("/*")[0].ToString());
        }

        [Fact]
        public void PerformsXpathCalculations()
        {
            IXML xml = new XMLCursor("<x><a/><a/><a/></x>");

            Assert.Single(
                xml.Values("count(//x/a)")
            );


            Assert.Equal(
                "3",
                xml.Values("count(//a)")[0]
            );
        }

        [Fact]
        public void BuildsDomNode()
        {
            IXML doc = new XMLCursor("<?xml version='1.0'?><f/>");

            Assert.True(
                doc.AsNode().NodeType == XmlNodeType.Document
            );
            Assert.True(
                doc.Nodes("/f")[0].AsNode().NodeType == XmlNodeType.Element
            );
        }

        [Fact]
        public void ComparesToAnotherDocument()
        {
            var left = new XMLCursor("<hi><dude>  </dude></hi>");
            var right = new XMLCursor("<hi><dude>  </dude></hi>");

            Assert.Equal(
                left, right
            );

            Assert.NotEqual(
                new XMLCursor("<hi><man></man></hi>"),
                new XMLCursor("<hi><man>  </man></hi>")
            );
        }

        [Fact]
        public void PreservesXmlNamespaces()
        {
            var xml = "<a xmlns='http://www.w3.org/1999/xhtml'><b/></a>";

            Assert.Equal(
                1,
                new XMLCursor(xml).Nodes("/xhtml:a/xhtml:b").Count
            );
        }

        [Fact]
        public void PreservesImmutability()
        {
            IXML xml = new XMLCursor("<r1><a/></r1>");
            XNode node = xml.Nodes("/r1/a")[0].AsNode();
            node.Remove();

            Assert.Single(
                xml.Nodes("/r1/a")
            );
        }

        [Fact]
        public void PreservesNodeWhenAccessingValue()
        {
            IXML xml = new XMLCursor("<r1><a><b>1</b></a><a><b>2</b></a></r1>");

            foreach (var node in xml.Nodes("/r1/a/b"))
            {
                //This was a usecase where the bug occured. Count was 1.
                Assert.Equal(0, node.Values("/b/text()").Count);
            }
        }

        [Fact]
        public void AppliesXpathToClonedNode()
        {
            IXML xml = new XMLCursor("<t6><z9 a='433'/></t6>");
            IXML root = xml.Nodes("/t6")[0];

            Assert.Equal(
                "433",
                root.Values("//z9/@a")[0]
            );
        }

        [Fact]
        public void ContainsOnlyQueriedNodes()
        {
            IXML xml = new XMLCursor("<t6><z9 a='433'><item>a</item><item>b</item></z9><z9 a='432'><item>c</item></z9></t6>");
            IXML root = xml.Nodes("/t6/z9[@a='433']")[0];

            Assert.Empty(root.Values("/z9[@a='432']"));

        }

        [Fact]
        public void NodesCanBeInterlaced()
        {
            IXML doc =
                new XMLCursor(
                    "<root><item1><subitem1/></item1><item2/><item3/></root>");

            Assert.Equal(
                "subitem1",
                doc.Nodes("/root/item1")[0].Nodes("./subitem1")[0].Values("name()")[0]
            );
        }
    }
}