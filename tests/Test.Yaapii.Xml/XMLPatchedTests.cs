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

using Xunit;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

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
                    new XMLCursor("<root><node><leaf name='test' /></node></root>"),
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
                new XMLCursor(
                    "<root><node><leaf name='test' /></node></root>"
                ).AsNode();
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