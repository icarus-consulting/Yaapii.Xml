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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary> 
    /// A XML Cursor, which can be set to different nodes via XPath queries. Type is <see cref="IXML"/>.
    /// </summary>
    public sealed class FastXMLCursor : IXML
    {
        private readonly IScalar<XPathDocument> document;
        private readonly IScalar<XPathNavigator> navigator;
        private readonly IScalar<XNode> node;

        /// <summary> XMLCursor from <see cref="IInput"/>. </summary>
        /// <param name="input"> XNode to make XML from </param>
        public FastXMLCursor(IInput input, Encoding encoding) : this(
            new TextOf(input, encoding)
        )
        { }

        public FastXMLCursor(IText xml) : this(new InputOf(xml))
        { }

        public FastXMLCursor(string xml) : this(new InputOf(xml))
        { }

        public FastXMLCursor(XNode node) : this(new InputOf(node.ToString()))
        { }

        public FastXMLCursor(IInput xml) : this(
            new ScalarOf<XPathDocument>(() => new XPathDocument(xml.Stream()))
        )
        { }

        private FastXMLCursor(IScalar<XPathDocument> doc) : this(
            doc, 
            new ScalarOf<XPathNavigator>(() => doc.Value().CreateNavigator())
        )
        { }

        private FastXMLCursor(IScalar<XPathDocument> doc, XPathNavigator nav) : this(
            doc, 
            new ScalarOf<XPathNavigator>(nav), 
            new ScalarOf<XNode>(nav.UnderlyingObject as XNode)
        )
        { }

        private FastXMLCursor(IScalar<XPathDocument> doc, IScalar<XPathNavigator> nav) : this(
            doc, 
            nav,
            new ScalarOf<XNode>(() => nav.Value().UnderlyingObject as XNode)
        )
        { }

        private FastXMLCursor(IScalar<XPathDocument> doc, XPathNavigator nav, XNode node) : this(
            doc,
            new ScalarOf<XPathNavigator>(nav),
            node
        )
        { }

        private FastXMLCursor(IScalar<XPathDocument> doc, IScalar<XPathNavigator> nav, XNode node) : this(
            doc,
            nav,
            new ScalarOf<XNode>(node)
        )
        { }

        private FastXMLCursor(IScalar<XPathDocument> doc, IScalar<XPathNavigator> nav, IScalar<XNode> node)
        {
            this.document = new Sticky<XPathDocument>(doc);
            this.navigator = new Sticky<XPathNavigator>(nav);
            this.node = new Sticky<XNode>(node);
        }

        /// <summary> The xml formatted as string. </summary>
        /// <returns> xml as string </returns>
        public override sealed string ToString()
        {
            return this.navigator.Value().OuterXml;
        }

        /// <summary> The xml as XNode. </summary>
        /// <returns></returns>
        public XNode AsNode()
        {
            return XDocument.Parse(this.navigator.Value().OuterXml);
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
                var iterator = this.navigator.Value().Select(xpath);
                var result = new List<IXML>();
                while (iterator.MoveNext())
                {
                    if (iterator.Current.NodeType != XPathNodeType.Element && iterator.Current.NodeType != XPathNodeType.Root)
                    {
                        throw new XPathException("Only text() nodes or attributes are retrievable with xpath().");
                    }
                    result.Add(
                        new FastXMLCursor(
                            this.document,
                            iterator.Current.Clone(),
                            iterator.Current.UnderlyingObject as XNode
                        )
                    );
                }
                return result;
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
            var result = new List<string>();
            var selection = this.navigator.Value().Evaluate(xpath);
            if(selection is XPathNodeIterator)
            {
                var iterator = selection as XPathNodeIterator;
                while (iterator.MoveNext())
                {
                    if (iterator.Current.NodeType != XPathNodeType.Text && iterator.Current.NodeType != XPathNodeType.Attribute)
                    {
                        throw
                            new ArgumentException(
                                new Formatted(
                                    "Only text() nodes, CData sections or attributes are retrievable with xpath(). Your path: '{0}'.", xpath
                                ).AsString()
                            );
                    }
                    result.Add(iterator.Current.Value);
                }
            }
            else
            {
                if(selection != null)
                {
                    result.Add(selection.ToString());
                }
            }
            return result;
        }

        public string Value(string xpath, string def)
        {
            string result = def;
            var selection = this.navigator.Value().SelectSingleNode(xpath);
            if (selection != null //C# uses null, so we have to do it here
                && selection.NodeType != XPathNodeType.Text 
                && selection.NodeType != XPathNodeType.Attribute
            )
            {
                throw
                    new ArgumentException(
                        new Formatted(
                            "Only text() nodes, CData sections or attributes are retrievable with xpath(). Your path: '{0}'.", xpath
                        ).AsString()
                    );
            }
            return selection.Value;
        }

        /// <summary> Exact equality test, regarding whitespaces and blanks. </summary>
        /// <param name="obj"> to compare to </param>
        /// <returns> true if equal </returns>
        public override bool Equals(object obj)
        {
            return true;
        }

        /// <summary> Hashcode for this object. </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.navigator.Value().GetHashCode();
        }
    }
}
