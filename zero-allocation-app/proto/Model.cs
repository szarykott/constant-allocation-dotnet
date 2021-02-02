using System.Collections.Generic;
using System;
using ProtoBuf;

namespace zero_allocation_app.proto {

    [ProtoContract]
    public sealed class Model {
        public static Random _rand = new Random();

        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int Id;
        [ProtoMember(3)]
        public byte[] EncodedContent;
        [ProtoMember(4)]
        public List<Model> SubModels;
    
        public static Model GenerateRandomModel() {
            var model = new Model {
                Name = "dupa",
                Id = _rand.Next(),
                EncodedContent = new byte[_rand.Next(0, 10)],
                SubModels = new List<Model>()
            };
            for (var i = 0; i < _rand.Next(0, 50); i++) {
                model.SubModels.Add(new Model {
                    Name = "dupa2",
                    Id = _rand.Next(),
                    EncodedContent = new byte[_rand.Next(0, 100)],
                    SubModels = new List<Model>()
                });
            }
            return model;
        }
    }
}

