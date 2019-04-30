using System.Collections.Generic;
using System.Xml.Linq;
using Yaapii.Atoms;
using Yaapii.Xml;

namespace Yaapii.XML
{
    /// <summary>
    /// An envelope for XML
    /// </summary>
    public abstract class XMLEnvelope : IXML
    {
        private readonly IScalar<IXML> xml;

        /// <summary>
        /// An envelope for XML
        /// </summary>
        public XMLEnvelope(IScalar<IXML> xml)
        {
            this.xml = xml;
        }

        public XNode AsNode()
        {
            return this.xml.Value().AsNode();
        }

        public IList<IXML> Nodes(string query)
        {
            return this.xml.Value().Nodes(query);

        }

        public IList<string> Values(string query)
        {
            return this.xml.Value().Values(query);

        }

        public IXML WithNamespace(string prefix, object uri)
        {
            return this.xml.Value().WithNamespace(prefix, uri);
        }
    }
}
