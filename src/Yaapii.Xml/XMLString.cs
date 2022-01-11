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
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml
{
    /// <summary>
    /// A string in a document, retrieved by xpath.
    /// </summary>
    public sealed class XMLString : IScalar<string>
    {
        private readonly IScalar<string> result;

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public XMLString(IXML xml, string xpath) :
            this
            (
                xml,
                xpath,
                () =>
                {
                    throw new ArgumentException(
                        $"Cannot retrieve single value with XPath '{xpath}', because it had no results in document{Environment.NewLine}'{xml.AsNode().ToString()}'."
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
            this.result = new ScalarOf<string>(() =>
            {

                var matches = xml.Values(xpath);
                var result = string.Empty;
                if (matches.Count < 1)
                {
                    result = fallback();
                }
                else if (matches.Count > 1)
                {
                    throw new ArgumentException(
                        $"Cannot retrieve single value with XPath '{xpath}' because it resulted in multiple values in document{Environment.NewLine}'{xml.AsNode().ToString()}'."
                    );
                }
                else
                {
                    result = matches[0];
                }
                return result;
            });
        }

        /// <summary>
        /// A string in a document, retrieved by xpath.
        /// </summary>
        public string Value()
        {
            return this.result.Value();
        }
    }
}
