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
                xsl.Transformed(new XMLQuery(xml)).Nodes("/done").Count
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
                    new XMLQuery(
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
                    new XMLQuery("<something/>")
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
                    .TransformedToText(new XMLQuery("<ehe/>"))
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
                    .TransformedToText(new XMLQuery("<r0/>"))
            );
        }
    }
}
