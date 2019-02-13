// MIT License
//
// Copyright(c) 2019 ICARUS Consulting GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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