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
using OpenTK.Graphics.ES20;
using OpenTK.Input;
using OpenTK.Platform.Tizen;
using System.Threading.Tasks;
using TizenGameEngine.Renderer;

namespace CubeTexture
{
    class CubeByMultProgramView : TizenGameApplication
    {
        private Renderer _renderer;

        public void OnKeyEvent(object sender, KeyboardKeyEventArgs e)
        {
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _renderer = new Renderer(DirectoryInfo, Window.Width, Window.Height);

            Window.RenderFrame += OnRenderFrame;
            Window.KeyDown += OnKeyEvent;

            GL.Viewport(0, 0, Window.Width, Window.Height);

            _renderer.Load();
        }


        void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _renderer.Draw();

            Window.SwapBuffers();
        }

        protected override void OnResume()
        {
            Window.MakeCurrent();
            GL.Viewport(0, 0, Window.Width, Window.Height);

            Draw();
        }

        protected void OnRenderFrame(object oe, FrameEventArgs e)
        {
            Draw();
        }

        static async Task Main(string[] args)
        {
            var app = new CubeByMultProgramView() { GLMajor = 2, GLMinor = 0 };
            app.Run(args);

            await Task.Delay(4000);
        }
    }

}