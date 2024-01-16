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

#nullable enable

namespace ProfitCalculator.main
{
    /// <summary>
    /// Parses the vanilla crops from the game files. Also parses crops from the ManualCrops.json file.
    /// </summary>
    public class VanillaCropParser : CropParser
    {
        private readonly Dictionary<int, int> seedPriceOverrides;

        /// <summary>
        /// Constructor for the VanillaCropParser class.
        /// </summary>
        /// <param name="name"> The name of the parser. Defaults to "VanillaCropParser". </param>
        public VanillaCropParser(string name = "VanillaCropParser") : base(name)
        {
            seedPriceOverrides = Helper.ModContent.Load<Dictionary<int, int>>(Path.Combine("assets", "SeedPrices.json"));
        }

        /// <inheritdoc/>
        /// <summary>
        /// Builds a dictionary of crops from the game files. Accesses the crops from the game files (@"Data\Crops) and parses them into a dictionary.
        /// </summary>
        /// <returns> A dictionary of crops. </returns>
        public override Dictionary<string, Crop> BuildCrops()
        {
            //read crop data from game files
            //add crops to list
            Dictionary<string, Crop> Crops = new();
            Dictionary<int, string> crops = Game1.content.Load<Dictionary<int, string>>(@"Data\Crops");
            foreach (KeyValuePair<int, string> crop in crops)
            {
                Crop? cropToAdd = BuildCrop(crop.Value.Split('/'), crop.Key);
                if (cropToAdd != null && !Crops.ContainsKey(crop.Key.ToString()))
                    Crops.Add(cropToAdd.Id.ToString(), cropToAdd);
            }
            foreach (KeyValuePair<string, Crop> crop in this.BuildCropsFromJson())
            {
                if (!Crops.ContainsKey(crop.Key))
                {
                    Crops.Add(crop.Key, crop.Value);
                }
            }
            return Crops;
        }

