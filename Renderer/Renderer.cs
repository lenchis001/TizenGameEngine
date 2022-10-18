
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
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

namespace TizenGameEngine.Renderer
{
    public class Renderer
    {
        private readonly ICollection<IRenderableObject> _renderableObjects;

        private readonly IShaderService _shaderService;

        private readonly ReferenceContainer<Matrix4> _perspective;

        private readonly float _ratio;

        private string _resourcesPath;

        private BaseCamera _activeCamera;

        // Rotation angle
        private float angleX = 45.0f;

        public Renderer(IShaderService shaderService, float aspectRatio, string resourcesPath)
		{
			_renderableObjects = new List<IRenderableObject>();

			_shaderService = shaderService;

			_perspective = new ReferenceContainer<Matrix4>();

			_ratio = aspectRatio;
			_resourcesPath = resourcesPath;
		}

		public void Load()
        {
            GL.ClearColor(Color4.Gray);
            GL.Enable(EnableCap.DepthTest);

			var cube = new CubeRenderableObject(_resourcesPath, _perspective, _shaderService.GetShader(ShaderUsage.CUBE));
			cube.Load();
			cube.Move(-1, 0, -3);
			_renderableObjects.Add(cube);

			//var cube2 = new NObjMeshRenderableObject(_directoryInfo, _perspective, _shaderService.GetShader(ShaderUsage.CUBE));
			//cube2.Load();
			//cube2.Move(0, 0, -7);
			//_renderableObjects.Add(cube2);

			//var mesh = new ObjMeshRenderableObject(_directoryInfo, _perspective, _shaderService.GetShader(ShaderUsage.MESH));
			//mesh.Load();
			//cube2.Move(-1, 0, -3);
			//_renderableObjects.Add(mesh);
		}

        public void UseCamera()
        {
            _activeCamera = new RemoteCamera(_perspective, _ratio);
            _activeCamera.UpdateView();
        }

        public void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var renderableObject in _renderableObjects)
            {
                renderableObject.Draw();
            }
        }

        float a = 0;
        private void _OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            //_activeCamera.OnKeyDown(e);
            _renderableObjects.First().Rotate(0, a++, 0);
            //foreach (var obj in _renderableObjects)
            //{
            //    obj.RecalculateMatrix();
            //}
        }
    }
}

