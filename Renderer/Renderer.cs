
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
#if TIZEN
using OpenTK;
using OpenTK.Input;
#else
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
#endif
using TizenGameEngine.Renderer.Cameras;
using TizenGameEngine.Renderer.Models;
using TizenGameEngine.Renderer.RenderableObjects;
using TizenGameEngine.Renderer.Services;
using TizenGameEngine.LevelLoader;
using TizenGameEngine.LevelLoader.Models;

namespace TizenGameEngine.Renderer
{
    public class Renderer
    {
        private readonly ICollection<IRenderableObject> _renderableObjects;

        private readonly IShaderService _shaderService;
        private readonly ILevelLoader _levelLoader;

        private readonly ReferenceContainer<Matrix4> _perspective;

        private readonly float _ratio;

        private string _resourcesPath;

        private BaseCamera _activeCamera;

        // Rotation angle
        private float angleX = 45.0f;

        public Renderer(IShaderService shaderService, ILevelLoader levelLoader, float aspectRatio, string resourcesPath)
		{
			_renderableObjects = new List<IRenderableObject>();

			_shaderService = shaderService;
            _levelLoader = levelLoader;

			_perspective = new ReferenceContainer<Matrix4>();

			_ratio = aspectRatio;
			_resourcesPath = resourcesPath;
		}

		public void Load()
        {
            GL.ClearColor(Color4.Gray);
            GL.Enable(EnableCap.DepthTest);

            LoadLevel(_resourcesPath + "testLevel.tlf");
        }

        public void UseCamera()
        {
            _activeCamera = new RemoteCamera(_perspective, _ratio);
            _activeCamera.UpdateView();
        }

        float a = 0;
        public void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _activeCamera.UpdateView();

            foreach (var renderableObject in _renderableObjects)
            {
                renderableObject.Rotate(a += 0.5F, 0, 0);
                renderableObject.Draw();
            }
        }

        public void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            _activeCamera.OnKeyDown(e);

            foreach (var obj in _renderableObjects)
            {
                obj.RecalculateMatrix();
            }
        }

        public void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            _activeCamera.OnKeyUp(e);
        }

        private void LoadLevel(string path)
        {
            var level = _levelLoader.LoadFile(path);

            LoadChildren(level.Children);
        }

        private void LoadChildren(ICollection<LevelLoader.Models.Object> objects)
        {
            foreach (var item in objects)
            {
                switch (item.Type)
                {
                    case LevelLoader.Models.ObjectType.OBJ_MESH:
                        var castedModel = (ObjMesh)item;

                        var mesh = new NObjMeshRenderableObject(_resourcesPath + castedModel.GeometryPath, _resourcesPath + castedModel.Textures.First(), _perspective, _shaderService.GetShader(ShaderUsage.CUBE));
                        mesh.Load();
                        mesh.Move(castedModel.Position.X, castedModel.Position.Y, castedModel.Position.Z);
                        _renderableObjects.Add(mesh);
                        break;

                }
            }
        }
    }
}

