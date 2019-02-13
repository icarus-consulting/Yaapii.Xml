using System;
using Yaapii.Atoms;
using Yaapii.Atoms.Text;
using Yaapii.Xml.Error;

namespace Yaapii.Xml
{
    /// <summary>
    /// A string in a document, retrieved by xpath.
    /// </summary>
    public sealed class XMLString : IScalar<string>
    {
        private readonly IXML xml;
        private readonly string xpath;
        private readonly Func<string> fallback;

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public XMLString(IXML xml, string xpath) : this(xml, xpath, 
            () =>
            {
                throw
                    new ArgumentException(
                        new FormattedText(
                            $"Cannot retrieve single value with XPath '{0}', because it had no results in document{Environment.NewLine} {1}",
                            xpath,
                            xml.AsNode().ToString()
                        ).AsString()
                    );
            }
        )
        { }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public XMLString(IXML xml, string xpath, string def) : this(xml, xpath, () => def)
        { }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        internal XMLString(IXML xml, string xpath, Func<string> fallback)
        {
            this.xml = xml;
            this.xpath = xpath;
            this.fallback = fallback;
        }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public string Value()
        {
            var matches = this.xml.Values(xpath);
            var result = string.Empty;
            if (matches.Count < 1)
            {
                result = this.fallback();
            }
            else if(matches.Count > 1)
            {
                throw 
                    new ArgumentException(
                        $"Cannot extract single value with xpath {this.xpath} because it resulted in multiple values in document {xml.ToString()}"
                    );
            }
            else
            {
                result = matches[0];
            }
            return result;
        }
    }
}
