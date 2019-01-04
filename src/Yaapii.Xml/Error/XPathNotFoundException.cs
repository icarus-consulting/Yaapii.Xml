using System;

namespace Yaapii.Xml.Error
{
    public sealed class XPathNotFoundException : Exception
    {
        public XPathNotFoundException(string message) : base(
            message
        )
        { }
    }
}
