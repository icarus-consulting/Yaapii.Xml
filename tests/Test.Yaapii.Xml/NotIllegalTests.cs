using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yaapii.Xml;

namespace Test.Yaapii.Xml
{
    public sealed class NotIllegalTests
    {
        [Theory]
        [InlineData(0x09)]
        [InlineData(0x0D)]
        [InlineData(0x20)]
        [InlineData(0x85)]
        [InlineData(0xA0)]
        [InlineData(0xFF)]
        public void PassLegalChar(int charEncodeing)
        {
            var chr = Convert.ToChar(charEncodeing);

            Assert.True(
                new NotIllegal(chr).Value() == chr);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(0x01)]
        [InlineData(0x02)]
        [InlineData(0x03)]
        [InlineData(0x04)]
        [InlineData(0x05)]
        [InlineData(0x06)]
        [InlineData(0x07)]
        [InlineData(0x08)]
        [InlineData(0x0B)]
        [InlineData(0x0C)]
        [InlineData(0x0E)]
        [InlineData(0x0F)]
        [InlineData(0x10)]
        [InlineData(0x11)]
        [InlineData(0x12)]
        [InlineData(0x13)]
        [InlineData(0x14)]
        [InlineData(0x15)]
        [InlineData(0x16)]
        [InlineData(0x17)]
        [InlineData(0x18)]
        [InlineData(0x19)]
        [InlineData(0x1A)]
        [InlineData(0x1B)]
        [InlineData(0x1C)]
        [InlineData(0x1D)]
        [InlineData(0x1E)]
        [InlineData(0x1F)]
        [InlineData(0x7F)]
        [InlineData(0x80)]
        [InlineData(0x81)]
        [InlineData(0x82)]
        [InlineData(0x83)]
        [InlineData(0x84)]
        [InlineData(0x86)]
        [InlineData(0x87)]
        [InlineData(0x88)]
        [InlineData(0x89)]
        [InlineData(0x8A)]
        [InlineData(0x8B)]
        [InlineData(0x8C)]
        [InlineData(0x8D)]
        [InlineData(0x8E)]
        [InlineData(0x8F)]
        [InlineData(0x90)]
        [InlineData(0x91)]
        [InlineData(0x92)]
        [InlineData(0x93)]
        [InlineData(0x94)]
        [InlineData(0x95)]
        [InlineData(0x96)]
        [InlineData(0x97)]
        [InlineData(0x98)]
        [InlineData(0x99)]
        [InlineData(0x9A)]
        [InlineData(0x9B)]
        [InlineData(0x9C)]
        [InlineData(0x9D)]
        [InlineData(0x9E)]
        [InlineData(0x9F)]
        public void RejectsIllegalChar(int charEncodeing)
        {
            var chr = Convert.ToChar(charEncodeing);

            try
            {
                new NotIllegal(chr).Value();
                Assert.True(false, "Expected an Exception, should not get here.");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "is in the restricted XML range",
                    ex.Message);
            }
        }
    }
}
