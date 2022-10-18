using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.ES20;
using Tizen.Applications;
using TizenGameEngine.Logger;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;
using TizenGameEngine.Renderer.Services;
using DirectoryInfo = Tizen.Applications.DirectoryInfo;

namespace TizenGameEngine.Renderer.RenderableObjects
{
    public class NObjMeshRenderableObject : IRenderableObject
    {
        readonly DirectoryInfo _directoryInfo;
        readonly ReferenceContainer<Matrix4> _perspective;

        int _vbo, _textureVbo, _vertexesAmount;
        // Handle to a program object
        int _shaderProgram;
        // Attribute locations
        int _uniformLoc;
        // Uniform locations
        int _mvpLoc;

        private Vector3 _position, _rotation, _scale;

        private string _path;

        // MVP matrix
        Matrix4 _mvpMatrix;
        Matrix4 _modelview;
        private bool disposedValue;

        public NObjMeshRenderableObject(
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
            var (vertices, textureCoordinates) = _LoadMeshFile();

            WebLogger.LogAsync($"Amount: {vertices.Length}");

            _vertexesAmount = vertices.Length;

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // -------------------------

            int textID = TextureHelper.CreateTexture2D(_directoryInfo.Resource + "1.bmp");
            GL.ActiveTexture(TextureUnit.Texture0);
            // Bind the texture to this unit.
            GL.BindTexture(TextureTarget.Texture2D, textID);
            GL.Uniform1(_uniformLoc, 0);

            _textureVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * textureCoordinates.Length, textureCoordinates, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.UseProgram(_shaderProgram);
            var positionLoc = GL.GetAttribLocation(_shaderProgram, "a_position");
            var textureHandle = GL.GetAttribLocation(_shaderProgram, "aTexture");
            _uniformLoc = GL.GetAttribLocation(_shaderProgram, "uTexMap");

            GL.EnableVertexAttribArray(positionLoc);
            GL.EnableVertexAttribArray(textureHandle);
            unsafe
            {
                // Prepare the triangle coordinate data
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                _mvpLoc = GL.GetUniformLocation(_shaderProgram, "u_mvpMatrix");
                GL.UniformMatrix4(_mvpLoc, false, ref _mvpMatrix);

                // Prepare the triangle coordinate data
                GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
                GL.VertexAttribPointer(textureHandle, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertexesAmount);
            }
            // Disable vertex array
            GL.DisableVertexAttribArray(positionLoc);
            GL.DisableVertexAttribArray(textureHandle);
        }

        public void Move(float x, float y, float z)
        {
            _position.X = x;
            _position.Y = y;
            _position.Z = z;

            RecalculateMatrix();
        }

        public void Rotate(float x, float y, float z)
        {
            _rotation.X = x;
            _rotation.Y = y;
            _rotation.Z = z;

            RecalculateMatrix();
        }

        public void Scale(float x, float y, float z)
        {
            _scale.X = x;
            _scale.Y = y;
            _scale.Z = z;

            RecalculateMatrix();
        }

        public void RecalculateMatrix()
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
            GL.DeleteBuffer(_textureVbo);
            GC.SuppressFinalize(this);
        }

        private (float[], float[]) _LoadMeshFile()
        {
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException("Unable to open \"" + _path + "\", does not exist.");
            }

            var vertices = new List<float>();
            var textureVertices = new List<float>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> vertexIndices = new List<uint>();
            List<uint> textureIndices = new List<uint>();
            List<uint> normalIndices = new List<uint>();

            using (StreamReader streamReader = new StreamReader(_path))
            {

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
                            textureVertices.Add(float.Parse(words[0]));
                            textureVertices.Add(float.Parse(words[1]));
                            //textureVertices.Add(float.Parse(words[2]));
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
            }

            return (vertices.ToArray(), textureVertices.ToArray());
        }
    }
}

