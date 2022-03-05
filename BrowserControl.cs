using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using ImpromptuNinjas.UltralightSharp.Enums;
using ImpromptuNinjas.UltralightSharp.Safe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MouseEventType = ImpromptuNinjas.UltralightSharp.Enums.MouseEventType;

namespace WikiModule
{
    public unsafe class BrowserControl : Control
    {
        private static readonly MouseHandler Mouse = GameService.Input.Mouse;
        private static readonly KeyboardHandler Keyboard = GameService.Input.Keyboard;

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

            Mouse.LeftMouseButtonPressed += OnLeftMouseButtonPressed;
            Mouse.LeftMouseButtonReleased += OnLeftMouseButtonReleased;
            Mouse.MouseMoved += OnMouseMoved;
            Mouse.MouseWheelScrolled += OnMouseWheelScrolled;
            Keyboard.KeyPressed += OnKeyPressed;
            Keyboard.KeyReleased += OnKeyReleased;
        }

        private void OnLeftMouseButtonPressed(object sender, MouseEventArgs e)
        {
            var mousePosition = Mouse.Position - AbsoluteBounds.Location;
            _view?.FireMouseEvent(new MouseEvent(MouseEventType.MouseDown, mousePosition.X, mousePosition.Y, MouseButton.Left));
        }

        private void OnLeftMouseButtonReleased(object sender, MouseEventArgs e)
        {
            var mousePosition = Mouse.Position - AbsoluteBounds.Location;
            _view?.FireMouseEvent(new MouseEvent(MouseEventType.MouseUp, mousePosition.X, mousePosition.Y, MouseButton.Left));
        }

        private void OnMouseMoved(object sender, MouseEventArgs e)
        {
            var mousePosition = Mouse.Position - AbsoluteBounds.Location;
            _view?.FireMouseEvent(new MouseEvent(MouseEventType.MouseMoved, mousePosition.X, mousePosition.Y, MouseButton.None));
        }

        private void OnMouseWheelScrolled(object sender, MouseEventArgs e)
        {
            if (!AbsoluteBounds.Contains(Mouse.Position))
            {
                return;
            }

            var deltaX = Mouse.State.HorizontalScrollWheelValue;
            var deltaY = Mouse.State.ScrollWheelValue;
            _view?.FireScrollEvent(new ScrollEvent(ScrollEventType.ScrollByPixel, deltaX, deltaY));
        }

        private void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            _view?.FireKeyEvent(new KeyEvent(KeyEventType.RawKeyDown, (UIntPtr)e.Key, IntPtr.Zero, false));
            _view?.FireKeyEvent(new KeyEvent(KeyEventType.Char, (UIntPtr)e.Key.ToChar(), IntPtr.Zero, false));
        }

        private void OnKeyReleased(object sender, KeyboardEventArgs e)
        {
            _view?.FireKeyEvent(new KeyEvent(KeyEventType.KeyUp, (UIntPtr)e.Key, IntPtr.Zero, false));
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            if (!_initialized)
            {
                _renderer = new Renderer(_config);
                _view = new View(_renderer, (uint)bounds.Width, (uint)bounds.Height, false, new Session(null));
                _view.LoadUrl(Url);
                _view.Focus();
                _pixels = new int[bounds.Width * bounds.Height];
                _texture = new Texture2D(spriteBatch.GraphicsDevice, bounds.Width, bounds.Height, false, SurfaceFormat.Bgra32);
                _initialized = true;
            }

            _renderer.Update();
            _renderer.Render();

            var surface = _view.GetSurface();

            if (!surface.GetDirtyBounds().IsEmpty())
            {
                var bitmap = surface.GetBitmap();
                var pixels = bitmap.LockPixels();
                Marshal.Copy(pixels, _pixels, 0, _pixels.Length);
                bitmap.UnlockPixels();
                _texture.SetData(_pixels);
                surface.ClearDirtyBounds();
            }

            spriteBatch.DrawOnCtrl(this, _texture, bounds);
        }

        protected override void DisposeControl()
        {
            if (_initialized)
            {
                _view.Dispose();
                _renderer.Dispose();
            }

            Mouse.LeftMouseButtonPressed -= OnLeftMouseButtonPressed;
            Mouse.LeftMouseButtonReleased -= OnLeftMouseButtonReleased;
            Mouse.MouseMoved -= OnMouseMoved;
            Mouse.MouseWheelScrolled -= OnMouseWheelScrolled;
            Keyboard.KeyPressed -= OnKeyPressed;
            Keyboard.KeyReleased -= OnKeyReleased;

            _config.Dispose();
            base.DisposeControl();
        }
    }
}
