using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK;
#if TIZEN
using OpenTK;
#else
using OpenTK.Mathematics;
#endif
using Renderer.Models.Mesh;

namespace Renderer.MeshLoaders
{
    public class ObjMeshLoader : IMeshLoader
    {
        public ICollection<Face> Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Unable to open \"" + path + "\", does not exist.");
            }

            var vertices = new List<Vector3>();
            var textureVertices = new List<Vector2>();
            var faces = new List<Face>();

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            using (StreamReader streamReader = new StreamReader(path))
            {
                while (!streamReader.EndOfStream)
                {
                    List<string> words = new List<string>(streamReader.ReadLine().ToLower().Split(' '));
                    words.RemoveAll(s => s == string.Empty);

                    if (words.Count == 0)
                        continue;

                    string type = words[0];
                    words.RemoveAt(0);

                    switch (type)
                    {
                        // vertex
                        case "v":
                            vertices.Add(new Vector3
                            {
                                X = float.Parse(words[0]),
                                Y = float.Parse(words[1]),
                                Z = float.Parse(words[2]),
                            });
                            break;

                        case "vt":
                            textureVertices.Add(new Vector2
                            {
                                X = float.Parse(words[0]),
                                Y = float.Parse(words[1]),
                            });
                            break;

                        // face
                        case "f":
                            var face = new Face
                            {
                                Vertexes = new List<Vector3>(),
                                Textures = new List<Vector2>(),
                                Normals = new List<Vector3>(),
                            };

                            foreach (string w in words)
                            {
                                if (w.Length == 0)
                                    continue;

                                string[] comps = w.Split('/');

                                face.Vertexes.Add(vertices[int.Parse(comps[0])-1]);
                                face.Textures.Add(textureVertices[int.Parse(comps[1])-1]);
                            }

                            faces.Add(face);
                            break;

                        default:
                            break;
                    }
                }
            }


            return faces;
        }
    }
}

