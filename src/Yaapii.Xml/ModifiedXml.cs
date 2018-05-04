using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;
using Yaapii.Xml.Xambly;

namespace Yaapii.Xml
{
    /// <summary>
    /// Represents a by Xambly modified XML 
    /// <para> Is <see cref="IXML"/> </para>
    /// </summary>
    public sealed class ModifiedXml : IXML
    {
        private readonly StickyScalar<IXML> _modifiedXml;

        /// <summary> Initializes class by xml as <see cref="IText"/> and list of <see cref="IDirective"/> s </summary>
        /// <param name="xml"> Xml wich will be modified </param>
        /// <param name="xamblyDirectives"> Xambly directives to modify xml </param>
        public ModifiedXml(IText xml, IEnumerable<IDirective> xamblyDirectives)
            : this(xml, new Xambler(xamblyDirectives))
        { }

        /// <summary> Initializes class by xml as <see cref="string"/> and list of <see cref="IDirective"/> s </summary>
        /// <param name="xml"> Xml wich will be modified </param>
        /// <param name="xamblyDirectives"> Xambly directives to modify xml </param>
        public ModifiedXml(string xml, IEnumerable<IDirective> xamblyDirectives)
            : this(xml, new Xambler(xamblyDirectives))
        { }

        /// <summary> Initializes class by xml as <see cref="IXML"/> and list of <see cref="IDirective"/> s </summary>
        /// <param name="xml"> Xml wich will be modified </param>
        /// <param name="xamblyDirectives"> Xambly directives to modify xml </param>
        public ModifiedXml(IXML xml, IEnumerable<IDirective> xamblyDirectives)
            : this(xml, new Xambler(xamblyDirectives))
        { }

        /// <summary> Initializes class by xml as <see cref="XNode"/> and list of <see cref="IDirective"/> s </summary>
        /// <param name="node"> Xml wich will be modified </param>
        /// <param name="xamblyDirectives"> Xambly directives to modify xml </param>
        public ModifiedXml(XNode node, IEnumerable<IDirective> xamblyDirectives)
            : this(node, new Xambler(xamblyDirectives))
        { }

        /// <summary> Initializes class by xml as <see cref="XmlNode"/> and list of <see cref="IDirective"/> s </summary>
        /// <param name="node"> Xml wich will be modified </param>
        /// <param name="xamblyDirectives"> Xambly directives to modify xml </param>
        public ModifiedXml(XmlNode node, IEnumerable<IDirective> xamblyDirectives)
            : this(node, new Xambler(xamblyDirectives))
        { }

        /// <summary> Initializes class by xml as <see cref="IText"/> and <see cref="Xambler"/> </summary>
        /// <param name="xml"> Xml wich will be modified </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public ModifiedXml(IText xml, Xambler xambler)
            : this(xml.AsString(), xambler)
        { }

        /// <summary> Initializes class by xml as <see cref="string"/> and <see cref="Xambler"/> </summary>
        /// <param name="xml"> Xml wich will be modified </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public ModifiedXml(string xml, Xambler xambler)
            : this(
                 new ScalarOf<XmlNode>(() =>
                 {
                     var result = new XmlDocument();
                     result.LoadXml(xml);

                     return result.FirstChild;
                 }),
                 xambler)
        { }

        /// <summary> Initializes class by xml as <see cref="IXML"/> and <see cref="Xambler"/> </summary>
        /// <param name="xml"> Xml wich will be modified </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public ModifiedXml(IXML xml, Xambler xambler)
            : this(xml.Node(), xambler)
        { }

        /// <summary> Initializes class by xml as <see cref="XNode"/> and <see cref="Xambler"/> </summary>
        /// <param name="node"> Xml wich will be modified </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public ModifiedXml(XNode node, Xambler xambler)
            : this(node.ToString(), xambler)
        { }

        /// <summary> Initializes class by xml as <see cref="XmlNode"/> and <see cref="Xambler"/> </summary>
        /// <param name="node"> Xml wich will be modified </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public ModifiedXml(XmlNode node, Xambler xambler)
            : this(new ScalarOf<XmlNode>(node), xambler)
        { }

        /// <summary> Initializes class by xml as <see cref="XmlNode"/> and <see cref="Xambler"/> </summary>
        /// <param name="node"> Xml wich will be modified </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public ModifiedXml(IScalar<XmlNode> node, Xambler xambler)
            : this(new StickyScalar<IXML>(() => new XMLQuery(xambler.Apply(node.Value()).OuterXml)))
        { }

        /// <summary> Primary private Ctor </summary>
        /// <param name="modifiedXml"> Modified xml </param>
        private ModifiedXml(StickyScalar<IXML> modifiedXml)
        {
            _modifiedXml = modifiedXml;
        }

        /// <summary> Current xml as <see cref="XNode"/> </summary>
        /// <returns> Current xml as <see cref="XNode"/> </returns>
        public XNode Node()
        {
            return _modifiedXml.Value().Node();
        }

        /// <summary> Nodes found by given XPath expression. </summary>
        /// <param name="query"> The XPath query </param>
        /// <returns> Collection of nodes as <see cref="IXML"/> </returns>
        public IList<IXML> Nodes(string query)
        {
            return _modifiedXml.Value().Nodes(query);
        }

        /// <summary> Find and return text elements or attributes matched by XPath address. </summary>
        /// <param name="query"> The XPath query </param>
        /// <returns> The list of string values(texts) or single function result </returns>
        public IList<string> Values(string query)
        {
            return _modifiedXml.Value().Values(query);
        }

        /// <summary> Registers a new namespace to this xml. You get back a XML with the new namespace. </summary>
        /// <param name="prefix"> namespace prefix </param>
        /// <param name="uri"> namespace uri </param>
        /// <returns> xml with the namespace registered </returns>
        public IXML WithNamespace(string prefix, object uri)
        {
            return _modifiedXml.Value().WithNamespace(prefix, uri);
        }
    }
}