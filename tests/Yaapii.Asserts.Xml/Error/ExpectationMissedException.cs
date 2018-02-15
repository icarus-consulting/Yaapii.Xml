using System;
using System.Collections.Generic;
using System.Text;

namespace Yaapii.Asserts.Xml
{
    public class ExpectationMissedException : Exception
    {
        public ExpectationMissedException()
        {

        }

        public ExpectationMissedException(string hint) : base(hint)
        {

        }
    }
}
