/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ProfitCalculator.Utils;
using SObject = StardewValley.Object;
using Newtonsoft.Json.Linq;

using DynamicGameAssets;
using DynamicGameAssets.PackData;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using static StardewValley.Minigames.CraneGame;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

#nullable enable

namespace ProfitCalculator.main
{
    /// <summary>
    /// Parses the vanilla crops from the game files. Also parses crops from the ManualCrops.json file.
    /// </summary>
    public class DgaCropParser : ICropParser
    {
        private readonly Dictionary<string, int> seedPriceOverrides;

        /// <summary>
        /// Constructor for the VanillaCropParser class.
        /// </summary>
        /// <param name="name"> The name of the parser. Defaults to "VanillaCropParser". </param>
        public DgaCropParser(string name = "DGACropParser") : base(name)
        {
            seedPriceOverrides = Helper.ModContent.Load<Dictionary<string, int>>(Path.Combine("assets", "SeedPrices.json"));
        }
    }
}*/