using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public class XSLStrippedTests
    {
        [Fact]
        public void StripsXml()
        {
            Assert.True(
            new XSLStripped().Transformed(
                new XMLQuery("<a>   <b/>  </a>")
            ).ToString() ==
            new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>") //mtu\unsure about this: should it be here or not? I think, the input is NOT a document so normally the declaration should not appear.
                .Append("<a>")
                .Append("<b />")
                .Append("</a>")
                .ToString()
            );
        }
    }
}
