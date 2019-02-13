using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary>
    /// Texts, extracted from a xml with xpath.
    /// </summary>
    public sealed class XMLTexts : EnumerableEnvelope<IText>
    {
        /// <summary>
        /// Texts, extracted from a xml with xpath.
        /// </summary>
        public XMLTexts(IXML xml, string xpath) : base(() =>
            new Mapped<string, IText>(
                str => new TextOf(str),
                xml.Values(xpath)
            )
        )
        { }
    }
}
