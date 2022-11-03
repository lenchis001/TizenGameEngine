using System;
#if TIZEN
using OpenTK;
using OpenTK.Input;
#else
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
#endif
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;

namespace TizenGameEngine.Renderer.Cameras
{
    public class RemoteCamera: BaseCamera
    {
        float vertical = 0;
        float horizontal = 0;
        float _ratio;
        _MoveMode _moveMode;

        public RemoteCamera(ReferenceContainer<Matrix4> perspective, float ratio) : base(perspective) {
            _ratio = ratio;
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
#if TIZEN
                case Key.Left:
#else
                case Keys.Left:
#endif
                    _moveMode = _MoveMode.LEFT;
                    break;
#if TIZEN
                case Key.Right:
#else
                case Keys.Right:
#endif
                    _moveMode = _MoveMode.RIGHT;
                    break;

#if TIZEN
                case Key.Up:
#else
                case Keys.Up:
#endif
                    _moveMode = _MoveMode.FORWARD;
                    break;
#if TIZEN
                case Key.Down:
#else
                case Keys.Down:
#endif
                    _moveMode = _MoveMode.BACKWARD;
                    break;
#if TIZEN
                case Key.Plus:
#else
                case Keys.W:
#endif
                    _moveMode = _MoveMode.FORWARD;
                    break;
#if TIZEN
                case Key.Minus:
#else
                case Keys.S:
#endif
                    _moveMode = _MoveMode.BACKWARD;
                    break;
            }
        }

        public override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            _moveMode = _MoveMode.NO_MOVE;
        }

        public override void UpdateView()
        {
            if(_moveMode==_MoveMode.NO_MOVE)
            {
                return;
            }

            UpdateNavigation();

            MatrixState.EsMatrixLoadIdentity(ref _perspective.Value);
            MatrixState.EsPerspective(ref _perspective.Value, 40.0f, _ratio, 1.0f, 20.0f);
            MatrixState.EsRotate(ref _perspective.Value, vertical, 0, 1, 0);
            MatrixState.EsRotate(ref _perspective.Value, -6F, 1, 0, 0);
            MatrixState.EsTranslate(ref _perspective.Value, 0, 0, horizontal);
        }

        private void UpdateNavigation()
        {
            switch (_moveMode)
            {
                case _MoveMode.LEFT:
                    vertical -= 0.1F;
                    break;
                case _MoveMode.RIGHT:
                    vertical += 0.1F;
                    break;
                case _MoveMode.BACKWARD:
                    horizontal -= 0.01F;
                    break;
                case _MoveMode.FORWARD:
                    horizontal += 0.01F;
                    break;
            }
        }
    }

    enum _MoveMode
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FORWARD,
        BACKWARD,
        NO_MOVE
    }
}

