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

using System.Reflection;
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Xml;

namespace Yaapii.Xml.Test
{
    public sealed class XSLDocumentTests
    {
        [Theory]
        [InlineData("<a/>")]
        [InlineData("<a></a>")]
        public void MakesXslTransformations(string xml)
        {
            IXSL xsl =
                new XSLDocument(
                    @"<xsl:stylesheet
                    xmlns:xsl='http://www.w3.org/1999/XSL/Transform' 
                    version='2.0'>
                    <xsl:template match='/'><done/>
                    </xsl:template></xsl:stylesheet>"
                );

            Assert.Equal(
                1, 
                xsl.Transformed(new XMLCursor(xml)).Nodes("/done").Count
            );
        }

        [Fact]
        public void TransformsWithImports()
        {
            IXSL xsl =
                new XSLDocument(
                    new ResourceOf(
                        "Resources/first.xsl",
                        Assembly.GetExecutingAssembly()
                    )
                ).With(
                    new SourcesEmbedded(
                        Assembly.GetExecutingAssembly(),
                        "Resources"
                    )
                );
            Assert.Equal(
                1,
                xsl.Transformed(
                    new XMLCursor(
                        "<simple-test/>"
                    )
                ).Nodes("/result[.=6]").Count
            );
        }

        [Fact]
        public void TransformsToText()
        {
            IXSL xsl =
                new XSLDocument(
                    @"<xsl:stylesheet 
                        xmlns:xsl='http://www.w3.org/1999/XSL/Transform'  
                        version='2.0'><xsl:output method='text'/>
                    <xsl:template match='/'>hello</xsl:template></xsl:stylesheet>"
                );
            Assert.Equal(
                "hello",
                xsl.TransformedToText(
                    new XMLCursor("<something/>")
                )
            );
        }

        [Fact]
        public void TransformsToTextWithParams()
        {
            IXSL xsl =
                new XSLDocument(
                    @"<xsl:stylesheet   
                     xmlns:xsl='http://www.w3.org/1999/XSL/Transform'    
                     xmlns:xs='http://www.w3.org/2001/XMLSchema'
                     version='2.0'><xsl:output method='text'  />
                    <xsl:param name='boom' />
                    <xsl:template match='/'>[<xsl:value-of select='$boom'/>]</xsl:template>   </xsl:stylesheet>"
                );
            Assert.Equal(
                "[Donny]",
                xsl
                    .With("boom", "Donny")
                    .TransformedToText(new XMLCursor("<ehe/>"))
            );
        }

        [Fact]
        public void TransformsToTextWithIntegerParams()
        {
            IXSL xsl =
                new XSLDocument(
                    @"<xsl:stylesheet     
                     xmlns:xsl='http://www.w3.org/1999/XSL/Transform'       
                     version='2.0'><xsl:output method='text'    />
                    <xsl:param name='faa' as='xs:integer' select='5'/>
                    <xsl:template match='/'>+<xsl:value-of select='$faa'/>+</xsl:template>   
                    </xsl:stylesheet>  "
                );
            Assert.Equal(
                "+1+",
                xsl
                    .With("faa", 1)
                    .TransformedToText(new XMLCursor("<r0/>"))
            );
        }
    }
}
