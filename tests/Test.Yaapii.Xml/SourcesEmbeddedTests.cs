using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Text;
using Yaapii.Xml;

namespace Test.Yaapii.Xml
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
