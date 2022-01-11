// MIT License
//
// Copyright(c) 2022 ICARUS Consulting GmbH
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

using System.Text;
using Xunit;

namespace Yaapii.Xml.Test
{
    public class XSLStrippedTests
    {
        [Fact]
        public void StripsXml()
        {
            Assert.True(
            new XSLStripped().Transformed(
                new XMLCursor("<a>   <b/>  </a>")
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
