using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ProfitCalculator
{
    public class ModConfig
    {
        public SButton HotKey { get; set; }
        public int ToolTipDelay { get; set; }

        public ModConfig()
        {
            HotKey = SButton.F8;
            ToolTipDelay = 30;
        }
    }
}