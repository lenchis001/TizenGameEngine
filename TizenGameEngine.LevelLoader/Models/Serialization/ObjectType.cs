using System;
using System.Runtime.Serialization;

namespace TizenGameEngine.LevelLoader.Models.Serialization
{
    [DataContract]
    public enum ObjectType
    {
        [EnumMember(Value = "level")]
        LEVEL,

        [EnumMember(Value = "obj_mesh")]
        OBJ_MESH,

        UNKNOWN
    }
}

