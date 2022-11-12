using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
#if TIZEN
using OpenTK;
#else
using OpenTK.Mathematics;
#endif
using System.Linq;
using OpenTK.Graphics.ES30;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;
using Renderer.MeshLoaders;
using TizenGameEngine.Renderer.Services.MeshLoading;

namespace TizenGameEngine.Renderer.RenderableObjects
{
    public class MeshRenderableObject : IRenderableObject
    {
        private readonly string _modelPath;
        private readonly ICollection<string> _texturePath;
        readonly ReferenceContainer<Matrix4> _perspective;

        int _vbo, _textureVbo, _vertexesAmount;
        // Handle to a program object
        int _shaderProgram;
        // Attribute locations
        int _uniformLoc;
        // Uniform locations
        int _mvpLoc;

        private Vector3 _position, _rotation, _scale;

        // MVP matrix
        Matrix4 _mvpMatrix;
        Matrix4 _modelview;
        private bool disposedValue;

        public MeshRenderableObject(
            string modelPath,
            ICollection<string> texturePath,
            ReferenceContainer<Matrix4> perspective,
            int shaderProgram)
        {
            _modelPath = modelPath;
            _texturePath = texturePath;
            _perspective = perspective;
            _shaderProgram = shaderProgram;
        }

        public void Load()
        {
            var meshLoader = new ObjMeshLoadingService();
            var faces = meshLoader.Load(_modelPath);

            var vertices = faces.ToVertexes().ToIndices().ToArray();
            var textureCoordinates = faces.ToTextureVertexes().ToIndices().ToArray();

            _vertexesAmount = vertices.Count();

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Count(), vertices, BufferUsageHint.StaticDraw);

            // -------------------------

            int textID = TextureHelper.CreateTexture2D(_texturePath.First());
            GL.ActiveTexture(TextureUnit.Texture0);
            // Bind the texture to this unit.
            GL.BindTexture(TextureTarget.Texture2D, textID);
            GL.Uniform1(_uniformLoc, 0);

            _textureVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * textureCoordinates.Count(), textureCoordinates, BufferUsageHint.StaticDraw);
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

                _mvpLoc = GL.GetUniformLocation(_shaderProgram, "u_mvpMatrix");
                GL.UniformMatrix4(_mvpLoc, false, ref _mvpMatrix);

                // Prepare the triangle coordinate data
                GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
                GL.VertexAttribPointer(textureHandle, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexesAmount/3);
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
    }
}

