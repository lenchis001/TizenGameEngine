/*
 * Copyright (c) 2017 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using OpenTK;
using OpenTK.Graphics.ES30;
using OpenTK.Input;
using OpenTK.Platform.Tizen;
using System;
using System.Threading.Tasks;
using TizenGameEngine.Logger;
using TizenGameEngine.Renderer;
using TizenGameEngine.Renderer.Services;

namespace CubeTexture
{
    class TizenGameEngineApplication : TizenGameApplication
    {
        private Renderer _renderer;
        private readonly IShaderService _shaderService;

        public TizenGameEngineApplication()
        {
            _shaderService = new ShaderService();
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _renderer = new Renderer(DirectoryInfo, Window, _shaderService);
            _renderer.UseCamera();
            _renderer.SubscribeToEvents();

            GL.Viewport(0, 0, Window.Width, Window.Height);

            _renderer.Load();
        }

        protected override void OnResume()
        {
            Window.MakeCurrent();
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }

        protected override void OnTerminate()
        {
            _shaderService.Dispose();
            _renderer.UnsubscribeFromEvents();

            base.OnTerminate();
        }

        static void Main(string[] args)
        {
            try
            {
                var app = new TizenGameEngineApplication() { GLMajor = 3, GLMinor = 0 };
                app.Run(args);
            }
            catch(Exception e)
            {
                WebLogger.LogAsync("An error ocured.");
                WebLogger.LogAsync(e.Message);
                WebLogger.LogAsync(e.StackTrace);
            }
        }
    }

}