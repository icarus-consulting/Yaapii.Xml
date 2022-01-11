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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace Yaapii.Xml
{
    /// <summary> 
    /// A XML Cursor, which can be set to different nodes via XPath queries. Type is <see cref="IXML"/>.
    /// </summary>
    public sealed class XMLCursor : IXML
    {
        private readonly IScalar<string> xml;
        private readonly IScalar<XNode> cache;
        private readonly IScalar<IXmlNamespaceResolver> context;
        private readonly Func<XNode, string> stringTransform;

        /// <summary> 
        /// XMLCursor from Xambly Directives. 
        /// </summary>
        /// <param name="patch">Xambly patch</param>
        public XMLCursor(IEnumerable<IDirective> patch) : this(
            new Xambler(patch)
        )
        { }

        /// <summary> XMLCursor from a Xambler. </summary>
        /// <param name="xambler"> Xambler to make Xml from </param>
        public XMLCursor(Xambler xambler) : this(
            new TextOf(
                new ScalarOf<string>(() => xambler.Xml())
            )
        )
        { }

        /// <summary> XMLCursor from a XNode. </summary>
        /// <param name="node"> XNode to make XML from </param>
        public XMLCursor(XNode node) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(new XPathContext())
        )
        { }

        /// <summary> XMLCursor from a stream. </summary>
        /// <param name="stream"> stream with xml text </param>
        public XMLCursor(Stream stream) : this(stream, Encoding.Default)
        { }

        /// <summary> XMLCursor from a stream. </summary>
        /// <param name="stream"> stream with xml text </param>
        public XMLCursor(Stream stream, Encoding encoding) : this(
            new TextOf(
                new InputOf(stream),
                encoding
            )
        )
        { }

        /// <summary> XMLCursor from a url. </summary>
        /// <param name="url"> url to get xml text from </param>
        public XMLCursor(Url url) : this(
            new InputOf(url))
        { }

        /// <summary> XMLCursor from a url. </summary>
        /// <param name="url"> url to get xml text from </param>
        public XMLCursor(Url url, Encoding encoding) : this(
            new InputOf(url), encoding
        )
        { }

        /// <summary> XMLCursor from a file. </summary>
        /// <param name="file"> file to get xml text from </param>
        public XMLCursor(Uri file) : this(file, Encoding.Default)
        { }

        /// <summary> XMLCursor from a file. </summary>
        /// <param name="file"> file to get xml text from </param>
        public XMLCursor(Uri file, Encoding encoding) : this(
            new InputOf(file),
            encoding
        )
        { }

        /// <summary> XMLCursor from <see cref="IInput"/>. </summary>
        /// <param name="input"> XNode to make XML from </param>
        public XMLCursor(IInput input) : this(input, Encoding.Default)
        { }

        /// <summary> XMLCursor from <see cref="IInput"/>. </summary>
        /// <param name="input"> XNode to make XML from </param>
        public XMLCursor(IInput input, Encoding encoding) : this(
            new TextOf(input, encoding)
        )
        { }

        /// <summary> XMLCursor from a string. </summary>
        /// <param name="text"> xml as string </param>
        public XMLCursor(String text) : this(
            new TextOf(text))
        { }

        /// <summary> XMLCursor from <see cref="IText"/> </summary>
        /// <param name="text"> xml as text </param>
        public XMLCursor(IText text) : this(
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
                            new Formatted("Cannot parse xml: {0}\r\nXML Content: '{1}'", ex.Message, text.AsString()).AsString(),
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
        public XMLCursor(XNode node, IXmlNamespaceResolver context) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(context)
        )
        { }

        internal XMLCursor(IScalar<XNode> node, IScalar<IXmlNamespaceResolver> context)
        {
            this.stringTransform = nd =>
            {
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = nd.NodeType != XmlNodeType.Document;
                xws.Indent = false;
                using (XmlWriter xw = XmlWriter.Create(sb, xws))
                {
                    nd.Document.WriteTo(xw);
                }
                return sb.ToString();
            };
            this.xml = new ScalarOf<string>(
                () =>
                {
                    return this.stringTransform.Invoke(node.Value());
                });
            this.cache = new ScalarOf<XNode>(node);
            this.context = new ScalarOf<IXmlNamespaceResolver>(context);
        }

        /// <summary> The xml formatted as string. </summary>
        /// <returns> xml as string </returns>
        public override sealed string ToString()
        {
            return this.stringTransform.Invoke(this.AsNode());
        }

        /// <summary> The xml as XNode. </summary>
        /// <returns></returns>
        public XNode AsNode()
        {
            var casted = this.cache.Value();
            XNode answer = null;

            if (casted.NodeType == XmlNodeType.Document)
            {
                answer = casted as XNode;
            }
            else
            {
                var doc = new XDocument();
                doc.AddFirst(casted);
                answer = doc.Root as XNode;
            }
            return answer;
        }

        /// <summary>
        /// Retrieve DOM nodes from the XML response. 
        /// <para>
        /// The <see cref="IList{IXML}"/> returned will throw <see cref="IndexOutOfRangeException"/> if you try to access a node
        /// which wasn't found by this XPath query.
        /// </para>
        /// <para> An <see cref="ArgumentException"/> is thrown if the parameter passed is not a valid XPath expression. </para>
        /// </summary>
        /// <param name="xpath"> The XPath query </param>
        /// <returns> Collection of DOM nodes </returns>
        public IList<IXML> Nodes(string xpath)
        {
            try
            {
                var el = this.cache.Value().XPathSelectElements(xpath, this.context.Value());
                var mapped =
                    new Mapped<XElement, IXML>(
                        elem => new XMLCursor(elem),
                        el
                    );
                return mapped;
            }
            catch (XPathException ex)
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Invalid XPath expression '{0}': {1}",
                            xpath,
                            ex.Message
                        ).AsString(),
                        ex
                    );
            }
        }

        /// <summary>
        /// <para> Registers a new namespace to this xml. </para>
        /// <para> You get back a XML with the new namespace - this one stays like it is. </para>
        /// </summary>
        /// <param name="prefix"> namespace prefix </param>
        /// <param name="uri"> namespace uri </param>
        /// <returns> xml with the namespace registered </returns>
        public IXML WithNamespace(string prefix, object uri)
        {
            return
                new XMLCursor(
                    this.cache.Value(),
                    new XPathContext(
                        this.context.Value().GetNamespacesInScope(XmlNamespaceScope.All),
                        prefix,
                        uri.ToString()
                    )
                );
        }

        /// <summary>
        /// Find and return text elements or attributes matched by XPath address. 
        /// <para>
        /// The XPath query should point to text elements or attributes in the XML document. If any nodes of different types
        /// (elements, comments, etc.) are found in result node list - a <see cref="Exception"/> will be thrown.
        /// </para>
        /// <para>
        /// Alternatively, the XPath query can be a function or expression that returns a single value instead of pointing to a
        /// set of nodes. In this case, the result will be a List containing a single String, the content of which is the result
        /// of the evaluation. If the expression result is not a String, it will be converted to a String representation and
        /// returned as such. For example, a document containing three &lt;a&gt; elements, the input query "count(//a)", will
        /// return a singleton List with a single string value "3".
        /// </para>
        /// <para>
        /// This is a convenient method, which is used (according to our
        /// experience) in 95% of all cases. Usually you don't need to get anything else but a text value of some node or an
        /// attribute. And in most cases you are interested to get just the first value (use
        /// <code>
        /// xpath(..).get(0)
        /// </code>
        /// ). But when/if you need to get more than just a plain text - use {@link #nodes(String)}. 
        /// </para>
        /// <para>
        /// The <see cref="IList"/> returned will throw <see cref="IndexOutOfRangeException"/> if you try to access a node which
        /// wasn't found by this XPath query.
        /// </para>
        /// <para> An IllegalArgumentException} is thrown if the parameter passed is not a valid XPath expression. </para>
        /// </summary>
        /// <param name="xpath"> The XPath query </param>
        /// <returns> The list of string values(texts) or single function result </returns>
        public IList<string> Values(string xpath)
        {
            IList<string> items;
            try
            {
                var result = new List<string>();
                var eval = this.cache.Value().XPathEvaluate(xpath, this.context.Value());

                if (eval is string || eval is double || eval is bool) //necessary because of XPathEvaluate implementation in C#
                {
                    result.Add(eval.ToString());
                }
                else if (eval is IEnumerable) //would be true if eval is a string, so must stay after string
                {
                    foreach (var obj in eval as IEnumerable)
                    {
                        if (obj is XObject) //need to extract the value...
                        {
                            result.Add(
                                ValueFrom(
                                    obj as XObject
                                )
                            );
                        }
                        else //can add the value itself because it already is a string
                        {
                            result.Add(obj.ToString());
                        }
                    }
                }
                else
                {
                    throw
                        new Exception(
                            new Formatted("Unexpected xpath result type '{0}'", eval.GetType().Name).AsString());
                }
                items = result;
            }
            catch (ArgumentException aex)
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Only text() nodes, CData sections or attributes are retrievable with xpath() '{0}': {1}",
                            xpath,
                            aex.Message).AsString(),
                        aex
                    );
            }
            catch (XPathException ex)
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Invalid XPath expression '{0}': {1}",
                            xpath,
                            ex.Message
                        ).AsString(),
                        ex
                    );
            }
            catch (System.InvalidOperationException ex)
            {
                throw new InvalidOperationException("Could not perform xpath query. Did you try to read nodes instead of values (attributes) ? Use method .Values(string xpath).", ex);
            }

            return items;
        }

        /// <summary> Extracted value from XObject. </summary>
        /// <param name="xObject"></param>
        /// <returns></returns>
        private string ValueFrom(XObject xObject)
        {
            string result = String.Empty;

            //Casting necessary, because LinQ-XML is designed like this by Microsoft
            if (xObject.NodeType == XmlNodeType.Attribute)
            {
                result = (xObject as XAttribute).Value;
            }
            else if (xObject.NodeType == XmlNodeType.CDATA)
            {
                result = (xObject as XCData).Value;
            }
            else if (xObject.NodeType == XmlNodeType.Text)
            {
                result = (xObject as XText).Value;
            }
            else
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Unexpected node type {0}",
                            xObject.GetType().Name
                        ).AsString()
                    );
            }

            return result;
        }

        /// <summary> Exact equality test, regarding whitespaces and blanks. </summary>
        /// <param name="obj"> to compare to </param>
        /// <returns> true if equal </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is XMLCursor)) return false;

            var left = this.xml.Value().ToString();
            var right = (obj as XMLCursor).ToString();

            return left.Equals(right);
        }

        /// <summary> Hashcode for this object. </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.xml.GetHashCode();
        }
    }
}
