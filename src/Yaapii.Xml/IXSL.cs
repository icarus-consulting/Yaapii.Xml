using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Yaapii.Xml
{
    public interface IXSL
    {
        /// <summary>
        /// Transform XML to another one.
        /// </summary>
        /// <param name="xml">xml document</param>
        /// <returns>transformed document</returns>
        IXML Transformed(IXML xml);

        /// <summary>
        /// Transform XML to text.
        /// </summary>
        /// <param name="xml">xml document</param>
        /// <returns>transformed text</returns>
        string TransformedToText(IXML xml);

        /// <summary>
        /// Register a new source for XSL imports.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns>XSL with registered sources</returns>
        IXSL With(XmlResolver sources);

        /// <summary>
        /// Register a new parameter used in transformation.
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="value">the value</param>
        /// <returns>new XSL with registered parameter</returns>
        IXSL With(string name, object value);
    }
}
