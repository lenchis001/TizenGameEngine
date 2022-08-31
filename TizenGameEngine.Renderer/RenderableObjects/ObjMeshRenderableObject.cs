using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK;
using OpenTK.Graphics.ES30;
using Tizen.Applications;
using TizenGameEngine.Logger;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;
using DirectoryInfo = Tizen.Applications.DirectoryInfo;

namespace TizenGameEngine.Renderer.RenderableObjects
{
    public class ObjMeshRenderableObject : IRenderableObject
    {
        private readonly int _shaderProgram;

        private int _vertexBufferObject, _vertexArrayObject;

        string _path;

        // Handle to a program object
        int _programObject;
        // Attribute locations
        int _positionLoc, _uniformLoc;
        // Uniform locations
        int _mvpLoc, _textureHandle;

        int _vbo, _shader;

        private Vector3 _position, _rotation, _scale;

        // MVP matrix
        readonly ReferenceContainer<Matrix4> _perspective;
        Matrix4 _mvpMatrix;
        Matrix4 _modelview;

        readonly Tizen.Applications.DirectoryInfo _directoryInfo;

        public ObjMeshRenderableObject(
            DirectoryInfo directoryInfo,
            ReferenceContainer<Matrix4> perspective,
            int shaderProgram)
        {
            _directoryInfo = directoryInfo;
            _perspective = perspective;
            _shaderProgram = shaderProgram;

            _path = _directoryInfo.Resource + "car.obj";
        }

        public void Load()
        {
            _LoadMeshFile();

        }

        public void Draw()
        {
            GL.UseProgram(_shaderProgram);
            _mvpLoc = GL.GetUniformLocation(_shaderProgram, "u_mvpMatrix");
            GL.UniformMatrix4(_mvpLoc, false, ref _mvpMatrix);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public void Move(float x, float y, float z)
        {
            _position.X = x;
            _position.Y = y;
            _position.Z = z;

            _recalculateMatrix();
        }

        public void Rotate(float x, float y, float z)
        {
            _rotation.X = x;
            _rotation.Y = y;
            _rotation.Z = z;

            _recalculateMatrix();
        }

        public void Scale(float x, float y, float z)
        {
            _scale.X = x;
            _scale.Y = y;
            _scale.Z = z;

            _recalculateMatrix();
        }

        private void _recalculateMatrix()
        {
            MatrixState.EsMatrixLoadIdentity(ref _modelview);

            MatrixState.EsTranslate(ref _modelview, _position.X, _position.Y, _position.Z);

            MatrixState.EsRotate(ref _modelview, _rotation.X, 0.0f, 1.0f, 0.0f);
            MatrixState.EsRotate(ref _modelview, _rotation.Y, 1.0f, 0.0f, 0.0f);
            MatrixState.EsRotate(ref _modelview, _rotation.Z, 0.0f, 0.0f, 1.0f);

            _mvpMatrix = Matrix4.Mult(_modelview, _perspective.Value);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GC.SuppressFinalize(this);
        }

        private void _LoadMeshFile()
        {
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException("Unable to open \"" + _path + "\", does not exist.");
            }

            using (StreamReader streamReader = new StreamReader(_path))
            {
                var vertices = new List<float>();
                List<Vector3> textureVertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<uint> vertexIndices = new List<uint>();
                List<uint> textureIndices = new List<uint>();
                List<uint> normalIndices = new List<uint>();

                while (!streamReader.EndOfStream)
                {
                    List<string> words = new List<string>(streamReader.ReadLine().ToLower().Split(' '));
                    words.RemoveAll(s => s == string.Empty);

                    if (words.Count == 0)
                        continue;

                    string type = words[0];
                    words.RemoveAt(0);

                    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                    switch (type)
                    {
                        // vertex
                        case "v":
                            vertices.Add(float.Parse(words[0]));
                            vertices.Add(float.Parse(words[1]));
                            vertices.Add(float.Parse(words[2]));
                            break;

                        case "vt":
                            textureVertices.Add(new Vector3(float.Parse(words[0]), float.Parse(words[1]),
                                                            words.Count < 3 ? 0 : float.Parse(words[2])));
                            break;

                        case "vn":
                            normals.Add(new Vector3(float.Parse(words[0]), float.Parse(words[1]), float.Parse(words[2])));
                            break;

                        // face
                        case "f":
                            foreach (string w in words)
                            {
                                if (w.Length == 0)
                                    continue;

                                string[] comps = w.Split('/');

                                // subtract 1: indices start from 1, not 0
                                vertexIndices.Add(uint.Parse(comps[0]) - 1);

                                if (comps.Length > 1 && comps[1].Length != 0)
                                    textureIndices.Add(uint.Parse(comps[1]) - 1);

                                if (comps.Length > 2)
                                    normalIndices.Add(uint.Parse(comps[2]) - 1);
                            }
                            break;

                        default:
                            break;
                    }
                }

                _vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Count, vertices.ToArray(), BufferUsageHint.StaticDraw);

                _vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArrayObject);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }
        }
    }
}

