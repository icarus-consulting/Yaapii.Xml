using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Yaapii.Xml;

namespace Test.Yaapii.Xml
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
