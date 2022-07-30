
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using Tizen.Applications;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Objects;

namespace TizenGameEngine.Renderer
{
    public class Renderer
    {
        private readonly ICollection<IRenderableObject> _renderableObjects;

        private DirectoryInfo _directoryInfo;
        private int _width, _height;

        private Matrix4 _perspective;

        // Rotation angle
        private float angleX = 45.0f;

        public Renderer(DirectoryInfo directoryInfo, int width, int height)
        {
            _renderableObjects = new List<IRenderableObject>();

            _directoryInfo = directoryInfo;

            _width = width;
            _height = height;

            float ratio = (float)_width / _height;
            MatrixState.EsMatrixLoadIdentity(ref _perspective);
            MatrixState.EsPerspective(ref _perspective, 60.0f, ratio, 1.0f, 20.0f);
        }

        public void Load()
        {
            GL.ClearColor(Color4.Gray);
            GL.Enable(EnableCap.DepthTest);


            MatrixState.EsRotate(ref _perspective, 20f, 1, 0, 0);

            var cube = new CubeRenderableObject(_directoryInfo, ref _perspective);
            cube.Load();
            cube.Move(-1, 2, -3);
            _renderableObjects.Add(cube);

            var cube2 = new CubeRenderableObject(_directoryInfo, ref _perspective);
            cube2.Load();
            cube2.Move(1, 2, -3);
            _renderableObjects.Add(cube2);
        }

        public void Draw()
        {
            angleX += 1;
            if (angleX >= 360.0f)
            {
                angleX -= 360.0f;
            }

            foreach (var renderableObject in _renderableObjects)
            {
                renderableObject.Rotate(angleX, 0, 0);
                renderableObject.Draw();
            }
        }
    }
}

