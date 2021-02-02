using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using zero_allocation_app.proto;
using zero_allocation_app.ser;

namespace zero_allocation_app.test
{
    public class SerializationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Test_AllSerializer_HaveSameOutput(int size)
        {
            var model = SimpleModel.GetModel(size);
            var n = Serialization.Naive(model);
            var sm = Serialization.RecyclableStream(model);
            var sma = Serialization.RecyclableStreamWithArrayPool(model);

            Assert.Equal(n.Bytes.Length, sm.Bytes.Length);
            Assert.Equal(n.Bytes.Length, sma.Bytes.Length);
        }
    }
}
