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

namespace Yaapii.Xml.Test
{
    public sealed class XMLNumberTests
    {
        [Fact]
        public void ExtractsDouble()
        {
            Assert.Equal(
                1.2,
                new XMLNumber(
                    new XMLCursor("<root>1.2</root>"),
                    "/root/text()"
                ).AsDouble()
            );
        }

        [Fact]
        public void ExtractsInt()
        {
            Assert.Equal(
                2,
                new XMLNumber(
                    new XMLCursor("<root>2</root>"),
                    "/root/text()"
                ).AsInt()
            );
        }

        [Fact]
        public void ExtractsFloat()
        {
            Assert.Equal(
                2.3f,
                new XMLNumber(
                    new XMLCursor("<root>2.3</root>"),
                    "/root/text()"
                ).AsFloat()
            );
        }

        [Fact]
        public void ExtractsLong()
        {
            Assert.Equal(
                23,
                new XMLNumber(
                    new XMLCursor("<root>23</root>"),
                    "/root/text()"
                ).AsLong()
            );
        }
    }
}
