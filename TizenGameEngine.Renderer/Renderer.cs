
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using Tizen.Applications;
using TizenGameEngine.Renderer.Common;

namespace TizenGameEngine.Renderer
{
    public class Renderer
    {
        DirectoryInfo _directoryInfo;
        int _width, _height;

        public Renderer(DirectoryInfo directoryInfo, int width, int height)
        {
            _directoryInfo = directoryInfo;

            _width = width;
            _height = height;
        }

        // Handle to a program object
        int programObject;
        // Attribute locations
        int positionLoc, uniformLoc;
        // Uniform locations
        int mvpLoc, textureHandle;
        // Vertex data
        float[] vertices;
        // Attribute locations
        ushort[] indices;

        // MVP matrix
        public Matrix4 mvpMatrix;
        public Matrix4 perspective;
        public Matrix4 modelview;

        public void Load()
        {
            GL.ClearColor(Color4.Gray);
            GL.Enable(EnableCap.DepthTest);
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

            programObject = ShaderHelper.BuildProgram(vShaderStr, fShaderStr);
            GL.BindAttribLocation(programObject, 0, "a_position");
            GL.BindAttribLocation(programObject, 1, "aTexture");
            GL.LinkProgram(programObject);
            int textID = TextureHelper.CreateTexture2D(_directoryInfo.Resource + "1.bmp");
            GL.ActiveTexture(TextureUnit.Texture0);
            // Bind the texture to this unit.
            GL.BindTexture(TextureTarget.Texture2D, textID);
            GL.Uniform1(uniformLoc, 0);
            vertices = new float[]
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

            indices = new ushort[]
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
            GL.UseProgram(programObject);
            positionLoc = GL.GetAttribLocation(programObject, "a_position");
            textureHandle = GL.GetAttribLocation(programObject, "aTexture");
            uniformLoc = GL.GetAttribLocation(programObject, "uTexMap");

            GL.EnableVertexAttribArray(positionLoc);
            GL.EnableVertexAttribArray(textureHandle);
            unsafe
            {
                // Prepare the triangle coordinate data
                GL.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), vertices);
                mvpLoc = GL.GetUniformLocation(programObject, "u_mvpMatrix");
                GL.UniformMatrix4(mvpLoc, false, ref mvpMatrix);

                fixed (float* atexture = &vertices[3])
                {
                    // Prepare the triangle coordinate data
                    GL.VertexAttribPointer(textureHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), new IntPtr(atexture));
                }

                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, vertices.Length / 5);
            }
            // Disable vertex array
            GL.DisableVertexAttribArray(positionLoc);
            GL.DisableVertexAttribArray(textureHandle);
        }

        public void Rotate(ref float anglex, ref float angley, ref Matrix4 perspective, ref Matrix4 model, ref Matrix4 mvmatrix, int flag)
        {
            if (anglex >= 360.0f)
            {
                anglex -= 360.0f;
            }

            GL.Viewport(0, 0, _width, _height);
            float ratio = (float)_width / _height;
            MatrixState.EsMatrixLoadIdentity(ref perspective);
            MatrixState.EsPerspective(ref perspective, 60.0f, ratio, 1.0f, 20.0f);
            MatrixState.EsMatrixLoadIdentity(ref model);
            if (flag == 1)
            {
                MatrixState.EsTranslate(ref modelview, 0.0f, -0.8f, -3.0f);
            }
            else
            {
                MatrixState.EsTranslate(ref model, 0.0f, 1f, -5.0f);
            }

            MatrixState.EsRotate(ref model, anglex, 0.0f, 1.0f, 0.0f);

            if (angley >= 360.0f)
                {
                    angley -= 360.0f;
                }

                MatrixState.EsRotate(ref model, angley, 1.0f, 0.0f, 0.0f);

            mvmatrix = Matrix4.Mult(model, perspective);
        }
    }
}

