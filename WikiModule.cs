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

        private WindowTab _wikiTab;

        [ImportingConstructor]
        public WikiModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void Initialize()
        {
            var mainDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var resourcePath = Path.Combine(mainDirectory, "resources");
            var cachePath = DirectoriesManager.GetFullDirectoryPath("cache");

            Browser.Initialize(resourcePath, cachePath);

            _wikiTab = new WindowTab("Wiki", new AsyncTexture2D());
            GameService.Overlay.BlishHudWindow.AddTab(_wikiTab, () => new WikiView());
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
        }
    }
}
