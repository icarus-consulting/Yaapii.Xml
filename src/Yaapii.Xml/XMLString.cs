using System;
using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xml.Error;

namespace Yaapii.Xml
{
    public sealed class XMLString : IScalar<string>
    {
        private readonly IXML document;
        private readonly string xpath;
        private readonly IScalar<Exception> ex;

        public XMLString(IXML document, string xpath) : this(
            document,
            xpath,
            new StickyScalar<Exception>(() =>
                new XPathNotFoundException(
                    new FormattedText(
                        "XPath '{0}' not found in document: {1}",
                        xpath,
                        document.Node().ToString()
                    ).AsString()
                )
            )
        )
        { }

        public XMLString(IXML document, string xpath, string hint) : this(
            document,
            xpath,
            new Exception(hint)
        )
        { }

        public XMLString(IXML document, string xpath, Exception ex) : this(
            document,
            xpath,
            new ScalarOf<Exception>(ex)
        )
        { }

        private XMLString(IXML document, string xpath, IScalar<Exception> ex)
        {
            this.document = document;
            this.xpath = xpath;
            this.ex = ex;
        }

        public string Value()
        {
            var results = this.document.Values(xpath);
            if (new LengthOf(results).Value() < 1)
            {
                throw this.ex.Value();
            }
            return new ItemAt<string>(results).Value();
        }
    }
}
