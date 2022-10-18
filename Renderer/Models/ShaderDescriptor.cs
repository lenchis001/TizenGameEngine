using System.Collections.Generic;

namespace TizenGameEngine.Renderer.Models
{
    public class ShaderDescriptor
    {
        public string VertexShaderSource { get; set; }

        public string FragmentShader { get; set; }

        public ICollection<string> Arguments { get; set; }
    }
}

