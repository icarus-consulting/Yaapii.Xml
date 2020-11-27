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

using System.Reflection;
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class XSLDocumentTests
    {
        [Theory]
        [InlineData("<a/>")]
        [InlineData("<a></a>")]
        public void MakesXslTransformations(string xml)
        {
            IXSL xsl =
                new XSLDocument(
                    @"<xsl:stylesheet
                    xmlns:xsl='http://www.w3.org/1999/XSL/Transform' 
                    version='2.0'>
                    <xsl:template match='/'><done/>
                    </xsl:template></xsl:stylesheet>"
                );

            Assert.Equal(
                1,
                xsl.Transformed(new XMLCursor(xml)).Nodes("/done").Count
            );
        }

        [Fact]
        public void TakesUri()
        {
            new TidyFileList(
                Path.Combine(Directory.GetCurrentDirectory(), "Test"),
                new MapOf<string, string>(
                    new KeyValuePair<string, string>(
                        "demo.xsl",
                        new TextOf(
                            new ResourceOf(
                                "Resources/CreatesDone.xsl",
                                Assembly.GetExecutingAssembly()
                            )
                        ).AsString()
                    )
                ),
                () =>
                {
                    IXSL xsl =
                        new XSLDocument(
                            new Uri("file://" + Path.Combine(Directory.GetCurrentDirectory(), "Test/demo.xsl"))
                        );

                    AssertXml.HasNode(
                        xsl.Transformed(new XMLQuery("<a/>")),
                        "/done"
                    );
                }
            ).Invoke();
        }

        [Fact]
        public void TakesUriAndResolver()
        {
            new TidyFileList(
                Path.Combine(Directory.GetCurrentDirectory(), "Test"),
                new MapOf<string, string>(
                    new KeyValuePair<string, string>(
                        "first.xsl",
                        new TextOf(
                            new ResourceOf(
                                "Resources/first.xsl",
                                Assembly.GetExecutingAssembly()
                            )
                        ).AsString()
                    )
                ),
                () =>
                {
                    IXSL xsl =
                        new XSLDocument(
                            new Uri("file://" + Path.Combine(Directory.GetCurrentDirectory(), "Test/first.xsl")),
                            new SourcesEmbedded(
                                Assembly.GetExecutingAssembly(),
                                "Resources"
                            )
                        );

                    AssertXml.HasNode(
                        xsl.Transformed(new XMLQuery("<simple-test/>")),
                        "/result[.=6]"
                    );
                }
            ).Invoke();
        }

        [Fact]
        public void TakesIInput()
        {
            IXSL xsl =
                new XSLDocument(
                        new ResourceOf(
                            "Resources/CreatesDone.xsl",
                            Assembly.GetExecutingAssembly()
                    )
                );

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<a/>")),
                "/done"
            );
        }

        [Fact]
        public void TakesIInputAndResolver()
        {
            IXSL xsl =
                new XSLDocument(
                    new ResourceOf(
                        "Resources/first.xsl",
                        Assembly.GetExecutingAssembly()
                    ),
                    new SourcesEmbedded(
                        Assembly.GetExecutingAssembly(),
                        "Resources"
                    )
                );
            Assert.Equal(
                1,
                xsl.Transformed(
                    new XMLCursor(
                        "<simple-test/>"
                    )
                ).Nodes("/result[.=6]").Count
            );
        }

        [Fact]
        public void TakesString()
        {
            IXSL xsl =
                new XSLDocument(
                    new TextOf(
                        new ResourceOf(
                            "Resources/CreatesDone.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ).AsString()
                );

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<a/>")),
                "/done"
            );
        }

        [Fact]
        public void TakesStringAndResolver()
        {
            IXSL xsl =
                new XSLDocument(
                    new TextOf(
                        new ResourceOf(
                            "Resources/first.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ).AsString(),
                    new SourcesEmbedded(
                        Assembly.GetExecutingAssembly(),
                        "Resources"
                    )
                );

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<simple-test/>")),
                "/result[.=6]"
            );
        }

        [Fact]
        public void TakesStringAndResolverAndDictionary()
        {
            IXSL xsl =
                new XSLDocument(
                    new TextOf(
                        new ResourceOf(
                            "Resources/firstWithParam.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ).AsString(),
                    new SourcesEmbedded(
                        Assembly.GetExecutingAssembly(),
                        "Resources"
                    ),
                    new MapOf<string, object>(
                        new KeyValuePair<string, object>("faa", 9)
                    )
                );

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<simple-test/>")),
                "/result/number[text() = 9]"
            );
        }

        [Fact]
        public void TakesITextResolverAndDictionary()
        {
            IXSL xsl =
                new XSLDocument(
                    new TextOf(
                        new ResourceOf(
                            "Resources/firstWithParam.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmbedded(
                        Assembly.GetExecutingAssembly(),
                        "Resources"
                    ),
                    new MapOf<string, object>(
                        new KeyValuePair<string, object>("faa", 9)
                    )
                );

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<simple-test/>")),
                "/result/number[text() = 9]"
            );
        }
    }
}
