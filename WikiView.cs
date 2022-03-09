using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;

namespace WikiModule
{
    public class WikiView : View
    {
        private BrowserControl _browser;

        protected override void Build(Container buildPanel)
        {
            _browser = new BrowserControl()
            {
                Parent = buildPanel,
                Size = buildPanel.Size
            };

            _browser.LoadUrl("https://wiki.guildwars2.com/wiki/Main_Page");
            _browser.Focus();
            buildPanel.AddChild(_browser);
            base.Build(buildPanel);
        }

        protected override void Unload()
        {
            _browser?.Dispose();
            _browser = null;
            base.Unload();
        }
    }
}