        /// <summary>
        /// Builds a crop from the given data. The data is split by the '/' character. The data is then parsed into a crop. The crop is then returned.  Thanks to Klhoe Leclair for this code.
        ///
        /// </summary>
        /// <param name="cropData"> The data of the crop. </param>
        /// <param name="id"> The id of the crop. </param>
        /// <returns> The crop that was built. </returns>
        private Crop? BuildCrop(string[] cropData, int id)
        {
            //print crop data.
            //stages of the crop, we shouldnt care about this
            string[] phases = cropData[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            //seasons the crop grows in, we care a lot about this
            string[] seasons = cropData[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // sprite row of the crop, we care about this
            int sprite = Convert.ToInt32(cropData[2]);
            // harvest of the crop, we care about this
            int harvest = Convert.ToInt32(cropData[3]);
            // regrow of the crop, we care about this
            int regrow = Convert.ToInt32(cropData[4]);
            Rectangle[] baseRectangle = new Rectangle[phases.Length + 1];
            //trellis crop we care about this because seed purchase is only once.
            bool raisedSeeds = Convert.ToBoolean(cropData[7]);
            //wether or not the crop is in water, we dont care. assume always appropriate
            bool paddyCrop = harvest == 271 || harvest == 830;
            //we care a lot about this, we need to know if crops drop more than one item
            string[] cropYieldSplit = cropData[6].Split(' ');
            int minHarvest = 1;
            int maxHarvest = 1;
            int maxHarvestIncreasePerFarmingLevel = 0;
            double chanceForExtraCrops = 0.0f;

            if (cropYieldSplit.Length != 0 && cropYieldSplit[0].Equals("true"))
            {
                minHarvest = Convert.ToInt32(cropYieldSplit[1]);
                maxHarvest = Convert.ToInt32(cropYieldSplit[2]);
                maxHarvestIncreasePerFarmingLevel = Convert.ToInt32(cropYieldSplit[3]);
                chanceForExtraCrops = Convert.ToDouble(cropYieldSplit[4]);
            }
            double[] harvestValues = new double[4]
            {
                minHarvest,
                maxHarvest,
                maxHarvestIncreasePerFarmingLevel,
                chanceForExtraCrops
            };

            // If the sprite is 23, it's a seasonal multi-seed
            // so we want to show that rather than the seed.
            Item item = new SObject(sprite == 23 ? id : harvest, 1);

            if (!Game1.objectInformation.ContainsKey(harvest))
                return null;
            //we dont care for colors

            for (int i = 0; i < baseRectangle.Length; i++)
            {
                _ = i == (baseRectangle.Length - (regrow > 0 ? 2 : 1)); //bool final

                baseRectangle[i] =
                    new Rectangle(
                        Math.Min(240, (i + 1) * 16 + (sprite % 2 != 0 ? 128 : 0)),
                        sprite / 2 * 16 * 2,
                        16, 32);
            }

            /*Helpers.Monitor.Log($"Sprite: {sprite}, Harvest: {harvest}, Regrow: {regrow}, HarvestMethod: {harvestMethod}, RaisedSeeds: {raisedSeeds}", LogLevel.Debug);*/
            int tileSize = SObject.spriteSheetTileSize;

            bool isGiantCrop = false;
            Tuple<Texture2D, Rectangle>? giantSprite = null;

            // Vanilla Giant Crops
            if (harvest == 190 || harvest == 254 || harvest == 276)
            {
                isGiantCrop = true;

                int which;
                if (harvest == 190)
                    which = 0;
                else if (harvest == 254)
                    which = 1;
                else
                    which = 2;

                giantSprite = new(
                    Game1.cropSpriteSheet,
                    new Rectangle(112 + which * 48, 512, 48, 64)
                );
            }

            // JsonAssets Giant Crops
            if (!isGiantCrop && Utils.JApi != null)
            {
                Texture2D? tex = null;

                if (Utils.JApi.TryGetGiantCropSprite(harvest, out var text))
                    tex = text.Value;

                if (tex != null)
                {
                    isGiantCrop = true;
                    giantSprite = new(tex, tex.Bounds);
                }
            }

            Item[] seeds = { new SObject(id, 1) };
            int? seedPrice = seeds[0].salePrice();
            seedPrice = this.SeedPrice(id, seedPrice);

            Crop crop = new(
                id: harvest,
                item: item,
                name: item.DisplayName,
                sprite: new(Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, tileSize, tileSize)),
                isTrellisCrop: raisedSeeds,
                isGiantCrop: isGiantCrop,
                giantSprite: giantSprite,
                seeds: seeds,
                phases: phases.Select(int.Parse).ToArray(),
                regrow: regrow,
                isPaddyCrop: paddyCrop,
                seasons: seasons.Select(s => (Season)Enum.Parse(typeof(Season), s, true)).ToArray(),
                harvestChanceValues: harvestValues,
                seedPrice: seedPrice
            );
            return crop;
        }

        /// <summary>
        /// Gets the seed price for the given seed id. If the seed id is not in the seedPriceOverrides dictionary, the seed price is returned. If the seed id is in the seedPriceOverrides dictionary, the seed price override is returned.
        /// </summary>
        /// <param name="id"> The id of the seed. </param>
        /// <param name="seedPrice"> The current seed price of the crop. </param>
        /// <returns></returns>
        private int? SeedPrice(int id, int? seedPrice)
        {
            if (seedPriceOverrides.ContainsKey(id))
            {
                seedPrice = seedPriceOverrides[id];
            }
            return seedPrice;
        }

        /// <summary>
        /// Builds a dictionary of crops from the ManualCrops.json file. The crops are parsed from the json file and then added to a dictionary.
        /// </summary>
        /// <returns> A dictionary of crops. </returns>
        private Dictionary<string, Crop> BuildCropsFromJson()
        {
            Dictionary<string, Crop> Crops = new();
            Dictionary<int, string[]> crops = Helper.ModContent.Load<Dictionary<int, string[]>>(Path.Combine("assets", "ManualCrops.json"));
            foreach (KeyValuePair<int, string[]> crop in crops)
            {
                Crop? cropToAdd = BuildCropFromJson(crop.Value, crop.Key);
                if (cropToAdd != null && !Crops.ContainsKey(crop.Key.ToString()))
                {
                    Crops.Add(cropToAdd.Name.ToString(), cropToAdd);
                }
            }
            return Crops;
        }

        /// <summary>
        /// Builds a crop from the given data. The data is organized in an array. The data is then parsed into a crop. The crop is then returned.
        /// </summary>
        /// <param name="values"> The data of the crop. </param>
        /// <param name="id"> The id of the crop. </param>
        /// <returns></returns>
        private Crop? BuildCropFromJson(string[] values, int id)
        {
            //id is ID of drop item,
            //Format
            /*"251": [
                "Tea Leaves", // Name
                "815", //harvest id
                "20", //growth time
                "1", //regrowth time
                "spring summer fall", //seasons
                "50", //sale price
                "500", //purchase price
                "1", //min harvest
                "1", //max harvest
                "0.0", //max harvest increase per farming level
                "0.0", //chance for extra harvest
                "false", //affected by qquality
                "false", //affected by fertilizer
                "false", //raised crop
                "true", //bush crop
                "false", //paddy crop
                "false" //giant crop
              ]*/

            int harvest = Convert.ToInt32(values[1]);
            int growthTime = Convert.ToInt32(values[2]);
            int regrowthTime = Convert.ToInt32(values[3]);
            string[] seasons = values[4].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int purchasePrice = Convert.ToInt32(values[6]);
            int minHarvest = Convert.ToInt32(values[7]);
            int maxHarvest = Convert.ToInt32(values[8]);
            int maxHarvestIncreasePerFarmingLevel = Convert.ToInt32(values[9]);
            double chanceForExtraCrops = Convert.ToDouble(values[10]);
            bool affectedByQuality = Convert.ToBoolean(values[11]);
            bool affectedByFertilizer = Convert.ToBoolean(values[12]);
            bool isRaisedCrop = Convert.ToBoolean(values[13]);
            bool paddyCrop = Convert.ToBoolean(values[15]);
            bool isGiantCrop = Convert.ToBoolean(values[16]);

            double[] harvestValues = new double[4]
          {
                minHarvest,
                maxHarvest,
                maxHarvestIncreasePerFarmingLevel,
                chanceForExtraCrops
          };

            // If the sprite is 23, it's a seasonal multi-seed
            // so we want to show that rather than the seed.
            Item item = new SObject(harvest, 1);

            if (!Game1.objectInformation.ContainsKey(harvest))
                return null;
            //we dont care for colors

            int tileSize = SObject.spriteSheetTileSize;

            Tuple<Texture2D, Rectangle>? giantSprite = null;

            // Vanilla Giant Crops
            if (harvest == 190 || harvest == 254 || harvest == 276)
            {
                isGiantCrop = true;

                int which;
                if (harvest == 190)
                    which = 0;
                else if (harvest == 254)
                    which = 1;
                else
                    which = 2;

                giantSprite = new(
                    Game1.cropSpriteSheet,
                    new Rectangle(112 + which * 48, 512, 48, 64)
                );
            }

            WorldDate? startDate = null;
            WorldDate? endDate = null;

            // TODO: Handle weird crops with a gap.

            foreach (string season in seasons)
            {
                WorldDate? start = null;
                WorldDate? end = null;

                try
                {
                    start = new(1, season, 1);
                    end = new(1, season, 28);

                    // Sanity check the seasons, just in case.
                    string test = start.Season;
                    test = end.Season;
                }
                catch (Exception)
                {
                    Monitor.Log($"Invalid season for crop {id} (harvest:{harvest}): {season}", LogLevel.Warn);
                }

                if (startDate == null || startDate > start)
                    startDate = start;
                if (endDate == null || endDate < end)
                    endDate = end;
            }

            Crop crop = new(
                id: harvest,
                item: item,
                name: item.DisplayName,
                sprite: new(Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, tileSize, tileSize)),
                isTrellisCrop: isRaisedCrop,
                isGiantCrop: isGiantCrop,
                giantSprite: giantSprite,
                seeds: new Item[] { new SObject(id, 1) },
                phases: new int[] { growthTime },
                regrow: regrowthTime,
                isPaddyCrop: paddyCrop,
                seasons: seasons.Select(s => (Season)Enum.Parse(typeof(Season), s, true)).ToArray(),
                harvestChanceValues: harvestValues,
                affectByQuality: affectedByQuality,
                affectByFertilizer: affectedByFertilizer,
                seedPrice: purchasePrice
            );
            return crop;
        }
    }
}