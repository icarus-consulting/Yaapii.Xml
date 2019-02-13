using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Yaapii.Xml.Test
{
    public sealed class XMLNumberTests
    {
        [Fact]
        public void ExtractsDouble()
        {
            Assert.Equal(
                1.2,
                new XMLNumber(
                    new XMLCursor("<root>1.2</root>"),
                    "/root/text()"
                ).AsDouble()
            );
        }

        [Fact]
        public void ExtractsInt()
        {
            Assert.Equal(
                2,
                new XMLNumber(
                    new XMLCursor("<root>2</root>"),
                    "/root/text()"
                ).AsInt()
            );
        }

        [Fact]
        public void ExtractsFloat()
        {
            Assert.Equal(
                2.3f,
                new XMLNumber(
                    new XMLCursor("<root>2.3</root>"),
                    "/root/text()"
                ).AsFloat()
            );
        }

        [Fact]
        public void ExtractsLong()
        {
            Assert.Equal(
                23,
                new XMLNumber(
                    new XMLCursor("<root>23</root>"),
                    "/root/text()"
                ).AsLong()
            );
        }
    }
}
