using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
#if !TIZEN
using OpenTK.Mathematics;
#endif
using Renderer.Models.Mesh;

namespace TizenGameEngine.Renderer.Services.MeshLoading
{
    public interface IMeshLoadingService
    {
        ICollection<Face> Load(string path);
    }

    public static class MeshLoadersExtensions
    {
        public static IEnumerable<Vector3> ToVertexes(this IEnumerable<Face> faces)
            => faces
                .Select(f => (IEnumerable<Vector3>)f.Vertexes)
                .Aggregate((a, b) => a.Concat(b));

        public static IEnumerable<Vector2> ToTextureVertexes(this IEnumerable<Face> faces)
            => faces
                .Select(f => (IEnumerable<Vector2>)f.Textures)
                .Aggregate((a, b) => a.Concat(b));

        public static IEnumerable<float> ToIndices(this IEnumerable<Vector3> vectors)
            => vectors
                .Select(v => (IEnumerable<float>)(new[] { v.X, v.Y, v.Z }))
                .Aggregate((a, b) => a.Concat(b));

        public static IEnumerable<float> ToIndices(this IEnumerable<Vector2> vectors)
            => vectors
                .Select(v => (IEnumerable<float>)(new[] { v.X, v.Y }))
                .Aggregate((a, b) => a.Concat(b));
    }
}

