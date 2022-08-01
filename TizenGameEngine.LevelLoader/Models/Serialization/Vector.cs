using System;
using System.Runtime.Serialization;

namespace TizenGameEngine.LevelLoader.Models.Serialization
{
    [DataContract]
    public class Vector
    {
        [DataMember(Name = "x")]
        public float X { get; set; }

        [DataMember(Name = "y")]
        public float Y { get; set; }

        [DataMember(Name = "z")]
        public float Z { get; set; }
    }
}

