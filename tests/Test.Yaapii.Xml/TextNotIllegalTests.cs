using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yaapii.Atoms.Text;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class TextNotIllegalTests
    {
        [Fact]
        public void PassLegalText()
        {
            var txt = new TextOf("Well formed text");
            Assert.Equal(
                txt.AsString(),
                new TextNotIllegal(txt).AsString());
        }

        [Fact]
        public void RejectsIllegalTxt()
        {
            var txt = new TextOf(Convert.ToChar(0x0E));
            try
            {
                new TextNotIllegal(txt).AsString();
                Assert.True(false, "Expected an Exception, should not get here.");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "is in the restricted XML range",
                    ex.Message);
            }
        }

        [Fact]
        public void ComparesText()
        {
            var txt = new TextOf("Wonderful text!");
            Assert.True(
                new TextNotIllegal(txt).Equals(txt));
        }
    }
}
