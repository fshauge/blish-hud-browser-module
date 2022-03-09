using ImpromptuNinjas.UltralightSharp.Enums;
using ImpromptuNinjas.UltralightSharp.Safe;
using System;

namespace WikiModule
{
    public class Browser : IDisposable
    {
        private static readonly Blish_HUD.Logger Logger = Blish_HUD.Logger.GetLogger<Browser>();

        private static Config Config;
        private static Renderer Renderer;

        public static void Initialize(string resourcePath, string cachePath)
        {
            if (Config != null || Renderer != null)
            {
                return;
            }

            Ultralight.SetLogger(new Logger
            {
                LogMessage = (level, msg) =>
                {
                    switch (level)
                    {
                        case LogLevel.Info:
                            Logger.Info(msg);
                            break;
                        case LogLevel.Warning:
                            Logger.Warn(msg);
                            break;
                        case LogLevel.Error:
                            Logger.Error(msg);
                            break;
                    }
                }
            });

            AppCore.EnablePlatformFontLoader();

            Config = new Config();
            Config.SetResourcePath(resourcePath);
            Config.SetCachePath(cachePath);
            Config.SetUseGpuRenderer(false);
            Config.SetEnableImages(true);
            Config.SetEnableJavaScript(true);

            Renderer = new Renderer(Config);
        }

        private View _view;

        public unsafe Browser()
        {
            if (Config == null || Renderer == null)
            {
                throw new InvalidOperationException("Browser has not been initialized.");
            }

            _view = new View(Renderer, 0, 0, true, new Session(null));
        }

        public void Update()
        {
            Renderer.Update();
        }

        public void Render()
        {
            Renderer.Render();
        }

        public void LoadUrl(string url)
        {
            _view.LoadUrl(url);
        }

        public void Focus()
        {
            _view.Focus();
        }

        public void Unfocus()
        {
            _view.Unfocus();
        }

        public void FireMouseDownEvent(int x, int y)
        {
            _view.FireMouseEvent(new MouseEvent(MouseEventType.MouseDown, x, y, MouseButton.Left));
        }

        public void FireMouseUpEvent(int x, int y)
        {
            _view.FireMouseEvent(new MouseEvent(MouseEventType.MouseUp, x, y, MouseButton.Left));
        }
         
        public void FireMouseMovedEvent(int x, int y)
        {
            _view.FireMouseEvent(new MouseEvent(MouseEventType.MouseMoved, x, y, MouseButton.None));
        }

        public void FireScrollEvent(int deltaX, int deltaY)
        {
            _view.FireScrollEvent(new ScrollEvent(ScrollEventType.ScrollByPixel, deltaX, deltaY));
        }

        public void FireKeyDownEvent(uint key)
        {
            _view.FireKeyEvent(new KeyEvent(KeyEventType.RawKeyDown, (UIntPtr)key, IntPtr.Zero, false));

            if (Keyboard.TryGetChar(key, out var ch))
            {
                _view.FireKeyEvent(new KeyEvent(KeyEventType.Char, (UIntPtr)ch, IntPtr.Zero, false));
            }
        }

        public void FireKeyUpEvent(uint key)
        {
            _view.FireKeyEvent(new KeyEvent(KeyEventType.KeyUp, (UIntPtr)key, IntPtr.Zero, false));
        }

        public void Resize(int width, int height)
        {
            _view.Resize((uint)width, (uint)height);
        }

        public void LockPixelsIfDirty(Action<IntPtr> callback)
        {
            var surface = _view.GetSurface();

            if (!surface.GetDirtyBounds().IsEmpty())
            {
                var bitmap = surface.GetBitmap();
                bitmap.WithPixelsLocked(pixels => callback(pixels));
                surface.ClearDirtyBounds();
            }
        }

        public void Dispose()
        {
            _view.Dispose();
            _view = null;
        }
    }
}
