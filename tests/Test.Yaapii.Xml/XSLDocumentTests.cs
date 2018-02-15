using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;
using Yaapii.Asserts.Xml;
using Yaapii.Atoms.IO;
using Yaapii.Xml;
using Yaapii.Atoms.Text;
using Yaapii.IO;

namespace Test.Yaapii.Xml
{
    public sealed class XSLDocumentTests
    {
        [Fact]
        public void MakesXslTransformations()
        {
            IXSL xsl = new XSLDocument(
                    @"<xsl:stylesheet
                    xmlns:xsl='http://www.w3.org/1999/XSL/Transform' 
                    version='2.0'>
                    <xsl:template match='/'><done/>
                    </xsl:template></xsl:stylesheet>"
            );

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<a/>")),
                "/done");

            AssertXml.HasNode(
                xsl.Transformed(new XMLQuery("<a></a>")),
                "/done"
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


            AssertXml.HasNode(
                xsl.Transformed(
                    new XMLQuery(
                        "<simple-test/>"
                    )
                ),
                "/result[.=6]"
            );
        }

        [Fact]
        public void TransformsToText()
        {
            IXSL xsl = new XSLDocument(
                @"<xsl:stylesheet 
                    xmlns:xsl='http://www.w3.org/1999/XSL/Transform'  
                    version='2.0'><xsl:output method='text'/>
                <xsl:template match='/'>hello</xsl:template></xsl:stylesheet>"
            );

            Assert.Equal(
                "hello",
                xsl.TransformedToText(
                    new XMLQuery("<something/>")
                )
            );
        }

        [Fact]
        public void TransformsToTextWithParams()
        {
            IXSL xsl = new XSLDocument(
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
                .TransformedToText(
                    new XMLQuery("<ehe/>")
                )
            );
        }

        [Fact]
        public void TransformsToTextWithIntegerParams()
        {
            IXSL xsl = new XSLDocument(
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
                .TransformedToText(
                    new XMLQuery("<r0/>")
                )
            );
        }
    }
}
