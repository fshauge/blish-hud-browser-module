using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.InteropServices;
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
            var ultralightPath = DirectoriesManager.GetFullDirectoryPath("ultralight");
            var resourcePath = Path.Combine(ultralightPath, "resources");
            var cachePath = Path.Combine(ultralightPath, "cache");
            var libPath = Path.Combine(ultralightPath, "lib");

            ExtractUltralight(ultralightPath);
            LoadUltralight(libPath);

            Browser.Initialize(resourcePath, cachePath);

            _wikiTab = new WindowTab("Wiki", ContentsManager.GetTexture("textures/748852463700017253.png"));
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

        private void LoadUltralight(string libPath)
        {
            Kernel32.SetDllDirectory(libPath);

            if (Directory.Exists(libPath))
            {
                foreach (var library in Directory.EnumerateFiles(libPath, "*.dll"))
                {
                    if (Kernel32.LoadLibrary(library) == IntPtr.Zero)
                    {
                        throw Marshal.GetExceptionForHR(Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        private void ExtractUltralight(string path)
        {
            ExtractFile("lib/AppCore.dll", path);
            ExtractFile("lib/Ultralight.dll", path);
            ExtractFile("lib/UltralightCore.dll", path);
            ExtractFile("lib/WebCore.dll", path);
            ExtractFile("resources/cacert.pem", path);
            ExtractFile("resources/icudt67l.dat", path);
        }

        private void ExtractFile(string file, string output)
        {
            var path = Path.Combine(output, file);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (File.Exists(path))
            {
                // TODO: Check if hash matches
                return;
            }

            using (var stream = ContentsManager.GetFileStream(file))
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}
