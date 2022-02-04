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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml
{
    /// <summary>
    /// Validation of XML documents by XSD schema validation.
    /// <para>Is <see cref="IEnumerable{T}"/>. T is <see cref="Exception"/></para>
    /// </summary>
    public sealed class XSDErrors : IEnumerable<Exception>
    {
        private readonly IScalar<XDocument> xml;
        private readonly IScalar<string> schema;

        /// <summary>
        /// Validation of XML documents by XSD schema validation
        /// </summary>
        /// <param name="xml">The XML to be validate</param>
        /// <param name="schema">A XSD schema to validate with</param>
        public XSDErrors(IXML xml, IInput schema) : this(
            xml,
            new XMLCursor(schema)
        )
        { }

        /// <summary>
        /// Validation of XML documents by XSD schema validation
        /// </summary>
        /// <param name="xml">The XML to be validate</param>
        /// <param name="schema">A XSD schema to validate with</param>
        public XSDErrors(IXML xml, IXML schema) : this(
            new ScalarOf<XDocument>(() => xml.AsNode().Document),
            new ScalarOf<string>(() => schema.AsNode().ToString())
        )
        { }

        private XSDErrors(IScalar<XDocument> xml, IScalar<string> schema)
        {
            this.xml = xml;
            this.schema = schema;
        }

        public IEnumerator<Exception> GetEnumerator()
        {
            var schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(this.schema.Value())));
            IEnumerable<Exception> errors = new ManyOf<Exception>();
            this.xml.Value().Validate(
                schemas,
                (obj, ex) => errors = new Joined<Exception>(errors, ex.Exception)
            );

            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
