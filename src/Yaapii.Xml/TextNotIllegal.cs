﻿using System;
using System.Collections.Generic;
using System.Text;
using Yaapii.Atoms;

namespace Yaapii.Xml
{
    /// <summary>
    /// A text that is checked for chars which are illegal in xml.
    /// </summary>
    public class TextNotIllegal : IText
    {
        private readonly IText txt;

        public TextNotIllegal(IText txt)
        {
            this.txt = txt;
        }

        public string AsString()
        {
            var str = txt.AsString();
            foreach(var c in str)
            {
                new NotIllegal(c).Value();
            }
            return str;
        }

        public bool Equals(IText other)
        {
            return txt.Equals(other);
        }
    }
}
