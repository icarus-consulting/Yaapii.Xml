using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.IO;

namespace Yaapii.Xml
{
    /// <summary>
    /// Strips the whitespaces from a XML document.<br/>
    /// </summary>
    public class XSLStripped : XSLEnvelope
    {
        /// <summary>
        /// Strips whitespaces between element tags in XML Document.
        /// </summary>
        public XSLStripped() : base(
            new TextOf(
                new ResourceOf(
                    "Resources.strip.xsl", 
                    new ScalarOf<Assembly>(
                        () => Assembly.GetAssembly(typeof(XSLStripped))
                    )
                )
            ),
            new SourcesEmpty(),
            new Dictionary<string, object>()
        )
        { }
    }
}
