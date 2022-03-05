using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using UltralightConfig = ImpromptuNinjas.UltralightSharp.Safe.Config;

namespace WikiModule
{
    public class WikiView : View
    {
        private UltralightConfig _config;

        public WikiView(UltralightConfig config)
        {
            _config = config;
        }

        protected override void Build(Container buildPanel)
        {
            var browser = new BrowserControl()
            {
                Parent = buildPanel,
                Size = buildPanel.Size,
                Config = _config,
                Url = "https://wiki.guildwars2.com/wiki/Main_Page"
            };

            buildPanel.AddChild(browser);
            base.Build(buildPanel);
        }
    }
}
