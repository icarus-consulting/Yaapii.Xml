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
using System.IO;
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml.Test
{
    public sealed class XSLEnvelopeTests
    {
        [Fact]
        public void ReturnsSource()
        {
            var xsl = new XSLEnvelopeRealization(
                new TextOf(
                    new InputOf(
                        new SourcesEmbedded(
                            this.GetType(),
                            "Resources"
                        ).GetEntity(
                            new Uri("A:/first.xsl"),    // only the filename is relevant
                            string.Empty,               // not evaluated
                            typeof(object)              // not evaluated
                        ) as Stream
                    )
                ),
                new SourcesEmpty(),
                new Dictionary<String, Object>()
            );

            Assert.Contains(
                "<xsl:with-param name=\"value\" select=\"5.67\"",
                xsl.ToString()
            );
        }

        //[Fact]
        //public void xxx()
        //{
        //    var xsl = new XSLEnvelopeRealization(
        //        new TextOf("<root />"),
        //        new SourcesEmpty(),
        //        new Dictionary<String, Object>()
        //    ).With("xslArgName", "xslArgValue");

        //    var sxlDoc = new XSLDocument(xsl, new SourcesEmpty());

        //    //Assert.Equal(
        //    //    "xslArgValue",
        //    //    xsl.
        //    //);
        //}
    }
}
