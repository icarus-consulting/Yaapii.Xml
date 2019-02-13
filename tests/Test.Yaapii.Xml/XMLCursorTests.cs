﻿using System;
using Xunit;
using Yaapii.Atoms.Enumerable;
using System.Xml;
using System.Xml.Linq;

namespace Yaapii.Xml.Test
{
    public sealed class XMLCursorTests
    {
        [Fact]
        public void FindsDocumentNodes()
        {
            IXML doc =
                new XMLCursor(
                    new Atoms.IO.InputOf(
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
                   new Atoms.IO.InputOf(
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
                   new Atoms.IO.InputOf(
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

            Assert.True(
                new LengthOf(
                    doc.Nodes("/xhtml:html/xhtml:div")
                ).Value() == 1
            );

            Assert.True(
                new LengthOf(
                    doc.Nodes("//xhtml:div[.='\u0443\u0440\u0430!']")
                ).Value() == 1);
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

            Assert.True(
                new LengthOf(
                    doc.Nodes("//a")
                ).Value() == 2);

            Assert.True(
                doc
                .Nodes("/root/a")[0]
                .Values("x/text()")[0] == "1");
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

            Assert.Equal<XMLCursor>(
                left, right
            );

            Assert.NotEqual<XMLCursor>(
                new XMLCursor("<hi><man></man></hi>"),
                new XMLCursor("<hi><man>  </man></hi>")
            );
        }

        [Fact]
        public void PreservesXmlNamespaces()
        {
            String xml = "<a xmlns='http://www.w3.org/1999/xhtml'><b/></a>";
            Assert.True(
                new LengthOf(
                    new XMLCursor(xml).Nodes("/xhtml:a/xhtml:b")
                ).Value() == 1);
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

            foreach(var node in xml.Nodes("/r1/a/b"))
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