using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Yaapii.Asserts.Xml;
using Yaapii.Atoms.IO;
using Yaapii.Xml;
using Yaapii.Atoms.Text;
using System.IO;
using Yaapii.Atoms.Map;
using Yaapii.IO;

namespace Test.Yaapii.Xml
{
    public sealed class XSLDocumentTests
    {
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

        // first
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

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<simple-test/>")),
                "/result[.=6]"
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

        // gleich mit unten
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

        // gleich mit oben
        [Fact]
        public void TakesITextAndResolverAndDictionary()
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
