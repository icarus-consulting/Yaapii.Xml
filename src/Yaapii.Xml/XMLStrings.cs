using Yaapii.Atoms.Enumerable;

namespace Yaapii.Xml
{
    /// <summary>
    /// Strings in a document, retrieved by xpath.
    /// </summary>
    public sealed class XMLStrings : EnumerableEnvelope<string>
    {
        /// <summary>
        /// Strings in a document, retrieved by xpath.
        /// </summary>
        public XMLStrings(string xpath, IXML xml) : base(() =>
            {
                return xml.Values(xpath);
            }
        )
        { }
    }
}
