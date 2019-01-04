using System;
using Xunit;
using Yaapii.Atoms.Enumerable;
using System.Xml;
using System.Xml.Linq;

namespace Yaapii.Xml.Test
{
    public sealed class XMLQueryTests
    {
        [Fact]
        public void FindsDocumentNodes()
        {
            IXML doc =
                new XMLQuery(
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
                var n = new XMLQuery("<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>").Nodes("//x/[text()")[0];
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
                var v = new XMLQuery("<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>").Values("//[x")[0];
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
                var v = new XMLQuery("<root><a><x attr='test'>1</x></a><a><x>2</x></a></root>").Values("//a")[0];
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
               new XMLQuery(
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
               new XMLQuery(
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
                new XMLQuery(
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
                new XMLQuery(
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
                new XMLQuery(
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
            IXML doc = new XMLQuery("<hello><a/></hello>");

            Assert.Single(
                new XMLQuery(doc.ToString()).Nodes("/hello/a")
            );
        }

        [Fact]
        public void RetrievesDomNode()
        {
            IXML doc = new XMLQuery("<root><simple>hello</simple></root>");

            Assert.Equal(
                "<simple>hello</simple>",
                doc.Nodes("/root/simple")[0].Node().ToString()
            );

            Assert.Equal<XmlNodeType>(
                XmlNodeType.Element,
                doc.Nodes("//simple")[0].Node().NodeType
            );
        }

        [Fact]
        public void RejectsXPathWithNoResult()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new XMLQuery("<root/>").Values("/absent-node/text()")[0]
            );

            //TODO: Check for Exception containing detailed informations. 
            //This needs method FetchedValues to be updated, see ListWrapper implementation of jcabi.
        }

        [Fact]
        public void RejectsBrokenXPathQuery()
        {
            try
            {
                var v = new XMLQuery("<root-99/>").Values("/*/hello()")[0];
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
                new XMLQuery("<?xml version='1.0'?><?x test?><a/>").ToString()
            );
        }

        [Fact]
        public void PreservesDomStructureWhenXpath()
        {
            IXML doc =
                new XMLQuery(
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
            IXML doc = new XMLQuery("<hey/>");
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
            IXML xml = new XMLQuery("<x><a/><a/><a/></x>");

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
            IXML doc = new XMLQuery("<?xml version='1.0'?><f/>");

            Assert.True(
                doc.Node().NodeType == XmlNodeType.Document
            );
            Assert.True(
                doc.Nodes("/f")[0].Node().NodeType == XmlNodeType.Element
            );
        }

        [Fact]
        public void ComparesToAnotherDocument()
        {
            var left = new XMLQuery("<hi><dude>  </dude></hi>");
            var right = new XMLQuery("<hi><dude>  </dude></hi>");

            Assert.Equal<XMLQuery>(
                left, right
            );

            Assert.NotEqual<XMLQuery>(
                new XMLQuery("<hi><man></man></hi>"),
                new XMLQuery("<hi><man>  </man></hi>")
            );
        }

        [Fact]
        public void PreservesXmlNamespaces()
        {
            String xml = "<a xmlns='http://www.w3.org/1999/xhtml'><b/></a>";
            Assert.True(
                new LengthOf(
                    new XMLQuery(xml).Nodes("/xhtml:a/xhtml:b")
                ).Value() == 1);
        }

        [Fact]
        public void PreservesImmutability()
        {
            IXML xml = new XMLQuery("<r1><a/></r1>");
            XNode node = xml.Nodes("/r1/a")[0].Node();
            node.Remove();

            Assert.Single(
                xml.Nodes("/r1/a")
            );
        }

        [Fact]
        public void AppliesXpathToClonedNode()
        {
            IXML xml = new XMLQuery("<t6><z9 a='433'/></t6>");
            IXML root = xml.Nodes("/t6")[0];

            Assert.Equal(
                "433",
                root.Values("//z9/@a")[0]
            );
        }

        [Fact]
        public void ContainsOnlyQueriedNodes()
        {
            IXML xml = new XMLQuery("<t6><z9 a='433'><item>a</item><item>b</item></z9><z9 a='432'><item>c</item></z9></t6>");
            IXML root = xml.Nodes("/t6/z9[@a='433']")[0];

            Assert.Empty(root.Values("/z9[@a='432']"));

        }

        [Fact]
        public void NodesCanBeInterlaced()
        {
            IXML doc =
                new XMLQuery(
                    "<root><item1><subitem1/></item1><item2/><item3/></root>");
            
            Assert.Equal(
                "subitem1",
                doc.Nodes("/root/item1")[0].Nodes("./subitem1")[0].Values("name()")[0]
            );
        }
    }
}