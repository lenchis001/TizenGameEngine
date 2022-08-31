using System;
using OpenTK.Graphics.ES30;

namespace TizenGameEngine.Renderer.RenderableObjects
{
    public class TriangleRenderableObject: IRenderableObject
    {
        private readonly int _shaderProgram;

        private int _vertexBufferObject, _vertexArrayObject;

        public TriangleRenderableObject(int shaderProgram)
        {
            _shaderProgram = shaderProgram;
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vertexBufferObject);
        }

        public void Draw()
        {
            GL.UseProgram(_shaderProgram);

            GL.BindVertexArray(_vertexArrayObject);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public void Load()
        {
            float[] vertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
            };

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Move(float x, float y, float z)
        {
        }

        public void Rotate(float x, float y, float z)
        {
        }

        public void Scale(float x, float y, float z)
        {
        }
    }
}

