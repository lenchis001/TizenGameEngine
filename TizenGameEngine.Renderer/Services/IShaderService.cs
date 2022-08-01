using System;
using TizenGameEngine.Renderer.Models;

namespace TizenGameEngine.Renderer.Services
{
    public interface IShaderService: IDisposable
    {
        int GetShader(ShaderUsage usage);
    }
}

