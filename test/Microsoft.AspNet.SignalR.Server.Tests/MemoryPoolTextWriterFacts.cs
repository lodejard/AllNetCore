using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNet.SignalR.Infrastructure;
using Xunit;
using Moq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class MemoryPoolTextWriterFacts
    {
        [Fact]
        public void CanEncodingSurrogatePairsCorrectly()
        {
            var bytes = new List<byte>();
            var writer = new BinaryMemoryPoolTextWriter(new MemoryPool());

            writer.Write("\U00024B62"[0]);
            writer.Write("\U00024B62"[1]);
            writer.Flush();

            var expected = new byte[] { 0xF0, 0xA4, 0xAD, 0xA2 };

            var buffer = writer.Buffer;
            Assert.Equal(4, buffer.Count);
            Assert.Equal(expected, new ArraySegment<byte>(buffer.Array, 0, buffer.Count));
        }

        [Fact]
        public void CanInterleaveStringsAndRawBinary()
        {
            var writer = new BinaryMemoryPoolTextWriter(new MemoryPool());

            var encoding = new UTF8Encoding();

            writer.Write('H');
            writer.Write('e');
            writer.Write("llo ");
            writer.Write(new ArraySegment<byte>(encoding.GetBytes("World")));
            writer.Flush();

            var buffer = writer.Buffer;
            Assert.Equal(11, buffer.Count);
            var s = encoding.GetString(buffer.Array, 0, buffer.Count);

            Assert.Equal("Hello World", s);
        }
    }
}