#if TIZEN
using OpenTK;
using OpenTK.Input;
#else
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
#endif
using TizenGameEngine.Renderer.Models;

namespace TizenGameEngine.Renderer.Cameras
{
    public abstract class BaseCamera
    {
        protected readonly ReferenceContainer<Matrix4> _perspective;

        public BaseCamera(ReferenceContainer<Matrix4> perspective)
        {
            _perspective = perspective;
        }

        public abstract void UpdateView();

        public abstract void OnKeyDown(KeyboardKeyEventArgs e);
        public abstract void OnKeyUp(KeyboardKeyEventArgs e);
    }
}

