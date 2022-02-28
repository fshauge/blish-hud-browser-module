using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImpromptuNinjas.UltralightSharp.Safe;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System;

namespace BrowserModule
{
    public class BrowserControl : Control
    {
        private Config _config;
        private Renderer _renderer;
        private View _view;
        private int[] _pixels;
        private bool _loading;

        public unsafe BrowserControl()
        {
            var mainDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var resourcePath = Path.Combine(mainDirectory, "resources");
            var tmpDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var cachePath = Path.Combine(tmpDirectory, "cache");
            // var assetsPath = Path.Combine(tmpDirectory, "assets");

            AppCore.EnablePlatformFontLoader();
            // AppCore.EnablePlatformFileSystem(assetsPath);

            _config = new Config();
            _config.SetResourcePath(resourcePath);
            _config.SetCachePath(cachePath);
            _config.SetUseGpuRenderer(false);
            _config.SetEnableImages(false);
            _config.SetEnableJavaScript(false);

            _renderer = new Renderer(_config);
            _view = new View(_renderer, (uint)Width, (uint)Height, false, new Session(null));

            _view.SetFinishLoadingCallback(
                (userData, caller, frameId, isMainFrame, url) =>
                {
                    _loading = false;
                },
                IntPtr.Zero
            );

            _view.SetFailLoadingCallback(
                (userData, caller, frameId, isMainFrame, url, description, errorDomain, errorCode) =>
                {
                    _loading = false;
                },
                IntPtr.Zero
            );

            _pixels = new int[Width * Height];
            _loading = false;
        }

        protected override unsafe void OnResized(ResizedEventArgs e)
        {
            // Calling _view.Resize instead would be better, but it crashes the renderer :/
            _view.Dispose();
            _view = new View(_renderer, (uint)Width, (uint)Height, false, new Session(null));
            _pixels = new int[Width * Height];
            base.OnResized(e);
        }

        protected override unsafe void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            while (_loading)
            {
                _renderer.Update();
                _renderer.Render();
                return;
            }

            // TODO: Use surface.GetDirtyRegion() to only update texture when the surface has changed

            var surface = _view.GetSurface();
            var bitmap = surface.GetBitmap();
            var pixels = bitmap.LockPixels();
            Marshal.Copy(pixels, _pixels, 0, _pixels.Length);
            bitmap.UnlockPixels();

            var texture = new Texture2D(spriteBatch.GraphicsDevice, Width, Height);
            texture.SetData(_pixels);

            spriteBatch.Draw(texture, bounds, Color.White);
        }

        protected override void DisposeControl()
        {
            _view.Dispose();
            _renderer.Dispose();
            _config.Dispose();
            base.DisposeControl();
        }

        public void Load(string url)
        {
            _loading = true;
            _view.LoadUrl(url);
            _view.Focus();
        }
    }
}
