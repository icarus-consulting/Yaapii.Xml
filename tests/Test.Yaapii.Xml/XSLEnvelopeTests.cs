using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Text;
using Yaapii.Xml;

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
