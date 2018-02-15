using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Yaapii.Atoms.Fail;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary>
    /// Empty XSL sources. Used as dummy internally.
    /// </summary>
    public sealed class SourcesEmpty : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            throw
                new UnsupportedOperationException(
                    new FormattedText(
                        "Cannot resolve stylesheet '{0}' No XSL sources configured - use XSLDocument.With(XMLResolver) to add them.",
                        Path.GetFileName(absoluteUri.AbsoluteUri)
                    ).AsString()
                );
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            throw
                new UnsupportedOperationException(
                    new FormattedText(
                        "Cannot resolve stylesheet '{0}' No XSL sources configured - use XSLDocument.With(XMLResolver) to add them.",
                        Path.GetFileName(relativeUri)
                    ).AsString()
                );
        }
    }
}
