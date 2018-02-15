using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using System.Xml.XPath;
using Yaapii.Atoms.List;
using System.Collections;
using System.Diagnostics;
using System.Xml;

namespace Yaapii.Xml
{
    public sealed class XMLQuery : IXML
    {
        private readonly IScalar<string> _xml;

        /// <summary>
        /// Is it a leaf node (= not a document node)?
        /// </summary>
        private readonly IScalar<bool> _isLeaf;

        private readonly IScalar<XNode> _cache;

        private readonly IScalar<IXmlNamespaceResolver> _context;

        /// <summary>
        /// XML from a XNode.
        /// </summary>
        /// <param name="node">XNode to make XML from</param>
        public XMLQuery(XNode node) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(new XPathContext()),
            new StickyScalar<bool>(() => node.NodeType != System.Xml.XmlNodeType.Document))
        { }

        /// <summary>
        /// XML from a stream.
        /// </summary>
        /// <param name="node">stream with xml text</param>
        public XMLQuery(Stream stream) : this(
            new TextOf(
                new InputOf(stream)))
        { }

        /// <summary>
        /// XML from a url.
        /// </summary>
        /// <param name="node">url to ger xml text from</param>
        public XMLQuery(Url url) : this(
            new InputOf(url))
        { }

        /// <summary>
        /// XML from a file.
        /// </summary>
        /// <param name="node">file to get xml text from</param>
        public XMLQuery(Uri file) : this(
            new InputOf(file))
        { }

        /// <summary>
        /// XML from <see cref="IInput"/>.
        /// </summary>
        /// <param name="node">XNode to make XML from</param>
        public XMLQuery(IInput input) : this(
            new TextOf(input))
        { }

        /// <summary>
        /// XML from a string.
        /// </summary>
        /// <param name="text">xml as string</param>
        public XMLQuery(String text) : this(
            new TextOf(text))
        { }

