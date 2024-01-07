using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace MobileProfitCalculator
{
    public class ModConfig
    {
        public SButton HotKey { get; set; }

        public bool EnableMobileApp { get; set; }
        public Color AppBackgroundColor { get; set; }
        public Color InputColor { get; set; }
        public Color PrevInputColor { get; set; }
        public int AppMarginX { get; set; }
        public int AppMarginY { get; set; }

        public ModConfig()
        {
            HotKey = SButton.F8;
            EnableMobileApp = false;
            AppBackgroundColor = new Color(255, 200, 120);
            InputColor = Color.Black;
            PrevInputColor = new Color(100, 100, 100);
            AppMarginX = 5;
            AppMarginY = 5;
        }
    }
}