using System;
using OpenTK;
using TizenGameEngine.Renderer.Common;

namespace TizenGameEngine.Renderer.Objects
{
    public interface IRenderableObject
    {
        void Load();

        void Draw();

        void Move(float x, float y, float z);

        void Scale(float x, float y, float z);

        void Rotate(float x, float y, float z);
    }
}

