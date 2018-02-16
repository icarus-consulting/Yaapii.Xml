using System.Collections.Generic;
using System.Xml;
using Yaapii.Atoms;
using Yaapii.Xml;

namespace Yaapii.Xml
{
    public sealed class XSLEnvelopeImplementation : XSLEnvelope
    {
        public XSLEnvelopeImplementation(IText src, XmlResolver sources, IDictionary<string, object> map) : base(
            src,
            sources,
            map
        )
        { }
    }
}
