using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace Yaapii.Xml
{
    /// <summary>
    /// A XML which will slice when you call Nodes(xpath).
    /// </summary>
    public sealed class XMLSlice : IXML
    {
        private readonly XMLCursor cursor;

        /// <summary> 
        /// XMLCursor from Xambly Directives. 
        /// </summary>
        /// <param name="patch">Xambly patch</param>
        public XMLSlice(IEnumerable<IDirective> patch) : this(
            new Xambler(patch)
        )
        { }

        /// <summary> XMLCursor from a Xambler. </summary>
        /// <param name="xambler"> Xambler to make Xml from </param>
        public XMLSlice(Xambler xambler) : this(
            new TextOf(
                new StickyScalar<string>(() => xambler.Xml())
            )
        )
        { }

        /// <summary> XMLCursor from a XNode. </summary>
        /// <param name="node"> XNode to make XML from </param>
        public XMLSlice(XNode node) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(new XPathContext())
        )
        { }

        /// <summary> XMLCursor from a stream. </summary>
        /// <param name="stream"> stream with xml text </param>
        public XMLSlice(Stream stream) : this(
            new TextOf(
                new InputOf(stream)))
        { }

        /// <summary> XMLCursor from a url. </summary>
        /// <param name="url"> url to get xml text from </param>
        public XMLSlice(Url url) : this(
            new InputOf(url))
        { }

        /// <summary> XMLCursor from a file. </summary>
        /// <param name="file"> file to get xml text from </param>
        public XMLSlice(Uri file) : this(
            new InputOf(file))
        { }

        /// <summary> XMLCursor from <see cref="IInput"/>. </summary>
        /// <param name="input"> XNode to make XML from </param>
        public XMLSlice(IInput input) : this(
            new TextOf(input))
        { }

        /// <summary> XMLCursor from a string. </summary>
        /// <param name="text"> xml as string </param>
        public XMLSlice(String text) : this(
            new TextOf(text))
        { }

        /// <summary> XMLCursor from <see cref="IText"/> </summary>
        /// <param name="text"> xml as text </param>
        public XMLSlice(IText text) : this(
            new StickyScalar<XNode>(() =>
            {
                try
                {
                    return XDocument.Parse(text.AsString(), LoadOptions.PreserveWhitespace);
                }
                catch (Exception ex)
                {
                    throw
                        new XmlException(
                            new FormattedText("Cannot parse xml: {0}\r\nXML Content: '{1}'", ex.Message, text.AsString()).AsString(),
                            ex
                        );
                }
            }),
            new ScalarOf<IXmlNamespaceResolver>(
                new XPathContext()
            )
        )
        { }

        /// <summary> XMLCursor from node and context</summary>
        /// <param name="node"> xml as XNode </param>
        /// <param name="context"> context information about namespaces in the xml </param>
        /// <param name="leaf"> is it a document or a node </param>
        public XMLSlice(XNode node, IXmlNamespaceResolver context) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(context)
        )
        { }

        public XMLSlice(IScalar<XNode> node, IScalar<IXmlNamespaceResolver> context)
        {
            this.cursor = new XMLCursor(node, context);
        }

        public XNode AsNode()
        {
            return this.cursor.AsNode();
        }

        public IList<IXML> Nodes(string xpath)
        {
            return
                new Atoms.List.Mapped<IXML, IXML>(
                    node => new XMLSlice(node.AsNode()),
                    this.cursor.Nodes(xpath)
                );
        }

        public IList<string> Values(string xpath)
        {
            return this.cursor.Values(xpath);
        }

        public IXML WithNamespace(string prefix, object uri)
        {
            return new XMLSlice(this.cursor.WithNamespace(prefix, uri).AsNode());
        }
    }
}
