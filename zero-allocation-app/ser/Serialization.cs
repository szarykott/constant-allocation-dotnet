using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zero_allocation_app.proto;
using ProtoBuf;
using Microsoft.IO;
using System.Buffers;

namespace zero_allocation_app.ser
{
    public interface IBytes : IDisposable
    {
        ReadOnlyMemory<byte> Bytes { get; }
    }

    public sealed class Serialization
    {
        private static readonly RecyclableMemoryStreamManager _streamManager = new RecyclableMemoryStreamManager();

        private sealed class NaiveBytes : IBytes
        {
            public ReadOnlyMemory<byte> Bytes { get; set; }

            public void Dispose()
            {
                /* nothing */
            }
        }

        public static IBytes Naive(SimpleModel model)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, model);
                return new NaiveBytes
                {
                    Bytes = stream.ToArray()
                };
            }
        }

        public static IBytes RecyclableStream(SimpleModel model)
        {
            using (var stream = _streamManager.GetStream())
            {
                Serializer.Serialize(stream, model);
                return new NaiveBytes
                {
                    Bytes = stream.ToArray()
                };
            }
        }

        private sealed class PooledBytes : IBytes
        {
            private byte[] _bytes;
            private int _length;
            public ReadOnlyMemory<byte> Bytes => _bytes.AsMemory(0, _length);

            public PooledBytes(byte[] bytes, int length)
            {
                _bytes = bytes;
                _length = length;
            }

            public void Dispose()
            {
                ArrayPool<byte>.Shared.Return(_bytes);
                _bytes = null;
            }
        }

        public static IBytes RecyclableStreamWithArrayPool(SimpleModel model)
        {
            using (var stream = _streamManager.GetStream())
            {
                Serializer.Serialize(stream, model);

                var len = (int)stream.Length;
                var buffer = ArrayPool<byte>.Shared.Rent(len);
                stream.Read(buffer, 0, len);

                return new PooledBytes(buffer, len);
            }
        }

    }
}