        /// <summary>
        /// XML from <see cref="IText"/>
        /// </summary>
        /// <param name="text">xml as text</param>
        public XMLQuery(IText text) : this(
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
            ),
            new ScalarOf<bool>(false)
        )
        { }

        /// <summary>
        /// XML from node and context, plus info whether it is a document or a node.
        /// </summary>
        /// <param name="node">xml as XNode</param>
        /// <param name="context">context information about namespaces in the xml</param>
        /// <param name="leaf">is it a document or a node</param>
        public XMLQuery(XNode node, IXmlNamespaceResolver context, bool leaf) : this(
            new ScalarOf<XNode>(node),
            new ScalarOf<IXmlNamespaceResolver>(context),
            new ScalarOf<bool>(leaf)
        )
        { }

        private XMLQuery(IScalar<XNode> node, IScalar<IXmlNamespaceResolver> context, IScalar<bool> leaf)
        {
            this._xml = new StickyScalar<string>(
                () =>
                {
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings xws = new XmlWriterSettings();
                    xws.OmitXmlDeclaration = node.Value().NodeType != XmlNodeType.Document;
                    xws.Indent = false;
                    using (XmlWriter xw = XmlWriter.Create(sb, xws))
                    {
                        node.Value().Document.WriteTo(xw);
                    }
                    return sb.ToString();
                });
            this._isLeaf = new StickyScalar<bool>(leaf);
            this._cache = new StickyScalar<XNode>(node);
            this._context = new StickyScalar<IXmlNamespaceResolver>(context);
        }

        /// <summary>
        /// The xml formatted as string.
        /// </summary>
        /// <returns>xml as string</returns>
        public override sealed string ToString()
        {
            return this._xml.Value();
        }

        /// <summary>
        /// The xml as XNode.
        /// </summary>
        /// <returns></returns>
        public XNode Node()
        {
            var casted = this._cache.Value();
            XNode answer = null;

            if (casted.NodeType == XmlNodeType.Document)
            {
                answer = casted as XNode;
            }
            else
            {
                var doc = new XDocument();
                doc.AddFirst(casted); //Correct? Below is the original implementation
                answer = doc.Root as XNode;
                //answer = XMLDocument.createImportedNode(casted);
            }
            return answer;
        }

        /// <summary>
        /// 
        /// Retrieve DOM nodes from the XML response.
        ///
        /// <para>The <see cref="IList{IXML}"/>
        /// returned will throw <see cref="IndexOutOfRangeException"/>
        /// if you try to access a node which wasn't found by this XPath query.
        /// </para>
        /// <para>An <see cref="ArgumentException"/> is thrown if the parameter
        /// passed is not a valid XPath expression.
        ///
        /// </summary>
        /// <param name="query">The XPath query</param>
        /// <returns>Collection of DOM nodes</returns>
        public IList<IXML> Nodes(string xpQuery)
        {
            // csa_180201: The InvalidOperationException is never fired here.
            // FetchNodes returns a ListOf<XElement>(...) which is created by the result (IEnumerable<XElement>) of XPathSelectElements(...).
            // Hence the execpion will be fired in the colling code which evaluates the Enumerator of this Nodes(...)
            //try
            //{
            return new Mapped<XElement, IXML>(
                elem => new XMLQuery(elem),
                FetchedNodes(xpQuery));
            //}
            //catch (System.InvalidOperationException ex)
            //{
            //    throw new InvalidOperationException("Could not evaluate xpath query. Did you try to read values (attributes) instead of nodes? Use method .Nodes(string xpath).", ex);
            //}
        }

        /// <summary>
        /// <para>Registers a new namespace to this xml.</para>
        /// <para>You get back a XML with the new namespace - this one stays like it is.</para>
        /// </summary>
        /// <param name="prefix">namespace prefix</param>
        /// <param name="uri">namespace uri</param>
        /// <returns>xml with the namespace registered</returns>
        public IXML WithNamespace(string prefix, object uri)
        {
            return
                new XMLQuery(
                    this._cache.Value(),
                    new XPathContext(
                        this._context.Value().GetNamespacesInScope(XmlNamespaceScope.All),
                        prefix,
                        uri.ToString()
                    ),
                    this._isLeaf.Value()
                );
        }

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
        /// <para>This is a convenient method, which is used (according to our
        /// experience) in 95% of all cases. Usually you don't need to get anything
        /// else but a text value of some node or an attribute. And in most cases
        /// you are interested to get just the first value
        /// (use <code>xpath(..).get(0)</code>). But when/if you need to get more than
        /// just a plain text - use {@link #nodes(String)}.
        /// </para>
        /// <para>The <see cref="IList{string}"/> returned will throw <see cref="IndexOutOfRangeException"/>
        /// if you try to access a node which wasn't found by this XPath query.
        /// </para>
        /// <para>An IllegalArgumentException} is thrown if the parameter
        /// passed is not a valid XPath expression.
        /// </para>
        /// </summary>
        /// <param name="query">The XPath query</param>
        /// <returns>The list of string values(texts) or single function result</returns>
        public IList<string> Values(string xpath)
        {
            IList<string> items;
            try
            {
                items = this.FetchedValues(xpath);
            }
            // csa_180201: This exception can never be rise here. It is allready catched and transfromend to an ArgumentExecption in the FetchedValues() method.
            //catch (XPathException exp)
            //{
            //    throw new ArgumentException(
            //        new FormattedText(
            //            "Invalid XPath query '{0}': {1}",
            //            xpath,
            //            exp.Message
            //        ).AsString(),
            //        exp);
            //}
            catch (System.InvalidOperationException ex)
            {
                throw new InvalidOperationException("Could not perform xpath query. Did you try to read nodes instead of values (attributes) ? Use method .Values(string xpath).", ex);
            }

            return items;

            //return new ListWrapper<string>(items, this._cache.Value().ToString(), xpath);
        }

        /// <summary>
        /// Nodes fetched using the xpath query.
        /// </summary>
        /// <param name="xpath">xpath query</param>
        /// <returns>list of elements matching the xpath</returns>
        private IList<XElement> FetchedNodes(string xpath)
        {
            try
            {
                return
                    new ListOf<XElement>(
                        this._cache.Value().XPathSelectElements(xpath, this._context.Value())
                    );
            }
            catch (XPathException ex)
            {
                throw
                    new ArgumentException(
                        new FormattedText(
                            "Invalid XPath expression '{0}': {1}",
                            xpath,
                            ex.Message
                        ).AsString(),
                        ex
                    );
            }
        }

        /// <summary>
        /// Values fetched using the xpath query.
        /// </summary>
        /// <param name="xpath">xpath query</param>
        /// <returns>list of values</returns>
        private IList<string> FetchedValues(string xpath)
        {
            IList<string> result = new List<string>();
            try
            {
                //var eval = this._cache.Value().XPathEvaluate(xpath, this._context.Value());
                //@Todo: this fixes the issue #430 but the behavior is strange. The method should also work in the chache variable
                var eval = this.Node().XPathEvaluate(xpath, this._context.Value());

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
                            new FormattedText("Unexpected xpath result type '{0}'", eval.GetType().Name).AsString());
                }
            }
            catch (ArgumentException aex)
            {
                throw
                    new ArgumentException(
                        new FormattedText(
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
                        new FormattedText(
                            "Invalid XPath expression '{0}': {1}",
                            xpath,
                            ex.Message
                        ).AsString(),
                        ex
                    );
            }
            return result;
        }

        /// <summary>
        /// Extracted value from XObject.
        /// </summary>
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
                        new FormattedText(
                            "Unexpected node type {0}",
                            xObject.GetType().Name
                        ).AsString()
                    );
            }

            return result;
        }

        /// <summary>
        /// Exact equality test, regarding whitespaces and blanks.
        /// </summary>
        /// <param name="obj">to compare to</param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is XMLQuery)) return false;

            var left = this._xml.Value().ToString();
            var right = (obj as XMLQuery).ToString();

            return left.Equals(right);
        }

        public override int GetHashCode()
        {
            return this._xml.GetHashCode();
        }
    }
}