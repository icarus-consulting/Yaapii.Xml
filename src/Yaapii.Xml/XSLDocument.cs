using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary>
    /// A <see cref="IXSL"/> document.
    /// </summary>
    public sealed class XSLDocument : XSLEnvelope
    {
        /// <summary>
        /// XSL from a file.
        /// </summary>
        /// <param name="xsl">the xsl file</param>
        public XSLDocument(Uri file) : this(
            new InputOf(file), new SourcesEmpty()
        )
        { }

        /// <summary>
        /// XSL from a file.
        /// </summary>
        /// <param name="xsl">the xsl file</param>
        /// <param name="sources">Sources which can find imported stylesheets, like <see cref="XmlUrlResolver"/> or <see cref="SourcesEmbedded"/></param>
        public XSLDocument(Uri file, XmlResolver sources) : this(
            new InputOf(file), sources
        )
        { }

        /// <summary>
        /// XSL from a string with sources.
        /// </summary>
        /// <param name="xsl">the xsl as string</param>
        /// <param name="sources">Sources which can find imported stylesheets, like <see cref="XmlUrlResolver"/> or <see cref="SourcesEmbedded"/></param>
        public XSLDocument(IInput xsl, XmlResolver sources) : this(new TextOf(xsl), sources)
        { }

        /// <summary>
        /// XSL from a string.
        /// </summary>
        /// <param name="xsl">the xsl as string</param>
        public XSLDocument(IInput xsl) : this(new TextOf(xsl), new SourcesEmpty())
        { }

        /// <summary>
        /// XSL from a string.
        /// </summary>
        /// <param name="xsl">the xsl as string</param>
        public XSLDocument(String xsl) : this(xsl, new SourcesEmpty())
        { }

        /// <summary>
        /// XSL from a string.
        /// </summary>
        /// <param name="xsl">the xsl as string</param>
        /// <param name="sources">Sources which can find imported stylesheets, like <see cref="XmlUrlResolver"/> or <see cref="SourcesEmbedded"/></param>
        public XSLDocument(String xsl, XmlResolver sources) : this(
            new TextOf(xsl), sources
        )
        { }

        /// <summary>
        /// XSL from a <see cref="IText"/>.
        /// </summary>
        /// <param name="xsl">the xsl as string</param>
        /// <param name="sources">Sources which can find imported stylesheets, like <see cref="XmlUrlResolver"/> or <see cref="SourcesEmbedded"/></param>
        public XSLDocument(IText xsl, XmlResolver sources) : this(
            xsl, sources, new Dictionary<String, Object>()
        )
        { }

        /// <summary>
        /// XSL from a string.
        /// </summary>
        /// <param name="xsl">the xsl as string</param>
        /// <param name="sources">Sources which can find imported stylesheets, like <see cref="XmlUrlResolver"/> or <see cref="SourcesEmbedded"/></param>
        /// <param name="map">map with params needed by the transformation.</param>
        public XSLDocument(string xsl, XmlResolver sources, IDictionary<String, Object> map) : this(
            new TextOf(xsl), sources, map
        )
        { }

        /// <summary>
        /// XSL from a <see cref="IText"/>.
        /// </summary>
        /// <param name="xsl">the xsl as text</param>
        /// <param name="sources">Sources which can find imported stylesheets, like <see cref="XmlUrlResolver"/> or <see cref="SourcesEmbedded"/></param>
        /// <param name="map">map with params needed by the transformation.</param>
        public XSLDocument(IText xsl, XmlResolver sources, IDictionary<string, object> map) : base(
            xsl, sources, map
        )
        { }
    }
}
