using System.Collections.Generic;
using System.Xml;
using Xunit;
using Yaapii.Atoms.Text;
using Yaapii.Xml;
using Yaapii.Xml.Xambly;

namespace Test.Yaapii.Xml
{
    public sealed class ModifiedXmlTests
    {
        [Fact]
        public void WorksWithString()
        {
            var modifiedXml = new ModifiedXml(Xml(), Directives());

            Assert.NotEmpty(modifiedXml.Values(XPath()));
        }

        [Fact]
        public void WorksWithText()
        {
            var xml = new TextOf(Xml());
            var modifiedXml = new ModifiedXml(xml, Directives());

            Assert.NotEmpty(modifiedXml.Values(XPath()));
        }

        [Fact]
        public void WorksWithIXML()
        {
            var xml = new XMLQuery(Xml());
            var modifiedXml = new ModifiedXml(xml, Directives());

            Assert.NotEmpty(modifiedXml.Values(XPath()));
        }

        [Fact]
        public void WorksWithXNode()
        {
            var xml = new XMLQuery(Xml()).Node();
            var modifiedXml = new ModifiedXml(xml, Directives());

            Assert.NotEmpty(modifiedXml.Values(XPath()));
        }

        [Fact]
        public void WorksWithXmlNode()
        {
            var xml = new XmlDocument();
            xml.LoadXml(Xml());

            var modifiedXml = new ModifiedXml(xml, Directives());

            Assert.NotEmpty(modifiedXml.Values(XPath()));
        }

        private string Xml()
        {
            return "<root><node><leaf name='test' /></node></root>";
        }

        private IEnumerable<IDirective> Directives()
        {
            return new Directives()
                .Xpath("//root/node/leaf")
                .Attr("id", "123")
                .Add("content")
                .Set("testContent");
        }

        private string XPath()
        {
            return "/root/node/leaf[@id='123']/content/text()";
        }
    }
}