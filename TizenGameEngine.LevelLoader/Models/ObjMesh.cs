using System;
using System.Collections.Generic;

namespace TizenGameEngine.LevelLoader.Models
{
    public class ObjMesh: Object
    {
        public string GeometryPath { get; set; }
        public ICollection<string> Textures { get; set; }

        public Vector Position { get; set; }
        public Vector Rotation { get; set; }
        public Vector Scale { get; set; }
    }
}

