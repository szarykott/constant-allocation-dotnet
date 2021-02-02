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

namespace zero_allocation_app.bench {

    [MemoryDiagnoser]
    // [SimpleJob(RunStrategy.Throughput, warmupCount : 2, targetCount: 5)]
    [GcServer(true)]
    public class SerializationBenchmarks {

        private List<Model> _models;
        private RecyclableMemoryStreamManager _streamManager;

        [GlobalSetup]
        public void Setup() {
            _models = new List<Model>();
            for (var i = 0; i < 10_000; i++) {
                _models.Add(Model.GenerateRandomModel());
            }
            _streamManager = new RecyclableMemoryStreamManager();
        }

        [Benchmark]
        public void Naive() {
            foreach (var model in _models) {
                using (var stream = new MemoryStream()) {
                    Serializer.Serialize(stream, model);
                    var bytes = stream.ToArray();
                    Nop(bytes);
                }
            }
        }

        [Benchmark]
        public void RecyclableStream() {
            foreach (var model in _models) {
                using (var stream = _streamManager.GetStream()) {
                    Serializer.Serialize(stream, model);
                    var bytes = stream.ToArray();
                    Nop(bytes);
                }
            }
        }

        [Benchmark]
        public void RecyclableStreamWithArrayPool() {
            foreach (var model in _models) {
                using (var stream = _streamManager.GetStream()) {
                    Serializer.Serialize(stream, model);
                    var len = (int)stream.Length;
                    var buffer = ArrayPool<byte>.Shared.Rent(len);
                    stream.Read(buffer, 0, len);
                    var mem = buffer.AsMemory<byte>(0, len);
                    Nop(mem);
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Nop(Memory<byte> obj) { }
    }
}