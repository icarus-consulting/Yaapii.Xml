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
using System.Xml.Linq;

namespace Yaapii.Xml
{
    /// <summary>
    /// IXML provides acess to a XML Document via XPath queries.
    /// </summary>
    public interface IXML
    {
        /// <summary>
        ///
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
        /// <param name="query">The XPath query</param>
        /// <returns>The list of string values(texts) or single function result</returns>
        IList<String> Values(String query);

        /// <summary>
        /// Retrieve DOM nodes from the XML response.
        /// <para>An <see cref="ArgumentException"/> is thrown if the parameter
        /// passed is not a valid XPath expression.</para>
        ///
        /// </summary>
        /// <param name="query">The XPath query</param>
        /// <returns>Collection of DOM nodes</returns>
        IList<IXML> Nodes(String query);

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
        ///
        /// </summary>
        /// <param name="prefix">prefix The XPath prefix to register</param>
        /// <param name="uri">uri Namespace URI</param>
        /// <returns>A new XML document, with this additional namespace registered</returns>
        IXML WithNamespace(String prefix, Object uri);

        /// <summary>
        /// Retrieve DOM node, represented by an XNode
        /// </summary>
        /// <returns>DOM node</returns>
        XNode AsNode();
    }
}
