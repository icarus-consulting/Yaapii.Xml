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
using Yaapii.Atoms;
using Yaapii.Atoms.Text;
using Yaapii.Xml.Error;

namespace Yaapii.Xml
{
    /// <summary>
    /// A string in a document, retrieved by xpath.
    /// </summary>
    public sealed class XMLString : IScalar<string>
    {
        private readonly IXML xml;
        private readonly string xpath;
        private readonly Func<string> fallback;

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public XMLString(IXML xml, string xpath) : this(xml, xpath, 
            () =>
            {
                throw
                    new ArgumentException(
                        new FormattedText(
                            $"Cannot retrieve single value with XPath '{0}', because it had no results in document{Environment.NewLine} {1}",
                            xpath,
                            xml.AsNode().ToString()
                        ).AsString()
                    );
            }
        )
        { }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public XMLString(IXML xml, string xpath, string def) : this(xml, xpath, () => def)
        { }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        internal XMLString(IXML xml, string xpath, Func<string> fallback)
        {
            this.xml = xml;
            this.xpath = xpath;
            this.fallback = fallback;
        }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public string Value()
        {
            var matches = this.xml.Values(xpath);
            var result = string.Empty;
            if (matches.Count < 1)
            {
                result = this.fallback();
            }
            else if(matches.Count > 1)
            {
                throw 
                    new ArgumentException(
                        $"Cannot extract single value with xpath {this.xpath} because it resulted in multiple values in document {xml.ToString()}"
                    );
            }
            else
            {
                result = matches[0];
            }
            return result;
        }
    }
}
