using Xunit;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml.Test
{
    public sealed class XMLTextsTests
    {
        [Fact]
        public void DeliversTexts()
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
