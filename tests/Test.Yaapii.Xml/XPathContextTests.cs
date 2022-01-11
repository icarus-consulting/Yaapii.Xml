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

using System.Xml;
using Xunit;
using Yaapii.Atoms.Enumerable;

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
