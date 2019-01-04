using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml
{
    /// <summary>
    /// Default useful xml namespaces.
    /// </summary>
    public sealed class XPathContext : IXmlNamespaceResolver
    {
        private readonly IEnumerable<IScalar<IXmlNamespaceResolver>> contexts;
        private readonly IDictionary<string, string> map;

        private const string XML_NS_PREFIX = "xml";
        private const string XML_NS_URI = "http://www.w3.org/XML/1998/namespace";
        private const string XMLNS_ATTRIBUTE = "xmlns";
        private const string XMLNS_ATTRIBUTE_NS_URI = "http://www.w3.org/2000/xmlns/";
        private const string NULL_NS_URI = "";

        /// <summary>
        /// XpathContext with default namespaces:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>xhtml=http://www.w3.org/1999/xhtml </description>
        ///         </item>
        ///         <item>
        ///             <description>xs=http://www.w3.org/2001/XMLSchema </description>
        ///         </item>
        ///         <item>
        ///             <description>xsi=http://www.w3.org/2001/XMLSchema-instance </description>
        ///         </item>
        ///         <item>
        ///             <description>xsl=http://www.w3.org/1999/XSL/Transform </description>
        ///         </item>
        ///         <item>
        ///             <description>svg=http://www.w3.org/2000/svg </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public XPathContext() : this(
            new MapOf<string, string>(
                new KeyValuePair<string, string>("xhtml", "http://www.w3.org/1999/xhtml"),
                new KeyValuePair<string, string>("xs", "http://www.w3.org/2001/XMLSchema"),
                new KeyValuePair<string, string>("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new KeyValuePair<string, string>("xsl", "http://www.w3.org/1999/XSL/Transform"),
                new KeyValuePair<string, string>("svg", "http://www.w3.org/2000/svg")
            )
        )
        { }

        /// <summary>
        /// <para>A XpathContext built from the uris.
        /// Every uri will appear mapped with a numbered prefix ns&lt;index&gt;
        /// </para>
        /// <para>Example: a list of two items "http://uri-one", "http://uri-two" will
        /// result in:</para>
        /// <code>
        /// ns0=http://uri-one,
        /// ns1=http://uri-two
        /// </code>
        /// </summary>
        /// <param name="uris"></param>
        public XPathContext(params string[] uris) : this(
            new MapOf<string, string>(
                new Mapped<string, KeyValuePair<string, string>>(
                    (uri, idx) => new KeyValuePair<string, string>("ns" + idx, uri),
                    uris
                )
            )
        )
        { }

        /// <summary>
        /// A XPathContext, by merging the given parts
        /// </summary>
        /// <param name="old">existing namespace map</param>
        /// <param name="prefix">new namespace prefix</param>
        /// <param name="nsuri">new namespace uri</param>
        public XPathContext(IDictionary<string, string> old, string prefix, string nsuri) : this(
            new MapOf<string, string>(
                new Joined<KeyValuePair<string, string>>(
                    old,
                    new KeyValuePair<string, string>(prefix, nsuri)
                    )
                )
            )
        { }

        /// <summary>
        /// A XPathcontext, by merging the given parts
        /// </summary>
        /// <param name="map">map with new namespaces</param>
        /// <param name="contexts">existing contexts</param>
        private XPathContext(IDictionary<string, string> map, params IXmlNamespaceResolver[] contexts) : this(
            map,
            new Mapped<IXmlNamespaceResolver, IScalar<IXmlNamespaceResolver>>(
                nsr => new ScalarOf<IXmlNamespaceResolver>(nsr),
                contexts
            )
        )
        { }

        /// <summary>
        /// A XPathcontext, by merging the given parts
        /// </summary>
        /// <param name="map">map with new namespaces</param>
        /// <param name="contexts">existing contexts</param>
        private XPathContext(IDictionary<string, string> map, IEnumerable<IScalar<IXmlNamespaceResolver>> contexts)
        {
            this.map = map;
            this.contexts = contexts;
        }

        /// <summary>
        /// Uri for the prefix, if known.
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <returns>the uri</returns>
        public string LookupNamespace(string prefix)
        {
            IEnumerable<string> ns = FromMap(prefix);

            if (new LengthOf(ns).Value() == 0)
                ns = FromContexts(prefix);

            if (new LengthOf(ns).Value() == 0)
                ns = FromXMLSpec(prefix);

            if (new LengthOf(ns).Value() == 0)
                return null;
            else
                return new ItemAt<string>(ns).Value();
        }

        /// <summary>
        /// Prefix for the uri, if known.
        /// </summary>
        /// <param name="namespaceName">uri</param>
        /// <returns>the prefix</returns>
        public string LookupPrefix(string namespaceName)
        {
            String prefix = null;

            foreach (var entry in GetNamespacesInScope(XmlNamespaceScope.All))
            {
                if(entry.Value.Equals(namespaceName))
                {
                    prefix = entry.Key;
                    break;
                }
            }
            return prefix;
        }

        /// <summary>
        /// All known namespaces in the scope.
        /// </summary>
        /// <param name="scope">scope to look in</param>
        /// <returns>map of namespaces prefix=uri</returns>
        public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            foreach (var kvp in this.map)
            {
                if (scope != XmlNamespaceScope.ExcludeXml
                    ||
                    (kvp.Key != XMLNS_ATTRIBUTE && kvp.Key != XML_NS_PREFIX)
                )
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var ctx in this.contexts)
            {
                IEnumerable<KeyValuePair<string, string>> namespaces =
                    ctx.Value().GetNamespacesInScope(scope);
                result = new MapOf<string, string>(result, namespaces);
            }

            return result;
        }

        /// <summary>
        /// Known namespaces as a string.
        /// </summary>
        /// <returns>known namespaces</returns>
        public override string ToString()
        {
            return
                String.Join(
                    ", ",
                    new Mapped<KeyValuePair<string, string>, string>(
                        pair => pair.Key + "=" + pair.Value,
                        this.map
                    )
                );
        }

        /// <summary>
        /// Lookup in own map.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private IEnumerable<string> FromMap(string prefix)
        {
            var ns = new EnumerableOf<string>();
            if (this.map.ContainsKey(prefix))
            {
                ns = new EnumerableOf<string>(this.map[prefix]);
            }
            return ns;
        }

        /// <summary>
        /// Lookup in other contexts.
        /// </summary>
        /// <param name="prefix">prefix to find</param>
        /// <returns>list with one or no result</returns>
        private IEnumerable<string> FromContexts(string prefix)
        {
            IEnumerable<string> ns = new EnumerableOf<string>();
            foreach (IScalar<IXmlNamespaceResolver> ctx in this.contexts)
            {
                string uri = ctx.Value().LookupNamespace(prefix);
                if (uri != null)
                {
                    ns = new EnumerableOf<string>(uri);
                }
            }
            return ns;
        }

        /// <summary>
        /// Lookup in XML specification.
        /// </summary>
        /// <param name="prefix">prefix to find</param>
        /// <returns>list with one or no result</returns>
        private IEnumerable<string> FromXMLSpec(string prefix)
        {
            IEnumerable<string> ns;

            if (prefix.Equals(XML_NS_PREFIX)) //Defined by the XML specification to be "xml"
            {
                ns = new EnumerableOf<string>(XML_NS_URI); //Defined by the XML specification
            }
            else if (prefix.Equals(XMLNS_ATTRIBUTE)) //Defined by the XML specification to be "xmlns"
            {
                ns = new EnumerableOf<string>(XMLNS_ATTRIBUTE_NS_URI); //Defined by the XML specification
            }
            else
            {
                ns = new EnumerableOf<string>(NULL_NS_URI);
            }
            return ns;
        }
    }
}
