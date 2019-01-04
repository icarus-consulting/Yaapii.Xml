using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;
using Yaapii.Atoms.Enumerable;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class XPathContextTests
    {
        [Theory]
        [InlineData("xhtml", "http://www.w3.org/1999/xhtml")]
        [InlineData("xs", "http://www.w3.org/2001/XMLSchema")]
        [InlineData("xsi", "http://www.w3.org/2001/XMLSchema-instance")]
        [InlineData("xsl", "http://www.w3.org/1999/XSL/Transform")]
        [InlineData("svg", "http://www.w3.org/2000/svg")]
        public void FindsNamespace(string prefix, string namespaceName)
        {
            Assert.Equal(
                namespaceName,
                new XPathContext().LookupNamespace(prefix));
        }

        [Theory]
        [InlineData(XmlNamespaceScope.All, 7)]
        [InlineData(XmlNamespaceScope.ExcludeXml, 5)]
        [InlineData(XmlNamespaceScope.Local, 7)]
        public void FiltersByScope(XmlNamespaceScope scope, int count)
        {
            // Default namespace and additional xml namespace
            var withXml = new XPathContext(
                new XPathContext().GetNamespacesInScope(XmlNamespaceScope.All),
                "xml",
                "http://www.w3.org/XML/1998/namespace");
            // Default namespace and additional xml and smlns namespace 
            var withXmlAndXmlns = new XPathContext(
                withXml.GetNamespacesInScope(XmlNamespaceScope.All),
                "xmlns",
                "http://www.w3.org/2000/xmlns/");

            var y = withXmlAndXmlns.GetNamespacesInScope(scope);

            Assert.True(
                new LengthOf(withXmlAndXmlns.GetNamespacesInScope(scope)).Value() == count);
        }

        [Theory]
        [InlineData("xhtml", "http://www.w3.org/1999/xhtml")]
        [InlineData("xs", "http://www.w3.org/2001/XMLSchema")]
        [InlineData("xsi", "http://www.w3.org/2001/XMLSchema-instance")]
        [InlineData("xsl", "http://www.w3.org/1999/XSL/Transform")]
        [InlineData("svg", "http://www.w3.org/2000/svg")]
        public void FindsPrefix(string prefix, string namespaceName)
        {
            Assert.Equal(
                prefix,
                new XPathContext().LookupPrefix(namespaceName)
            );
        }

        [Fact]
        public void HasCustomUris()
        {
            Assert.True(
                new LengthOf(
                    new XPathContext(
                        "http://nice-uri",
                        "http://best.address"
                    ).GetNamespacesInScope(XmlNamespaceScope.All)
                ).Value() == 2
            );
        }
    }
}
