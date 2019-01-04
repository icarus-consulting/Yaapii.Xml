using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Yaapii.Atoms;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary>
    /// Envelope for XSL.
    /// </summary>
    public abstract class XSLEnvelope : IXSL //abstract is only used here to prevent direct instantiating. Usually abstract is prohibited in this project.
    {
        /// <summary>
        /// XSL Document.
        /// </summary>
        private readonly IText xsl;

        /// <summary>
        /// Stylesheet params.
        /// </summary>
        private readonly IDictionary<string, object> @params;

        /// <summary>
        /// Stylesheet Sources.
        /// </summary>
        private readonly XmlResolver sources;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="sources"></param>
        /// <param name="map"></param>
        /// <param name="bse"></param>
        public XSLEnvelope(IText src, XmlResolver sources, IDictionary<string, object> map)
        {
            this.xsl = src;
            this.sources = sources;
            this.@params = map;
        }

        /// <summary>
        /// The transformed XML.
        /// </summary>
        /// <param name="xml">input XML</param>
        /// <returns>transformed output</returns>
        public IXML Transformed(IXML xml)
        {
            var transformed = new XDocument();
            var output = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.ConformanceLevel = ConformanceLevel.Auto;

            this.ErrorHandled(() =>
            {
                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    this.Xslt().Transform(
                        xml.Node().CreateReader(),
                        Params(),
                        writer
                    );
                }
            });

            return new XMLQuery(output.ToString());
        }

        /// <summary>
        /// The XML transformed to simple text, no XML document.
        /// </summary>
        /// <param name="xml">input XML</param>
        /// <returns>transformed output</returns>
        public string TransformedToText(IXML xml)
        {
            StringBuilder sb = new StringBuilder();

            this.ErrorHandled(() =>
            {
                using (var writer = new StringWriter(sb))
                {
                    this.Xslt().Transform(
                        xml.Node().CreateReader(),
                        Params(),
                        writer
                    );
                }
            });

            return sb.ToString();
        }

        /// <summary>
        /// A new XSL with the given parameter.
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">content of the parameter</param>
        /// <returns>XSL with the parameter</returns>
        public IXSL With(string name, object value)
        {
            return new XSLDocument(
                this.xsl,
                this.sources,
                new MapOf<string, object>(
                    this.@params,
                    new KeyValuePair<string, object>(
                        name, value)
                    )
            );
        }

        /// <summary>
        /// The XSL formatted as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new XMLQuery(this.xsl).ToString();
        }

        /// <summary>
        /// The loaded stylesheet.
        /// </summary>
        /// <returns></returns>
        private XslCompiledTransform Xslt()
        {
            // Load the style sheet.  
            var xslt = new XslCompiledTransform();
            var sets = new XsltSettings(true, true);
            xslt.Load(
                XmlReader.Create(
                    new StringReader(this.xsl.AsString())
                ),
                sets,
                sources
            );

            return xslt;
        }

        /// <summary>
        /// Encapsulate error handling for the fetch functions.
        /// </summary>
        /// <param name="act">function to encapsulate</param>
        private void ErrorHandled(Action act)
        {
            try
            {
                act.Invoke();
            }
            catch (XsltException xex)
            {
                throw
                    new ArgumentException(
                        new FormattedText(
                            "invalid xslt: {0}\r\nContent:\r\n {1}",
                            xex.Message + (xex.InnerException != null ? ", " + xex.InnerException.Message : ""),
                            this.xsl.AsString()
                        ).AsString(),
                        xex
                    );
            }
        }

        private XsltArgumentList Params()
        {
            var args = new XsltArgumentList();
            foreach (var kvp in this.@params)
            {
                args.AddParam(kvp.Key, "", kvp.Value);
            }
            return args;
        }

        public IXSL With(XmlResolver sources)
        {
            return new XSLDocument(this.xsl, sources, this.@params);
        }
    }
}
