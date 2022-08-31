using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.ES20;
using TizenGameEngine.Renderer.Models;

namespace TizenGameEngine.Renderer.Services
{
    public class ShaderService: IShaderService
    {
        private readonly IDictionary<ShaderUsage, int> _shaderPrograms;
        private readonly IDictionary<ShaderUsage, ShaderDescriptor> _shaderSources = new Dictionary<ShaderUsage, ShaderDescriptor>
        {
            {
                ShaderUsage.CUBE,
                new ShaderDescriptor{
                    VertexShaderSource =
                          "uniform mat4 u_mvpMatrix;              \n" +
                          "attribute vec2 aTexture;                      \n" +
                          "attribute vec4 a_position;                  \n" +
                          "varying vec2 vtexture;\n" +
                          "void main()                                 \n" +
                          "{                                           \n" +
                          "   vtexture = aTexture;  \n" +
                          "   gl_Position = u_mvpMatrix * a_position;  \n" +
                          "}                                           \n",
                    FragmentShader =
                          "precision mediump float;  \n" +
                          "uniform sampler2D uTexMap;\n" +
                          "varying vec2 vtexture;\n" +
                          "void main()    \n" +
                          "{             \n" +
                          "  gl_FragColor = texture2D(uTexMap, vtexture); \n" +
                          "}\n",
                    Arguments = new []{ "a_position", "aTexture" }
                }
            },

            {
                ShaderUsage.MESH,
                new ShaderDescriptor{
                    VertexShaderSource =
                          "# version 320 es\n" +
                          "uniform mat4 u_mvpMatrix;\n" +
                          "in vec3 aPosition;\n" +
                          "\n" +
                          "void main()\n" +
                          "{\n" +
                                "gl_Position = vec4(aPosition, 1.0) * u_mvpMatrix;\n" +
                          "}",
                    FragmentShader =
                          "# version 320 es\n" +
                          "precision mediump float;  \n" +
                          "out vec4 FragColor;\n" +
                          "\n"+
                          "void main()\n" +
                          "{\n" +
                                "FragColor = vec4(1.0f, 0.5f, 0.2f, 0.5f);\n" +
                          "}",
                    Arguments = Enumerable.Empty<string>().ToArray()
                }
            }
        };

        public ShaderService()
        {
            _shaderPrograms = new Dictionary<ShaderUsage, int>();
        }

        public int GetShader(ShaderUsage usage)
        {
            if(!_shaderPrograms.ContainsKey(usage))
            {
                CreateShaderProgram(usage);
            }

            return _shaderPrograms[usage];
        }

        public void Dispose()
        {
            foreach (var shaderProgramHandle in _shaderPrograms.Values)
            {
                GL.DeleteProgram(shaderProgramHandle);
            }
        }

        private void CreateShaderProgram(ShaderUsage usage)
        {
            var sources = _shaderSources[usage];

            _shaderPrograms[usage] = CreateShaderProgram(sources.VertexShaderSource, sources.FragmentShader, sources.Arguments);
        }

        private int CreateShaderProgram(string vertexShaderSourceCode, string fragmentShaderSourceCode, ICollection<string> argumentsSetup)
        {
            // Vertex Shader
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSourceCode);
            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexShaderErrorCode);
            if (vertexShaderErrorCode != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new ShaderCreatingException(infoLog);
            }

            // Fragment Shader
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSourceCode);
            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragmentShaderErrorCode);
            if (fragmentShaderErrorCode != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new ShaderCreatingException(infoLog);
            }

            // Shader Program
            var programHandle = GL.CreateProgram();

            GL.AttachShader(programHandle, vertexShader);
            GL.AttachShader(programHandle, fragmentShader);

            GL.LinkProgram(programHandle);

            GL.GetProgram(programHandle, GetProgramParameterName.LinkStatus, out int shaderProgramErrorCode);
            if (shaderProgramErrorCode != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(programHandle);
                throw new ShaderCreatingException(infoLog);
            }

            // Clean Up
            GL.DetachShader(programHandle, vertexShader);
            GL.DetachShader(programHandle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return programHandle;
        }
    }
}

