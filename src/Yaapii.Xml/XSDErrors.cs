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
            IEnumerable<Exception> errors = new EnumerableOf<Exception>();
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
