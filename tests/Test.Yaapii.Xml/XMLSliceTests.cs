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
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml.Test
{
    public sealed class XMLSliceTests
    {
        [Fact]
        public void CreatesNewDocument()
        {
            var node = new XMLSlice("<root><slice>text</slice></root>").Nodes("/root/slice")[0].AsNode();
            Assert.Equal(
                node.ToString(),
                node.Document.ToString()
            );
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
                new XMLSlice(
                    new Yaapii.Atoms.IO.InputOf(inBytes),
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
                new XMLSlice(
                    new Yaapii.Atoms.IO.InputOf(inBytes).Stream(),
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
                    new XMLSlice(
                        new Uri("file:///" + path),
                        encoding
                    ).Values("/root/text()")[0]
                );
            }
        }

        [Fact]
        public void ValuesWorkWithNamespace()
        {
            Assert.Equal(
                "Content",
                FirstOf.New(
                    new XMLSlice(
                        new ResourceOf("Resources/xmlWithNamespace.xml", this.GetType())
                    ).WithNamespace(
                        "n0", "http://standards.iso.org/iso/ts/10303/-3001/-ed-2/tech/xml-schema/bo_model"
                    )
                    .Values(
                        "/n0:Root/n0:A/n0:B/text()"
                    )
                ).Value()
            );
        }

        [Fact]
        public void NodesWorkWithNamespace()
        {
            Assert.Single(
                new XMLSlice(
                    new ResourceOf("Resources/xmlWithNamespace.xml", this.GetType())
                ).WithNamespace(
                    "n0", "http://standards.iso.org/iso/ts/10303/-3001/-ed-2/tech/xml-schema/bo_model"
                )
                .Nodes(
                    "/n0:Root/n0:A"
                )
            );
        }

        [Fact]
        public void NodesDeliverXMLThatKnowNamespace()
        {
            var subNode =
                FirstOf.New(
                    new XMLSlice(
                        new ResourceOf("Resources/xmlWithNamespace.xml", this.GetType())
                    ).WithNamespace(
                        "n0", "http://standards.iso.org/iso/ts/10303/-3001/-ed-2/tech/xml-schema/bo_model"
                    )
                    .Nodes(
                        "/n0:Root/n0:A"
                    )
                ).Value();
            Assert.Single(
                subNode.Nodes("/n0:A/n0:B")
            );
        }
    }
}
