
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
        private float angleX = 45.0f, angleY = 0f;

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

            var cube = new CubeRenderableObject(_directoryInfo, ref _perspective);
            cube.Load();
            _renderableObjects.Add(cube);
        }

        public void Draw()
        {
            foreach (var renderableObject in _renderableObjects)
            {
                angleX += 1;
                renderableObject.Rotate(ref angleX, ref angleY, 1);
                renderableObject.Draw();
            }
        }
    }
}

