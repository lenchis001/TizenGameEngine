using System;

namespace TizenGameEngine.Renderer.RenderableObjects
{
    public interface IRenderableObject: IDisposable
    {
        void Load();

        void Draw();

        void Move(float x, float y, float z);

        void Scale(float x, float y, float z);

        void Rotate(float x, float y, float z);

        void RecalculateMatrix();
    }
}

