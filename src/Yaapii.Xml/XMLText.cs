using System;
using Yaapii.Atoms;
using Yaapii.Atoms.Text;
using Yaapii.Xml.Error;

namespace Yaapii.Xml
{
    /// <summary>
    /// A string in a document, retrieved by xpath.
    /// </summary>
    public sealed class XMLText : IText
    {
        private readonly IScalar<string> xmlString;

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public XMLText(IXML xml, string xpath)
        {
            this.xmlString =
                new XMLString(xml, xpath);
        }

        public string AsString()
        {
            return this.xmlString.Value();
        }

        public bool Equals(IText other)
        {
            return this.xmlString.Value().Equals(other.AsString());
        }
    }
}
