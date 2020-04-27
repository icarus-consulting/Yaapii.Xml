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

namespace Yaapii.Xml
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
        /// <param name="xml">The xml for the envelope</param>
        /// <param name="live">Is the envelope live or sticky</param>
        public XMLEnvelope(IScalar<IXML> xml, bool live = false)
        {
            this.xml = new ScalarOf<IXML>(xml, (file) => live);
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
