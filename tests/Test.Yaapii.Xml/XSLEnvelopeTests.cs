using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;
using Yaapii.Asserts.Xml;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;
using Yaapii.IO;
using Yaapii.Xml;

namespace Test.Yaapii.Xml
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

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery(input)),
                expected);
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
                    new XMLQuery("<something/>")
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
                    new XMLQuery("<ehe/>")
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
                    new XMLQuery("<r0/>")
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

            AssertXml.HasNode(
                xsl.Transformed(
                    new XMLQuery(
                        "<simple-test/>"
                    )
                ),
                "/result[.=6]"
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
                    new XMLQuery(
                        "<simple-test/>"
                    )
                )
            );
        }
    }
}
