using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Yaapii.Xml
{
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
        ///</para>
        /// <para>This is a convenient method, which is used (according to our
        /// experience) in 95% of all cases. Usually you don't need to get anything
        /// else but a text value of some node or an attribute. And in most cases
        /// you are interested to get just the first value
        /// (use <code>xpath(..).get(0)</code>). But when/if you need to get more than
        /// just a plain text - use {@link #nodes(String)}.
        ///</para>
        /// <para>The <see cref="IList{string}"/> returned will throw <see cref="IndexOutOfRangeException"/>
        /// if you try to access a node which wasn't found by this XPath query.
        /// </para>
        /// <para>An IllegalArgumentException} is thrown if the parameter
        /// passed is not a valid XPath expression.
        /// </para>
        /// </summary>
        /// <param name="query">The XPath query</param>
        /// <returns>The list of string values(texts) or single function result</returns>
        IList<String> Values(String query);

        /// <summary>
        /// 
        /// Retrieve DOM nodes from the XML response.
        ///
        /// <para>The <see cref="IList{IXML}"/>
        /// returned will throw {@link IndexOutOfBoundsException}
        /// if you try to access a node which wasn't found by this XPath query.
        /// </para>
        /// <para>An <see cref="ArgumentException"/> is thrown if the parameter
        /// passed is not a valid XPath expression.
        ///
        /// </summary>
        /// <param name="query">The XPath query</param>
        /// <returns>Collection of DOM nodes</returns>
        IList<IXML> Nodes(String query);

        /// <summary>
        ///
        /// Register additional namespace prefix for XPath.
        ///
        /// <para>For example:
        ///
        /// <code>
        /// String name = new XMLDocument("...")
        ///   .WithNamespace("ns1", "http://example.com")
        ///   .WithNamespace("foo", "http://example.com/foo")
        ///   .Xpath("/ns1:root/foo:name/text()")
        ///   .Item(0);
        /// </code>
        /// </para>
        ///
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
        XNode Node();

        //IXML Merged(NamespaceContext context);
    }
}
