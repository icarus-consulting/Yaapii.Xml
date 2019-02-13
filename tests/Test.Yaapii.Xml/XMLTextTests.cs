using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Yaapii.Xml.Test
{
    public sealed class XMLTextTests
    {
        [Fact]
        public void DeliversValue()
        {
            Assert.Equal(
                "hello",
                new XMLText(
                    new XMLCursor("<root><a>hello</a></root>"),
                    "/root/a/text()"
                ).AsString()
            );
        }
    }
}
