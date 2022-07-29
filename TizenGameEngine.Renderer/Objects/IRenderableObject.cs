using System;
using OpenTK;
using TizenGameEngine.Renderer.Common;

namespace TizenGameEngine.Renderer.Objects
{
    public interface IRenderableObject
    {
        void Load();

        void Draw();

        void Move();

        void Scale();

        void Rotate(ref float anglex, ref float angley, int flag);
    }
}

