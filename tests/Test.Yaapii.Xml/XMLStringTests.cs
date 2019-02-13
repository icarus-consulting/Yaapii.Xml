using System;
using Xunit;
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
    }
}
