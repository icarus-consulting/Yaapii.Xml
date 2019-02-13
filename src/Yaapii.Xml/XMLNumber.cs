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
