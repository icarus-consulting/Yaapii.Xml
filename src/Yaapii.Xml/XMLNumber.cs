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
using System.Globalization;
using Yaapii.Atoms;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;

namespace Yaapii.Xml
{
    /// <summary>
    /// Number extracted from an xml using xpath.
    /// </summary>
    public sealed class XMLNumber : INumber
    {
        private readonly IScalar<INumber> number;

        /// <summary>
        /// Number extracted from an xml using xpath.
        /// </summary>
        public XMLNumber(IXML xml, string xpath) : this(xml, xpath, CultureInfo.InvariantCulture)
        { }

        /// <summary>
        /// Number extracted from an xml using xpath.
        /// </summary>
        public XMLNumber(IXML xml, string xpath, IFormatProvider provider)
        {
            this.number =
                new ScalarOf<INumber>(() =>
                    new NumberOf(
                        new XMLString(xml, xpath).Value(), 
                        provider
                    )
                );
        }

        public double AsDouble()
        {
            return this.number.Value().AsDouble();
        }

        public float AsFloat()
        {
            return this.number.Value().AsFloat();
        }

        public int AsInt()
        {
            return this.number.Value().AsInt();
        }

        public long AsLong()
        {
            return this.number.Value().AsLong();
        }
    }
}
