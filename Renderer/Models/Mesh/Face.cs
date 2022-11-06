using System;
using System.Collections.Generic;
#if TIZEN
using OpenTK;
#else
using OpenTK.Mathematics;
#endif

namespace Renderer.Models.Mesh
{
    public struct Face
    {
        public ICollection<Vector3> Vertexes { get; set; }

        public ICollection<Vector2> Textures { get; set; }

        public ICollection<Vector3> Normals { get; set; }
    }
}

