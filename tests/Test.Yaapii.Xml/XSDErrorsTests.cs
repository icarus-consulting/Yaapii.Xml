using Xunit;
using Yaapii.Atoms.IO;

namespace Yaapii.Xml.Test
{
    public sealed class XSDErrorsTests
    {
        [Fact]
        public void ValidXml()
        {
            Assert.Empty(
                new XSDErrors(
                    new XMLQuery(new ResourceOf("Assets/example.xml", typeof(IXML))),
                    new XMLQuery(new ResourceOf("Assets/example-schema.xsd", typeof(IXML)))
                )
            );
        }
    }
}
