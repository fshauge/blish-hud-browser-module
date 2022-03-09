using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace WikiModule
{
    public unsafe class BrowserControl : Control
    {
        private static readonly MouseHandler Mouse = GameService.Input.Mouse;
        private static readonly KeyboardHandler Keyboard = GameService.Input.Keyboard;

        private Browser _browser;
        private int[] _pixels;
        private Texture2D _texture;
        private bool _focused;

        public BrowserControl()
        {
            Graphics.QueueMainThreadRender(graphicsDevice =>
            {
                _browser = new Browser();
            });

            Mouse.LeftMouseButtonPressed += OnLeftMouseButtonPressed;
            Mouse.LeftMouseButtonReleased += OnLeftMouseButtonReleased;
            Mouse.MouseMoved += OnMouseMoved;
            Mouse.MouseWheelScrolled += OnMouseWheelScrolled;
            Keyboard.KeyPressed += OnKeyPressed;
            Keyboard.KeyReleased += OnKeyReleased;
        }

        public void LoadUrl(string url)
        {
            Graphics.QueueMainThreadRender(graphicsDevice =>
            {
                _browser.LoadUrl(url);
                _browser.Focus();
            });
        }

        public void Focus()
        {
            Graphics.QueueMainThreadRender(graphicsDevice =>
            {
                _browser.Focus();
            });

            Keyboard.SetTextInputListner(OnTextInput);
            _focused = true;
        }

        public void Unfocus()
        {
            Graphics.QueueMainThreadRender(graphicsDevice =>
            {
                _browser.Unfocus();
            });

            Keyboard.UnsetTextInputListner(OnTextInput);
            _focused = false;
        }

        protected override void OnResized(ResizedEventArgs e)
        {
            Graphics.QueueMainThreadRender(graphicsDevice =>
            {
                _browser.Resize(Width, Height);
                _pixels = new int[Width * Height];
                _texture?.Dispose();
                _texture = new Texture2D(graphicsDevice, Width, Height, false, SurfaceFormat.Bgra32);
            });

            base.OnResized(e);
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            // Resources may not have been initialized yet
            if (_browser == null || _pixels == null || _texture == null)
            {
                return;
            }

            _browser.Update();
            _browser.Render();

            _browser.LockPixelsIfDirty(pixels =>
            {
                // Texture2D has no API for copying data directly from unmanaged memory
                Marshal.Copy(pixels, _pixels, 0, _pixels.Length);
                _texture.SetData(_pixels);
            });

            spriteBatch.DrawOnCtrl(this, _texture, bounds);
        }

        protected override void DisposeControl()
        {
            _texture?.Dispose();

            Graphics.QueueMainThreadRender(graphicsDevice =>
            {
                _browser.Dispose();
            });

            Mouse.LeftMouseButtonPressed -= OnLeftMouseButtonPressed;
            Mouse.LeftMouseButtonReleased -= OnLeftMouseButtonReleased;
            Mouse.MouseMoved -= OnMouseMoved;
            Mouse.MouseWheelScrolled -= OnMouseWheelScrolled;
            Keyboard.KeyPressed -= OnKeyPressed;
            Keyboard.KeyReleased -= OnKeyReleased;

            base.DisposeControl();
        }

        private void OnLeftMouseButtonPressed(object sender, MouseEventArgs e)
        {
            if (_mouseOver && _enabled)
            {
                Focus();
            }
            else
            {
                Unfocus();
            }

            if (!_focused)
            {
                return;
            }

            var mousePosition = Mouse.Position - AbsoluteBounds.Location;
            _browser?.FireMouseDownEvent(mousePosition.X, mousePosition.Y);
        }

        private void OnLeftMouseButtonReleased(object sender, MouseEventArgs e)
        {
            if (!_focused)
            {
                return;
            }

            var mousePosition = Mouse.Position - AbsoluteBounds.Location;
            _browser?.FireMouseUpEvent(mousePosition.X, mousePosition.Y);
        }

        private void OnMouseMoved(object sender, MouseEventArgs e)
        {
            if (!_focused)
            {
                return;
            }

            var mousePosition = Mouse.Position - AbsoluteBounds.Location;
            _browser?.FireMouseMovedEvent(mousePosition.X, mousePosition.Y);
        }

        private void OnMouseWheelScrolled(object sender, MouseEventArgs e)
        {
            if (!AbsoluteBounds.Contains(Mouse.Position))
            {
                return;
            }

            var deltaX = Mouse.State.HorizontalScrollWheelValue;
            var deltaY = Mouse.State.ScrollWheelValue;
            _browser?.FireScrollEvent(deltaX, deltaY);
        }

        private void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            if (!_focused)
            {
                return;
            }

            _browser.FireKeyDownEvent((uint)e.Key);
        }
        
        private void OnKeyReleased(object sender, KeyboardEventArgs e)
        {
            if (!_focused)
            {
                return;
            }

            _browser.FireKeyUpEvent((uint)e.Key);
        }

        private void OnTextInput(string input)
        {
            // We don't actually care about the input
        }
    }
}
