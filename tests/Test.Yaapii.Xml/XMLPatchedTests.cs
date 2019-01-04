using Xunit;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class XMLPatchedTests
    {
        [Fact]
        public void WorksWithString()
        {
            var modifiedXml =
                new XMLPatched(
                    "<root><node><leaf name='test' /></node></root>",
                    new Directives()
                        .Xpath("//root/node/leaf")
                        .Attr("id", "123")
                        .Add("content")
                        .Set("testContent")
                );
            Assert.NotEmpty(
                modifiedXml.Values("/root/node/leaf[@id='123']/content/text()")
            );
        }

        [Fact]
        public void WorksWithText()
        {
            var modifiedXml =
                new XMLPatched(
                    new TextOf("<root><node><leaf name='test' /></node></root>"),
                    new Directives()
                        .Xpath("//root/node/leaf")
                        .Attr("id", "123")
                        .Add("content")
                        .Set("testContent")
                );
            Assert.NotEmpty(
                modifiedXml.Values("/root/node/leaf[@id='123']/content/text()")
                );
        }

        [Fact]
        public void WorksWithIXML()
        {
            var modifiedXml =
                new XMLPatched(
                    new XMLQuery("<root><node><leaf name='test' /></node></root>"),
                    new Directives()
                        .Xpath("//root/node/leaf")
                        .Attr("id", "123")
                        .Add("content")
                        .Set("testContent")
                );
            Assert.NotEmpty(
                modifiedXml.Values("/root/node/leaf[@id='123']/content/text()")
            );
        }

        [Fact]
        public void WorksWithXNode()
        {
            var xml =
                new XMLQuery(
                    "<root><node><leaf name='test' /></node></root>"
                ).Node();
            var modifiedXml =
                new XMLPatched(
                    xml,
                    new Directives()
                        .Xpath("//root/node/leaf")
                        .Attr("id", "123")
                        .Add("content")
                        .Set("testContent")
                );
            Assert.NotEmpty(
                modifiedXml.Values("/root/node/leaf[@id='123']/content/text()")
            );
        }

        [Fact]
        public void WorksWithXmlNode()
        {
            var modifiedXml =
                new XMLPatched(
                    "<root><node><leaf name='test' /></node></root>",
                    new Directives()
                        .Xpath("//root/node/leaf")
                        .Attr("id", "123")
                        .Add("content")
                        .Set("testContent")
                );
            Assert.NotEmpty(
                modifiedXml.Values("/root/node/leaf[@id='123']/content/text()")
            );
        }
    }
}