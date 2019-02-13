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
