using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Ultralight = ImpromptuNinjas.UltralightSharp.Safe.Ultralight;
using UltralightAppCore = ImpromptuNinjas.UltralightSharp.Safe.AppCore;
using UltralightConfig = ImpromptuNinjas.UltralightSharp.Safe.Config;
using UltralightLogger = ImpromptuNinjas.UltralightSharp.Safe.Logger;
using UltralightLogLevel = ImpromptuNinjas.UltralightSharp.Enums.LogLevel;

namespace WikiModule
{
    [Export(typeof(Module))]
    public class WikiModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<WikiModule>();

        #region Service Managers
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;
        #endregion

        private UltralightConfig _config;
        private WindowTab _wikiTab;

        [ImportingConstructor]
        public WikiModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void Initialize()
        {
            Ultralight.SetLogger(new UltralightLogger
            {
                LogMessage = (level, msg) =>
                {
                    switch (level)
                    {
                        case UltralightLogLevel.Info:
                            Logger.Info(msg);
                            break;
                        case UltralightLogLevel.Warning:
                            Logger.Warn(msg);
                            break;
                        case UltralightLogLevel.Error:
                            Logger.Error(msg);
                            break;
                    }
                }
            });

            UltralightAppCore.EnablePlatformFontLoader();

            var mainDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var resourcePath = Path.Combine(mainDirectory, "resources");
            var tmpDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var cachePath = Path.Combine(tmpDirectory, "cache");

            _config = new UltralightConfig();
            _config.SetResourcePath(resourcePath);
            _config.SetCachePath(cachePath);
            _config.SetUseGpuRenderer(false);
            _config.SetEnableImages(true);
            _config.SetEnableJavaScript(true);

            _wikiTab = new WindowTab("Wiki", new AsyncTexture2D());
            GameService.Overlay.BlishHudWindow.AddTab(_wikiTab, () => new WikiView(_config));
        }

        protected override Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {
        }

        protected override void Unload()
        {
            GameService.Overlay.BlishHudWindow.RemoveTab(_wikiTab);
            _config.Dispose();
        }
    }
}
