using System.Collections.Generic;
using System;
using ProtoBuf;

namespace zero_allocation_app.proto {

    [ProtoContract]
    public sealed class SimpleModel
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public byte[] EncodedContent;
    
        public static SimpleModel GetModel(int contentLength) {
            return new SimpleModel {
                Id = 1,
                EncodedContent = new byte[contentLength],
            };
        }
    }
}

