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
using System.Reflection;
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml.Test
{
    public sealed class XSLEnvelopeTests
    {
        [Theory]
        [InlineData("<a/>", "/done")]
        [InlineData("<a></a>", "/done")]
        public void MakesXslTransformations(string input, string expected)
        {
            IXSL xsl =
                new XSLEnvelopeImplementation(
                    new TextOf(
                        new ResourceOf(
                            "Resources/CreatesDone.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmpty(),
                    new MapOf<string, object>()
                );

            Assert.Equal(
                1,
                xsl.Transformed(
                    new XMLCursor(input)
                ).Nodes(expected).Count
            );
        }

        [Fact]
        public void TransformsToText()
        {
            IXSL xsl =
                new XSLEnvelopeImplementation(
                    new TextOf(
                        new ResourceOf(
                            "Resources/CreatesHello.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmpty(),
                    new MapOf<string, object>()
            );

            Assert.Equal(
                "hello",
                xsl.TransformedToText(
                    new XMLCursor("<something/>")
                )
            );
        }

        [Fact]
        public void TransformsToTextWithParams()
        {
            IXSL xsl =
                new XSLEnvelopeImplementation(
                    new TextOf(
                        new ResourceOf(
                            "Resources/ValueOfBoom.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmpty(),
                    new MapOf<string, object>()
            );

            Assert.Equal(
                "[Donny]",
                xsl
                .With("boom", "Donny")
                .TransformedToText(
                    new XMLCursor("<ehe/>")
                )
            );
        }

        [Fact]
        public void TransformsToTextWithIntegerParams()
        {
            IXSL xsl =
                new XSLEnvelopeImplementation(
                    new TextOf(
                        new ResourceOf(
                            "Resources/ValueOfFaa.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmpty(),
                    new MapOf<string, object>()
            );

            Assert.Equal(
                "+1+",
                xsl
                .With("faa", 1)
                .TransformedToText(
                    new XMLCursor("<r0/>")
                )
            );
        }

        [Fact]
        public void ReturnsSource()
        {
            var xsl = new XSLEnvelopeImplementation(
                new TextOf(
                    new ResourceOf(
                        "Resources/first.xsl",
                        Assembly.GetExecutingAssembly()
                    )
                ),
                new SourcesEmpty(),
                new MapOf<String, Object>()
            );

            Assert.Contains(
                "<xsl:with-param name=\"value\" select=\"5.67\"",
                xsl.ToString()
            );
        }

        [Fact]
        public void TransformsWithImports()
        {
            IXSL xsl =
                new XSLEnvelopeImplementation(
                    new TextOf(
                        new ResourceOf(
                            "Resources/first.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmpty(),
                    new MapOf<string, object>()
                ).With(
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
        public void HandlesXsltExctption()
        {
            IXSL xsl =
                new XSLEnvelopeImplementation(
                    new TextOf(
                        new ResourceOf(
                            "Resources/first.xsl",
                            Assembly.GetExecutingAssembly()
                        )
                    ),
                    new SourcesEmpty(),
                    new MapOf<string, object>()
                );

            Assert.Throws<ArgumentException>(() =>
                xsl.Transformed(
                    new XMLCursor(
                        "<simple-test/>"
                    )
                )
            );
        }
    }
}
