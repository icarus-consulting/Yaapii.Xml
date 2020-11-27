using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Text;
using Yaapii.Xml.XPath;

namespace Yaapii.Xml
{
    /// <summary>
    /// <para>A enumerable of values, found by the given xpath.</para>
    /// <para>Object is IEnumerable&lt;string&gt;</para>
    /// </summary>
    public class XpValues : IEnumerable<string>
    {
        /// <summary>
        /// xpath
        /// </summary>
        private readonly IText xpath;

        /// <summary>
        /// document
        /// </summary>
        private readonly XmlNode dom;

        /// <summary>
        /// Extracted values from dom using given xpath
        /// </summary>
        /// <param name="xpath">Selection xpath</param>
        /// <param name="dom">document</param>
        public XpValues(string xpath, XmlNode dom) : this(new TextOf(xpath), dom)
        { }

        /// <summary>
        /// Extracted values from dom using given xpath
        /// </summary>
        /// <param name="xpath">Selection xpath</param>
        /// <param name="dom">document</param>
        public XpValues(IText xpath, XmlNode dom)
        {
            this.xpath = xpath;
            this.dom = dom;
        }

        /// <summary>
        /// Enumerator of values
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return
                new Mapped<XmlNode, string>(
                    node => node.InnerText,
                    new XpNodes(xpath, dom)
                ).GetEnumerator();
        }

        /// <summary>
        /// Enumerator of values
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
