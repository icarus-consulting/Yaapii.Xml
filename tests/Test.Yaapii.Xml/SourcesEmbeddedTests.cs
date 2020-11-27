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
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Text;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class SourcesEmbeddedTests
    {
        [Fact]
        public void GetsEntityFromType()
        {
            var strm =
                new SourcesEmbedded(
                    this.GetType(),
                    "Resources"
                ).GetEntity(
                    new Uri("A:/simple.xml"),   // only the filename is relevant
                    string.Empty,           // not evaluated
                    typeof(object));        // not evaluated

            Assert.Contains(
                "hello",
                new TextOf(new InputOf(strm as Stream)).AsString());
        }

        [Fact]
        public void GetsEntityFromAssembly()
        {
            var strm =
                new SourcesEmbedded(
                    this.GetType().Assembly,
                    "Resources"
                ).GetEntity(
                    new Uri("A:/simple.xml"),   // only the filename is relevant
                    string.Empty,           // not evaluated
                    typeof(object));        // not evaluated

            Assert.Contains(
                "hello",
                new TextOf(new InputOf(strm as Stream)).AsString());
        }

        [Fact]
        public void RejectsOnNotExistingResource()
        {
            var resName = "notExisting.xml";
            try
            {
                var strm =
                new SourcesEmbedded(
                    this.GetType(),
                    "Resources"
                ).GetEntity(
                    new Uri("A:/" + resName),   // only the filename is relevant
                    string.Empty,           // not evaluated
                    typeof(object));        // not evaluated

                Assert.True(false, "Expected an Exception, should not get here.");
            }
            catch (Exception ex)
            {
                Assert.Contains(
                    $"Cannot resolve stylesheet '{resName}' XSL-sources currently pointing to assembly resources - use XSLDocument.With(XMLResolver) to use another.",
                    ex.Message);
            }
        }
    }
}
