using System;
using System.IO;
using OpenTK;
using OpenTK.Windowing.Desktop;
using TizenGameEngine.Renderer;
using TizenGameEngine.Renderer.Services;
using OpenTK.Graphics.ES30;
using System.ComponentModel;
using OpenTK.Windowing.Common;
#if TIZEN
using OpenTK;
#else
using OpenTK.Mathematics;
#endif
using TizenGameEngine.LevelLoader.Models;

namespace BlankWindow
{
    sealed class Program : OpenTK.Windowing.Desktop.GameWindow
    {
        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _shaderService = new ShaderService();
        }

        private ContentRenderer _renderer;
        private readonly IShaderService _shaderService;

        protected override void OnLoad()
        {
            _renderer = new ContentRenderer(_shaderService, new TlfLoader(), this.ClientSize.X / this.ClientSize.Y, Path.Combine(Directory.GetCurrentDirectory(), "res"));
            _renderer.UseCamera();

            GL.Viewport(0, 0, this.ClientSize.X, this.ClientSize.Y);

            _renderer.Load();

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _renderer.RenderFrame();

            Context.SwapBuffers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _shaderService.Dispose();
        }

        [STAThread]
        public static void Main()
        {
            var gws = new GameWindowSettings();
            var nws = new NativeWindowSettings
            {
                Size = new Vector2i { X = 1920, Y = 1080 }
            };
            var program = new Program(gws, nws);
            program.Run();
        }
    }
}