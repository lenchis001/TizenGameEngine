﻿
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Input;
using OpenTK.Platform;
using Tizen.Applications;
using TizenGameEngine.Renderer.Cameras;
using TizenGameEngine.Renderer.Common;
using TizenGameEngine.Renderer.Models;
using TizenGameEngine.Renderer.RenderableObjects;

namespace TizenGameEngine.Renderer
{
    public class Renderer
    {
        private readonly ICollection<IRenderableObject> _renderableObjects;

        private readonly DirectoryInfo _directoryInfo;
        private readonly IGameWindow _gameWindow;

        private readonly ReferenceContainer<Matrix4> _perspective;

        private BaseCamera _activeCamera;

        // Rotation angle
        private float angleX = 45.0f;

        public Renderer(DirectoryInfo directoryInfo, IGameWindow gameWindow)
        {
            _renderableObjects = new List<IRenderableObject>();

            _directoryInfo = directoryInfo;
            _gameWindow = gameWindow;

            _perspective = new ReferenceContainer<Matrix4>();
        }

        public void Load()
        {
            GL.ClearColor(Color4.Gray);
            GL.Enable(EnableCap.DepthTest);

            var cube = new CubeRenderableObject(_directoryInfo, _perspective);
            cube.Load();
            cube.Move(-1, 0, -3);
            _renderableObjects.Add(cube);

            var cube2 = new CubeRenderableObject(_directoryInfo, _perspective);
            cube2.Load();
            cube2.Move(1, 0, -3);
            _renderableObjects.Add(cube2);
        }

        public void SubscribeToEvents()
        {
            _gameWindow.RenderFrame += _OnRenderFrame;
            _gameWindow.KeyDown += _OnKeyDown;
        }

        public void UnsubscribeFromEvents()
        {
            _gameWindow.RenderFrame -= _OnRenderFrame;
            _gameWindow.KeyDown -= _OnKeyDown;
        }

        public void UseCamera()
        {
            _activeCamera = new RemoteCamera(_perspective, (float)_gameWindow.Width / _gameWindow.Height);
            _activeCamera.UpdateView();
        }

        private void _OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var renderableObject in _renderableObjects)
            {
                renderableObject.Rotate(angleX, 0, 0);
                renderableObject.Draw();
            }

            _gameWindow.SwapBuffers();
        }

        private void _OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            _activeCamera.OnKeyDown(e);
        }
    }
}

