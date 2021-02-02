using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using zero_allocation_app.proto;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ProtoBuf;
using Microsoft.IO;
using System.Buffers;
using System;
using zero_allocation_app.ser;
using BenchmarkDotNet.Jobs;

namespace zero_allocation_app.bench {

    [MemoryDiagnoser]
    [GcServer(true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class SerializationBenchmarks {

        private List<SimpleModel> _models;

        [Params(100, 10_000, 100_000)]
        public int Size;

        [GlobalSetup]
        public void Setup() {
            _models = new List<SimpleModel>();
            for (var i = 0; i < 100; i++) {
                _models.Add(SimpleModel.GetModel(Size));
            }
        }

        [Benchmark]
        public void Naive() {
            foreach (var model in _models) {
                using var bytes = Serialization.Naive(model);
                Nop(bytes.Bytes);
            }
        }

        [Benchmark]
        public void RecyclableStream() {
            foreach (var model in _models) {
                using var bytes = Serialization.RecyclableStream(model);
                Nop(bytes.Bytes);
            }
        }

        [Benchmark]
        public void RecyclableStreamWithArrayPool() {
            foreach (var model in _models) {
                using var bytes = Serialization.RecyclableStreamWithArrayPool(model);
                Nop(bytes.Bytes);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Nop(ReadOnlyMemory<byte> obj) { }
    }
}