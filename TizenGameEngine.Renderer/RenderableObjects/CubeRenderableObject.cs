﻿using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.ES20;
using Tizen.Applications;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;

namespace TizenGameEngine.Renderer.RenderableObjects
{
    public class CubeRenderableObject: IRenderableObject
    {
        // Handle to a program object
        int _programObject;
        // Attribute locations
        int _positionLoc, _uniformLoc;
        // Uniform locations
        int _mvpLoc, _textureHandle;
        // Vertex data
        float[] _vertices;
        // Attribute locations
        ushort[] _indices;

        private Vector3 _position, _rotation, _scale;

        // MVP matrix
        readonly ReferenceContainer<Matrix4> _perspective;
        Matrix4 _mvpMatrix;
        Matrix4 _modelview;

        readonly DirectoryInfo _directoryInfo;

        public CubeRenderableObject(DirectoryInfo directoryInfo, ReferenceContainer<Matrix4> perspective)
        {
            _directoryInfo = directoryInfo;
            _perspective = perspective;
        }

        public void Load()
        {
            string vShaderStr = "uniform mat4 u_mvpMatrix;              \n" +
                          "attribute vec2 aTexture;                      \n" +
                          "attribute vec4 a_position;                  \n" +
                          "varying vec2 vtexture;\n" +
                          "void main()                                 \n" +
                          "{                                           \n" +
                          "   vtexture = aTexture;  \n" +
                          "   gl_Position = u_mvpMatrix * a_position;  \n" +
                          "}                                           \n";
            string fShaderStr = "precision mediump float;  \n" +
                                "uniform sampler2D uTexMap;\n" +
                                "varying vec2 vtexture;\n" +
                               "void main()    \n" +
                               "{             \n" +
                               "  gl_FragColor = texture2D( uTexMap, vtexture); \n" +
                               "}\n";

            _programObject = ShaderHelper.BuildProgram(vShaderStr, fShaderStr);
            GL.BindAttribLocation(_programObject, 0, "a_position");
            GL.BindAttribLocation(_programObject, 1, "aTexture");
            GL.LinkProgram(_programObject);
            int textID = TextureHelper.CreateTexture2D(_directoryInfo.Resource + "1.bmp");
            GL.ActiveTexture(TextureUnit.Texture0);
            // Bind the texture to this unit.
            GL.BindTexture(TextureTarget.Texture2D, textID);
            GL.Uniform1(_uniformLoc, 0);
            _vertices = new float[]
            {
                -0.5f, 0.5f, 0.5f, 0.0f,1.0f,//0
                0.5f, 0.5f,  0.5f, 1.0f,1.0f,//1
                -0.5f, 0.5f,  -0.5f, 0.0f,0.0f,//2
                0.5f, 0.5f, -0.5f, 1.0f,0.0f,//3
                -0.5f,  -0.5f, -0.5f, 0.0f,1.0f,//4
                0.5f,  -0.5f,  -0.5f, 1.0f,1.0f,//5
                -0.5f, -0.5f, 0.5f, 0.0f,0.0f,//6
                0.5f,  -0.5f,  0.5f, 1.0f, 0.0f,//7

                -0.5f, 0.5f, 0.5f, 0.0f,1.0f,//0
                0.5f, 0.5f,  0.5f, 1.0f,1.0f,//1
                0.5f, 0.5f,  0.5f, 1.0f,1.0f,//1
                0.5f, 0.5f, -0.5f, 1.0f,0.0f,//3

                0.5f, 0.5f, -0.5f, 1.0f,1.0f,//3
                0.5f, 0.5f,  0.5f, 0.0f,1.0f,//1
                0.5f,  -0.5f,  -0.5f, 1.0f,0.0f,//5
                0.5f,  -0.5f,  0.5f, 0.0f, 0.0f,//7
                
                0.5f,  -0.5f,  0.5f, 1.0f, 0.0f,//7
                -0.5f, -0.5f, 0.5f, 0.0f,0.0f,//6

                -0.5f, -0.5f, 0.5f, 0.0f,0.0f,//6
                -0.5f,  -0.5f, -0.5f, 1.0f,0.0f,//4
                -0.5f, 0.5f, 0.5f, 0.0f,1.0f,//0
                -0.5f, 0.5f,  -0.5f, 1.0f,1.0f,//2
            };

            _indices = new ushort[]
            {
                0, 2, 1,3,//up
				6,5,//right
				7,4,//below
				0,2,//left
				4,3,5,2,//back
				7,6,0,1 //front
            };
        }

        public void Draw()
        {
            GL.UseProgram(_programObject);
            _positionLoc = GL.GetAttribLocation(_programObject, "a_position");
            _textureHandle = GL.GetAttribLocation(_programObject, "aTexture");
            _uniformLoc = GL.GetAttribLocation(_programObject, "uTexMap");

            GL.EnableVertexAttribArray(_positionLoc);
            GL.EnableVertexAttribArray(_textureHandle);
            unsafe
            {
                // Prepare the triangle coordinate data
                GL.VertexAttribPointer(_positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), _vertices);
                _mvpLoc = GL.GetUniformLocation(_programObject, "u_mvpMatrix");
                GL.UniformMatrix4(_mvpLoc, false, ref _mvpMatrix);

                fixed (float* atexture = &_vertices[3])
                {
                    // Prepare the triangle coordinate data
                    GL.VertexAttribPointer(_textureHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), new IntPtr(atexture));
                }

                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, _vertices.Length / 5);
            }
            // Disable vertex array
            GL.DisableVertexAttribArray(_positionLoc);
            GL.DisableVertexAttribArray(_textureHandle);
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
    }
}
