using Microsoft.Xna.Framework;
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
    public class DgaCropParser : CropParser
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

        /// <inheritdoc/>
        /// <summary>
        /// Builds a dictionary of crops from the game files. Accesses the crops from the game files (@"Data\Crops) and parses them into a dictionary. Thanks to Almanac team for inspiration for this code.
        /// </summary>
        /// <returns> A dictionary of crops. </returns>
        public override Dictionary<string, Crop> BuildCrops()
        {
            Dictionary<string, List<ObjectPackData>> seeds = new();
            Dictionary<string, Crop> crops = new();

            foreach (ContentPack pack in DynamicGameAssets.Mod.GetPacks())
                foreach (CommonPackData item in pack.GetItems())
                {
                    if (!item.Enabled)
                        continue;
                    else if (item is ObjectPackData obj && obj.Plants != null)
                    {
                        if (seeds.TryGetValue(obj.Plants, out var crop))
                            crop.Add(obj);
                        else
                            seeds.Add(obj.Plants, new List<ObjectPackData> { obj });

                        Monitor.Log($"ObjectPackData: {obj.Plants}", LogLevel.Debug);
                    }
                }

            foreach (ContentPack pack in DynamicGameAssets.Mod.GetPacks())
                foreach (CommonPackData item in pack.GetItems())
                {
                    if (!item.Enabled)
                        continue;
                    else if (item is CropPackData crop)
                    {
                        crops.Add(crop.ID, this.ProcessCrop(crop, pack, seeds));
                    }
                }

            return crops;
        }

        private Crop ProcessCrop(CropPackData crop, ContentPack pack, Dictionary<string, List<ObjectPackData>> seedList)
        {
            Season[]? seasons = ParseSeasonConditions(crop.DynamicFields);
            Item? cropItem = null;
            Item[] seeds;
            List<int>? phases = new();
            bool isTrellisCrop = false;
            int return_idx = -1;
            int regrow = 0;
            CropPackData.PhaseData last = crop.Phases.Last();
            bool giant = false;
            IReflectedMethod? GetMultiTexture = Helper?.Reflection.GetMethod(pack, "GetMultiTexture");
            string id = $"{pack.GetManifest().UniqueID}/{crop.ID}";

            foreach (var phase in crop.Phases)
            {
                isTrellisCrop |= phase.Trellis;

                // Make a note of the last harvest result.
                if (phase.HarvestedDrops.Count > 0)
                {
                    var choices = phase.HarvestedDrops[0].Item;
                    if (choices.Count > 0)
                        cropItem = choices[0].Value.Create();
                }

                return_idx = phase.HarvestedNewPhase;

                // Zero length phases need not apply. Except the last one.
                if (phase.Length <= 0 && phase != last)
                    continue;

                // Still we don't *add* the last zero length phase. We
                if (phase.Length > 0)
                    phases.Add(phase.Length);
            }

            if (return_idx >= 0 && return_idx < phases.Count)
            {
                for (int i = return_idx; i < phases.Count; i++)
                    regrow += phases[i];
            }

            if (crop.GiantChance > 0 && crop.GiantTextureChoices != null)
            {
                TexturedRect? tex = GetMultiTexture?.Invoke<TexturedRect>(crop.GiantTextureChoices, 0, 48, 64);
                if (tex != null)
                {
                    giant = true;
                }
            }
            Monitor.Log($"Registering: {id}", LogLevel.Trace);

            if (seedList.TryGetValue(id, out var slist))
                seeds = slist.Select((x) => x.ToItem()).ToArray();
            else
                seeds = null;

            //dga sprite info
            Texture2D? texture;
            Rectangle? rect;
            object? Data = Helper?.Reflection.GetProperty<object>(cropItem, "Data", required: false)?.GetValue();
            if (Data != null)
            {
                object? Tex = Helper?.Reflection.GetMethod(Data, "GetTexture", required: false)?.Invoke<object>();
                texture = Helper?.Reflection.GetProperty<Texture2D>(Tex, "Texture", required: false)?.GetValue();
                rect = Helper?.Reflection.GetProperty<Rectangle?>(Tex, "Rect", required: false)?.GetValue();
            }
            else //// okay maybe the result is a vanilla item, let's try that
            {
                texture = Game1.objectSpriteSheet;
                rect = Game1.getSourceRectForStandardTileSheet(
                            Game1.objectSpriteSheet,
                            cropItem.ParentSheetIndex,
                            SObject.spriteSheetTileSize,
                            SObject.spriteSheetTileSize
                        );
            }
            int? seedPrice = seeds[0].salePrice();
            string seedId = $"{pack.GetManifest().UniqueID}/{seeds[0].Name}";
            seedPrice = SeedPrice(seedId, seedPrice);
            return new Crop
            (
                id,
                cropItem,
                cropItem.DisplayName ?? "Error",
                new(texture, rect ?? texture.Bounds),
                isTrellisCrop,
                giant,
                null,
                seeds,
                phases.ToArray(),
                regrow,
                crop.Type == CropPackData.CropType.Paddy,
                seasons,
                new double[] { last.HarvestedDrops[0].MaximumHarvestedQuantity, last.HarvestedDrops[0].MininumHarvestedQuantity, 0f, last.HarvestedDrops[0].ExtraQuantityChance },
                true,
                true,
                seedPrice
            );
        }

        private Season[]? ParseSeasonConditions(DynamicFieldData[] dynamicFields)
        {
            // TODO: Actually parse the conditions to see when
            // we can grow this crop.
            bool? spring = null;
            bool? summer = null;
            bool? fall = null;
            bool? winter = null;

            foreach (DynamicFieldData data in dynamicFields)
            {
                // We know it affects CanGrowNow. But what *is* the condition?
                foreach (var cond in data.Conditions)
                {
                    if (string.IsNullOrEmpty(cond.Key))
                        continue;

                    string[] bits = cond.Key.Split('|');
                    string? token = bits[0]?.Trim()?.ToLowerInvariant();
                    string[] choices;

                    bool cval;

                    if (bits.Length < 2)
                    {
                        // No contains=. Expect a list elsewhere.
                        cval = true;
                        choices = cond.Value.Split(',')
                                            .Select(x => x.Trim())
                                            .ToArray();
                    }
                    else
                    {
                        // We only support |contains=
                        string c = bits[1].Trim()
                                          .ToLowerInvariant();
                        if (!c.StartsWith("contains="))
                            return null;

                        cval = cond.Value?.Trim()?.ToLowerInvariant() == "true";
                        choices = c[9..].Split(',')
                                        .Select(x => x.Trim())
                                        .ToArray();
                    }

                    switch (token)
                    {
                        case "season":
                            if (choices.Contains("spring"))
                            {
                                if (spring.HasValue && spring.Value != cval)
                                    return null;
                                spring = cval;
                            }
                            if (choices.Contains("summer"))
                            {
                                if (summer.HasValue && summer.Value != cval)
                                    return null;
                                summer = cval;
                            }
                            if (choices.Contains("fall"))
                            {
                                if (fall.HasValue && fall.Value != cval)
                                    return null;
                                fall = cval;
                            }
                            if (choices.Contains("winter"))
                            {
                                if (winter.HasValue && winter.Value != cval)
                                    return null;
                                winter = cval;
                            }
                            break;

                        default:
                            // Unsupported condition!
                            return null;
                    }
                }
            }

            List<Season> seasons = new();
            if (spring ?? false)
                seasons.Add(Season.Spring);
            if (summer ?? false)
                seasons.Add(Season.Summer);
            if (fall ?? false)
                seasons.Add(Season.Fall);
            if (winter ?? false)
                seasons.Add(Season.Winter);

            return seasons.ToArray<Season>();
        }

        /// <summary>
        /// Gets the seed price for the given seed id. If the seed id is not in the seedPriceOverrides dictionary, the seed price is returned. If the seed id is in the seedPriceOverrides dictionary, the seed price override is returned.
        /// </summary>
        /// <param name="id"> The id of the seed. </param>
        /// <param name="seedPrice"> The current seed price of the crop. </param>
        /// <returns></returns>
        private int? SeedPrice(string id, int? seedPrice)
        {
            if (seedPriceOverrides.ContainsKey(id))
            {
                seedPrice = seedPriceOverrides[id];
            }
            return seedPrice;
        }
    }
}