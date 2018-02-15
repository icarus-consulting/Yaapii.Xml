using System.Collections.Generic;
using System.Xml;
using Yaapii.Atoms;
using Yaapii.Xml;

namespace Test.Yaapii.Xml
{
    public sealed class XSLEnvelopeRealization : XSLEnvelope
    {
        public XSLEnvelopeRealization(IText src, XmlResolver sources, IDictionary<string, object> map) : base(
            src,
            sources,
            map
        )
        { }
    }
}
