using System.Collections.Generic;
using System.Xml.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;
using Yaapii.Xambly;

namespace Yaapii.Xml
{
    /// <summary>
    /// A XML modified by Xambly.
    /// <para> Is <see cref="IXML"/> </para>
    /// </summary>
    public sealed class XMLPatched : IXML
    {
        private readonly StickyScalar<IXML> xml;

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="patch"> Xambly directives to modify xml </param>
        public XMLPatched(IText xml, IEnumerable<IDirective> patch) : this(
            xml,
            new Xambler(patch)
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="patch"> Xambly directives to modify xml </param>
        public XMLPatched(string xml, IEnumerable<IDirective> patch) : this(
            xml,
            new Xambler(patch)
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="patch"> Xambly directives to modify xml </param>
        public XMLPatched(IXML xml, IEnumerable<IDirective> patch) : this(
            xml,
            new Xambler(patch)
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="node"> XmlNode to patch </param>
        /// <param name="patch"> Xambly directives to modify xml </param>
        public XMLPatched(XNode node, IEnumerable<IDirective> patch) : this(
            node,
            new Xambler(patch)
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="xambler"> Patching Xambler </param>
        public XMLPatched(IText xml, Xambler xambler) : this(
            new ScalarOf<string>(() => xml.AsString()),
            xambler
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="xambler"> Patching Xambler </param>
        public XMLPatched(string xml, Xambler xambler) : this(
            new ScalarOf<string>(xml),
            xambler
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="xambler"> Patching Xambler </param>
        public XMLPatched(IScalar<string> xml, Xambler xambler) : this(
            new StickyScalar<XNode>(() =>
                {
                    return XDocument.Parse(xml.Value());
                }
            ),
            xambler
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="xambler"> Patching Xambler </param>
        public XMLPatched(IXML xml, Xambler xambler) : this(
            new StickyScalar<XNode>(
                () => xml.AsNode()
            ),
            xambler
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="xambler"> Patching Xambler </param>
        public XMLPatched(XNode xml, Xambler xambler) : this(
            new ScalarOf<XNode>(xml),
            xambler
        )
        { }

        /// <summary> Patched XML.</summary>
        /// <param name="xml"> XML to patch </param>
        /// <param name="xambler"> Xambler to modify xml </param>
        public XMLPatched(IScalar<XNode> xml, Xambler xambler) : this(
            new StickyScalar<IXML>(() =>
                new XMLCursor(xambler.Apply(xml.Value()))
            )
        )
        { }

        private XMLPatched(StickyScalar<IXML> modifiedXml)
        {
            xml = modifiedXml;
        }

        public XNode AsNode()
        {
            return xml.Value().AsNode();
        }

        public IList<IXML> Nodes(string query)
        {
            return xml.Value().Nodes(query);
        }

        public IList<string> Values(string query)
        {
            return xml.Value().Values(query);
        }

        public IXML WithNamespace(string prefix, object uri)
        {
            return xml.Value().WithNamespace(prefix, uri);
        }

        public override string ToString()
        {
            return xml.Value().ToString();
        }
    }
}