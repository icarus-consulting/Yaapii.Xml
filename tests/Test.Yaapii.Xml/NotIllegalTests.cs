// MIT License
//
// Copyright(c) 2019 ICARUS Consulting GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
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
