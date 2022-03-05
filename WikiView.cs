using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;

namespace WikiModule
{
    public class WikiView : View
    {
        protected override void Build(Container buildPanel)
        {
            var browser = new BrowserControl()
            {
                Parent = buildPanel,
                Size = buildPanel.Size,
                Url = "https://wiki.guildwars2.com/wiki/Main_Page"
            };

            buildPanel.AddChild(browser);
            base.Build(buildPanel);
        }
    }
}
