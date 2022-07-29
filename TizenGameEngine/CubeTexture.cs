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

        // Rotation angle
        float angleX = 45.0f, angleY = 0f;

        /// <summary>
        /// This well be called when key pressed down
        /// </summary>
        /// <param name="sender">Key instance</param>
        /// <param name="e">Key's args</param>
        public void OnKeyEvent(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                //_renderer.Rotate(-80f, 0f, ref angleX, ref angleY, ref _renderer.perspective, ref _renderer.modelview, ref _renderer.mvpMatrix, 1);
            }

            else if (e.Key == Key.Left)
            {
                //_renderer.Rotate(80f, 0f, ref angleX, ref angleY, ref _renderer.perspective, ref _renderer.modelview, ref _renderer.mvpMatrix, 1);

            }

            else if (e.Key == Key.Escape)
            {
                Exit();
            }

            Draw();
        }

        /// <summary>
        /// This gets called when the TizenGameApplication has been created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            _renderer = new Renderer(DirectoryInfo, Window.Width, Window.Height);

            Window.RenderFrame += OnRenderFrame;
            Window.MouseMove += (sender, e) =>
            {
                if (e.Mouse[MouseButton.Left])
                {
                    float x = (float)(e.XDelta);
                    float y = (float)(e.YDelta);

                    //_renderer.Rotate(-x, -y, ref angleX, ref angleY, ref _renderer.perspective, ref _renderer.modelview, ref _renderer.mvpMatrix, 1);
                    Draw();
                }
            };
            Window.KeyDown += OnKeyEvent;

            _renderer.Load();
        }


        void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _renderer.Draw();

            Window.SwapBuffers();
        }

        /// <summary>
        /// This well be called when the application is resumed
        /// </summary>
        protected override void OnResume()
        {
            Window.MakeCurrent();
            // Adjust the viewport based on geometry changes,
            // such as screen rotation
            GL.Viewport(0, 0, Window.Width, Window.Height);

            //_renderer.Rotate(0.8f, 0.8f, ref angleX, ref angleY, ref _renderer.perspective, ref _renderer.modelview, ref _renderer.mvpMatrix, 1);
            Draw();
        }
        /// <summary>
        /// Called when it is time to render the next frame. 
        /// </summary>
        /// <param name="oe">object</param>
        /// <param name="e">Frame Event</param>
        protected void OnRenderFrame(object oe, FrameEventArgs e)
        {
            angleX += 1;
            _renderer.Rotate(ref angleX, ref angleY, ref _renderer.perspective, ref _renderer.modelview, ref _renderer.mvpMatrix, 1);
            Draw();
        }
        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">The main arguments </param>
        static async Task Main(string[] args)
        {
            var app = new CubeByMultProgramView() { GLMajor = 2, GLMinor = 0 };
            app.Run(args);

            await Task.Delay(4000);
        }
    }

}