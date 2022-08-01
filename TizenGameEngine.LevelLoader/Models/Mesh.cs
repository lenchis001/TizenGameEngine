using System;
namespace TizenGameEngine.LevelLoader.Models
{
    public class ObjMesh: Object
    {
        public new ObjectType Type => ObjectType.OBJ_MESH;

        public string GeometryPath { get; set; }
        public string MtlPath { get; set; }

        public Vector Position { get; set; }
        public Vector Rotation { get; set; }
        public Vector Scale { get; set; }
    }
}

