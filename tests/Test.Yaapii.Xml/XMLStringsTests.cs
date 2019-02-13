using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace Yaapii.Xml.Test
{
    public sealed class XMLStringsTests
    {
        [Fact]
        public void DeliversStrings()
        {
            Assert.Equal(
                "1,2",
                new JoinedText(",",
                    new XMLStrings(
                        "/a/b/text()",
                        new XMLCursor("<a><b>1</b><b>2</b></a>")
                    )
                ).AsString()
            );
                
        }
    }
}
