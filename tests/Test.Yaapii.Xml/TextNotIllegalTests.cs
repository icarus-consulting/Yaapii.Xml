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
using Xunit;
using Yaapii.Atoms.Text;

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
