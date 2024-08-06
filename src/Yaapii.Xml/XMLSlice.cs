// MIT License
//
// Copyright(c) 2022 ICARUS Consulting GmbH
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    /// An xml object which will execute the slice operation once its method is invoked.
    /// </summary>
    public sealed class XMLSlice : IXML
    {
        private readonly XMLCursor cursor;
        private readonly IScalar<IXmlNamespaceResolver> context;

        //// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(IEnumerable<IDirective> patch) : this(
            new Xambler(patch)
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Xambler xambler) : this(
            new TextOf(
                new ScalarOf<string>(() => xambler.Xml())
            )
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(XNode node) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(new XPathContext())
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Stream stream) : this(
            stream,
            Encoding.Default
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Stream stream, Encoding encoding) : this(
            new TextOf(
                new InputOf(stream),
                encoding
            )
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Url url) : this(
            url,
            Encoding.Default
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Url url, Encoding encoding) : this(
            new InputOf(url),
            encoding
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Uri file) : this(
            file,
            Encoding.Default
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(Uri file, Encoding encoding) : this(
            new InputOf(file),
            encoding
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(IInput input) : this(
            input,
            Encoding.Default
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(IInput input, Encoding encoding) : this(
            new TextOf(input, encoding)
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(string text) : this(
            new TextOf(text)
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(IText text) : this(
            new ScalarOf<XNode>(() =>
            {
                try
                {
                    return XDocument.Parse(text.AsString(), LoadOptions.PreserveWhitespace);
                }
                catch (Exception ex)
                {
                    throw
                        new XmlException(
                            $"Cannot parse xml: {ex.Message}{Environment.NewLine}" +
                            $"XML Content: '{text.AsString()}'",
                            ex
                        );
                }
            }),
            new ScalarOf<IXmlNamespaceResolver>(
                new XPathContext()
            )
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(XNode node, IXmlNamespaceResolver context) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(context)
        )
        { }

        /// <summary>
        /// An xml object which will execute the slice operation once its method is invoked.
        /// </summary>
        public XMLSlice(IScalar<XNode> node, IScalar<IXmlNamespaceResolver> context)
        {
            this.cursor = new XMLCursor(node, context);
            this.context = context;
        }

        /// <summary>
        /// Retrieve DOM node, represented by an XNode
        /// </summary>
        public XNode AsNode()
        {
            return this.cursor.AsNode();
        }

        /// <summary>
        /// Retrieve DOM nodes from the XML response that has been sliced.
        /// <para>An <see cref="ArgumentException"/> is thrown if the parameter
        /// passed is not a valid XPath expression.</para>
        /// </summary>
        public IList<IXML> Nodes(string xpath)
        {
            return
                new Atoms.List.Mapped<IXML, IXML>(node =>
                    new XMLSlice(
                        node.AsNode(),
                        this.context.Value()
                    ),
                    this.cursor.Nodes(xpath)
                );
        }

        /// <summary>
        /// Find and return text elements or attributes matched by XPath address.
        ///
        /// <para>The XPath query should point to text elements or attributes in the
        /// XML document. If any nodes of different types (elements, comments, etc.)
        /// are found in result node list -
        /// a <see cref="Exception"/> will be thrown.
        /// </para>
        /// <para>Alternatively, the XPath query can be a function or expression that
        /// returns a single value instead of pointing to a set of nodes. In this
        /// case, the result will be a List containing a single String, the content
        /// of which is the result of the evaluation. If the expression result is not
        /// a String, it will be converted to a String representation and returned as
        /// such. For example, a document containing three &lt;a&gt; elements,
        /// the input query "count(//a)", will return a singleton List with a single
        /// string value "3".
        /// </para>
        /// </summary>
        public IList<string> Values(string xpath)
        {
            return this.cursor.Values(xpath);
        }

        /// <summary>
        /// Register additional namespace prefix for XPath.
        /// <para>For example:
        /// <code>
        /// String name = new XMLCursor("...") / new XMLSlice("...")
        ///   .WithNamespace("ns1", "http://example.com")
        ///   .WithNamespace("foo", "http://example.com/foo")
        ///   .Xpath("/ns1:root/foo:name/text()")
        ///   .Item(0);
        /// </code>
        /// </para>
        /// <para>A number of standard namespaces are registered by default in
        /// instances of XML. Their
        /// full list is in {@link XMLDocument#XMLDocument(String)}.
        /// </para>
        /// <para>If a namespace prefix is already registered an
        /// <see cref="ArgumentException"/> will be thrown.
        /// </para>
        /// </summary>
        public IXML WithNamespace(string prefix, object uri)
        {
            return
                new XMLSlice(
                    this.cursor.AsNode(),
                    new XPathContext(
                        this.context.Value().GetNamespacesInScope(XmlNamespaceScope.All),
                        prefix,
                        uri.ToString()
                    )
                );
        }
    }
}
