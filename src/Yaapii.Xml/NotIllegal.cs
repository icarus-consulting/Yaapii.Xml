using System;
using System.Collections.Generic;
using System.Text;
using Yaapii.Atoms;
using Yaapii.Atoms.Text;

namespace Yaapii.Xml
{
    /// <summary>
    /// A legal XML character.
    /// </summary>
    public class NotIllegal : IScalar<Char>
    {
        private readonly char _chr;

        /// <summary>
        /// Validate char number and throw exception if it's not legal.
        /// </summary>
        /// <param name="chr"></param>
        /// <exception cref="XmlContentException">If illegal</exception>
        public NotIllegal(char chr)
        {
            this._chr = chr;
        }

        /// <summary>
        /// Validate char number and throw exception if it's not legal.
        /// </summary>
        /// <returns>The same number</returns>
        public Char Value()
        {
            this.Range(_chr, 0x00, 0x08);
            this.Range(_chr, 0x0B, 0x0C);
            this.Range(_chr, 0x0E, 0x1F);
            this.Range(_chr, 0x7F, 0x84);
            this.Range(_chr, 0x86, 0x9F);
            return _chr;
        }

        /// <summary>
        /// Throw if number is in the range.
        /// </summary>
        /// <param name="c">Char number</param>
        /// <param name="left">Left number</param>
        /// <param name="right">Right number</param>
        /// <exception cref="XmlContentException">If illegal</exception>
        private void Range(char c, int left, int right)
        {
            if (c >= left && c <= right)
            {
                throw new System.Xml.XmlException(
                    new FormattedText(
                        "Character {0} is in the restricted XML range {1} - {2}, see http://www.w3.org/TR/2004/REC-xml11-20040204/#charsets",
                        c, left, right
                        ).AsString());
            }
        }
    }
}
