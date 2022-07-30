using System;
using OpenTK;
using OpenTK.Input;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;

namespace TizenGameEngine.Renderer.Cameras
{
    public class RemoteCamera: BaseCamera
    {
        float vertical = 0;
        float horizontal = 0;
        float _ratio;

        public RemoteCamera(ReferenceContainer<Matrix4> perspective, float ratio) : base(perspective) {
            _ratio = ratio;
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    vertical += 0.5f;
                    UpdateView();
                    break;
                case Key.Right:
                    vertical -= 0.5f;
                    UpdateView();
                    break;

                case Key.Up:
                    horizontal += 0.1f;
                    UpdateView();
                    break;
                case Key.Down:
                    horizontal -= 0.1f;
                    UpdateView();
                    break;
            }
        }

        public override void UpdateView()
        {
            MatrixState.EsMatrixLoadIdentity(ref _perspective.Value);
            MatrixState.EsPerspective(ref _perspective.Value, 40.0f, _ratio, 1.0f, 20.0f);
            MatrixState.EsRotate(ref _perspective.Value, vertical, 0, 1, 0);
            MatrixState.EsTranslate(ref _perspective.Value, 0, 0, horizontal);
        }
    }
}

