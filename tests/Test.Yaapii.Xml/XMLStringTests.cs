// MIT License
//
// Copyright(c) 2019 ICARUS Consulting GmbH
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
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Xambly;

namespace Yaapii.Xml.Test
{
    public sealed class XMLStringTests
    {
        [Fact]
        public void DeliversString()
        {
            var doc =
                new XMLPatched(
                    "<root/>",
                    new Directives()
                        .Xpath("/root")
                        .Set("ugly_text")
                );
            Assert.Equal(
                "ugly_text",
                new XMLString(
                    doc,
                    "/root/text()",
                    "Unable to access the text of root node"
                ).Value()
            );
        }

        [Fact]
        public void RejectsNoResults()
        {
            var doc =
                new XMLPatched(
                    "<root/>",
                    new Directives()
                        .Xpath("/root")
                        .Set("ugly_text")
                );
            Assert.Throws<ArgumentException>(() =>
                new XMLString(
                    doc,
                    "/wrong/text()"
                ).Value()
            );
        }

        [Fact]
        public void ReturnsFallback()
        {
            var doc =
                new XMLPatched(
                    "<root/>",
                    new Directives()
                        .Xpath("/root")
                        .Set("ugly_text")
                );

            Assert.Equal(
                "right text",
                new XMLString(
                    doc,
                    "/wrong/text()",
                    "right text"
                ).Value()
            );
        }

        [Fact]
        public void ReturnsXmlInExceptionMessage()
        {
            var xml =
                new XMLCursor(
                    new ResourceOf(
                        "Resources/simple.xml",
                        this.GetType()
                    ).Stream()
                );
            try
            {
                new XMLString(xml, "root/complex").Value();
            }
            catch (Exception ex)
            {
                Assert.Contains("" +
                    "Cannot retrieve single value with XPath 'root/complex', because it had no results in document\r\n'\r\n<root>\r\n  <simple>hello</simple>\r\n</root>\r\n'.",
                    ex.Message
                );
            }
        }
    }
}
