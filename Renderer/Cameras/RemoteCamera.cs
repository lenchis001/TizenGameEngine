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
        float _leftRight = 0;
        float _forwardBackward = -25;
        float _ratio;
        _MoveMode _moveMode;

        public RemoteCamera(ReferenceContainer<Matrix4> perspective, float ratio) : base(perspective) {
            _ratio = ratio;

            _moveMode = _MoveMode.NO_MOVE;

            RecalculateMatrix();
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
#if TIZEN
            if (e.Keyboard.IsKeyDown(Key.Up))
            {
                return;
            }
#endif

            _moveMode = _MoveMode.NO_MOVE;
        }

        public override void UpdateView()
        {
            if(_moveMode==_MoveMode.NO_MOVE)
            {
                return;
            }

            UpdateNavigation();

            RecalculateMatrix();
        }

        private void UpdateNavigation()
        {
            switch (_moveMode)
            {
                case _MoveMode.LEFT:
                    _leftRight += 0.1F;
                    break;
                case _MoveMode.RIGHT:
                    _leftRight -= 0.1F;
                    break;
                case _MoveMode.BACKWARD:
                    _forwardBackward -= 0.1F;
                    break;
                case _MoveMode.FORWARD:
                    _forwardBackward += 0.1F;
                    break;
            }
        }

        private void RecalculateMatrix() {
            MatrixState.EsMatrixLoadIdentity(ref _perspective.Value);

            MatrixState.EsPerspective(ref _perspective.Value, 40.0f, _ratio, 1.0f, 200.0f);
            MatrixState.EsRotate(ref _perspective.Value, -20F, 1, 0, 0);

            MatrixState.EsTranslate(ref _perspective.Value, _leftRight, -10, _forwardBackward);
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

