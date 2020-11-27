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
using System.Text;
using Xunit;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class SourcesEmptyTests
    {
        [Fact]
        public void RejectsGetEntity()
        {
            try
            {
                var obj =
                    new SourcesEmpty().GetEntity(
                        new Uri("http://servername/tmp/test.xsl"),
                        string.Empty,
                        typeof(Stream));
                Assert.True(false, "Expected an Exception, should not get here.");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "No XSL sources configured - use XSLDocument.With(XMLResolver) to add them.",
                    ex.Message);
            }
        }

        [Fact]
        public void RejectsResolveUri()
        {
            try
            {
                var obj =
                    new SourcesEmpty().ResolveUri(
                        new Uri("http://servername/tmp/test.xsl"),
                        "includefile.xsl");
                Assert.True(false, "Expected an Exception, should not get here.");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    "No XSL sources configured - use XSLDocument.With(XMLResolver) to add them.",
                    ex.Message);
            }
        }
    }
}
