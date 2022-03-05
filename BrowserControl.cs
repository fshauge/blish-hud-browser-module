using Blish_HUD;
using Blish_HUD.Controls;
using ImpromptuNinjas.UltralightSharp.Safe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace BrowserModule
{
    public class BrowserControl : Control
    {
        public string Url { get; set; }

        private Config _config;
        private bool _initialized;
        private Renderer _renderer;
        private View _view;
        private int[] _pixels;
        private Texture2D _texture;

        public BrowserControl()
        {
            AppCore.EnablePlatformFontLoader();

            var mainDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var resourcePath = Path.Combine(mainDirectory, "resources");
            var tmpDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var cachePath = Path.Combine(tmpDirectory, "cache");

            _config = new Config();
            _config.SetResourcePath(resourcePath);
            _config.SetCachePath(cachePath);
            _config.SetUseGpuRenderer(false);
            _config.SetEnableImages(true);
            _config.SetEnableJavaScript(true);

            _initialized = false;
        }

        protected override unsafe void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            if (!_initialized)
            {
                _renderer = new Renderer(_config);
                _view = new View(_renderer, (uint)bounds.Width, (uint)bounds.Height, false, new Session(null));
                _view.LoadUrl(Url);
                _pixels = new int[bounds.Width * bounds.Height];
                _texture = new Texture2D(spriteBatch.GraphicsDevice, bounds.Width, bounds.Height, false, SurfaceFormat.Bgra32);
                _initialized = true;
            }

            _renderer.Update();
            _renderer.Render();

            // TODO: Use surface.GetDirtyRegion() to only update texture when the surface has changed

            var surface = _view.GetSurface();
            var bitmap = surface.GetBitmap();
            var pixels = bitmap.LockPixels();
            Marshal.Copy(pixels, _pixels, 0, _pixels.Length);
            bitmap.UnlockPixels();
            _texture.SetData(_pixels);
            spriteBatch.DrawOnCtrl(this, _texture, bounds);
        }

        protected override void DisposeControl()
        {
            _view.Dispose();
            _renderer.Dispose();
            _config.Dispose();
            base.DisposeControl();
        }
    }
}
