using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Yaapii.Atoms;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml.XPath
{
    /// <summary>
    /// <para>A enumerable of XmlNodes, selected by the given Xpath.</para>
    /// <para>Object is IEnumerable&lt;XmlNode&gt;</para>
    /// </summary>
    public class XpNodes : IEnumerable<XmlNode>
    {
        private readonly IText xpath;
        private readonly XmlNode dom;

        /// <summary>
        /// Extracted nodes from dom using xpath
        /// </summary>
        /// <param name="xpath">xpath</param>
        /// <param name="dom">document</param>
        public XpNodes(string xpath, XmlNode dom) : this(new TextOf(xpath), dom)
        { }

        /// <summary>
        /// Extracted nodes from dom using xpath
        /// </summary>
        /// <param name="xpath">xpath</param>
        /// <param name="dom">document</param>
        public XpNodes(IText xpath, XmlNode dom)
        {
            this.xpath = xpath;
            this.dom = dom;
        }

        /// <summary>
        /// Enumerator for nodes
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<XmlNode> GetEnumerator()
        {
            var lst = dom.SelectNodes(xpath.AsString());

            var result = new List<XmlNode>();
            foreach (var xmlNode in lst)
            {
                result.Add(xmlNode as XmlNode); //Casting necessary from C#
            }
            return result.GetEnumerator();
        }

        /// <summary>
        /// Enumerator for nodes
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
