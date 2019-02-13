using Xunit;

namespace Yaapii.Xml.Test
{
    public sealed class XMLSliceTests
    {
        [Fact]
        public void CreatesNewDocument()
        {
            var node = new XMLSlice("<root><slice>text</slice></root>").Nodes("/root/slice")[0].AsNode();
            Assert.Equal(
                node.ToString(),
                node.Document.ToString()
            );
        }
    }
}
