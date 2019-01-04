using System;
using System.Collections.Generic;
using System.Text;
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
        public void ThrowsGivenException()
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
                    "/wrong/text()",
                    new ArgumentException("Wrong xpath")
                ).Value()
            );
        }

        [Fact]
        public void ThrowsGivenHint()
        {
            var doc =
                new XMLPatched(
                    "<root/>",
                    new Directives()
                        .Xpath("/root")
                        .Set("ugly_text")
                );
            try
            {
                new XMLString(
                    doc,
                    "/wrong/text()",
                    "Unable to access the text of root node"
                ).Value();
            }
            catch (Exception ex)
            {
                Assert.Equal(
                    "Unable to access the text of root node",
                    ex.Message
                );
            }
        }
    }
}
