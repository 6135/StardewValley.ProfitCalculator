using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.CraneGame;
using SObject = StardewValley.Object;
using SCrop = StardewValley.Crop;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using static ProfitCalculator.Helpers;

namespace ProfitCalculator.main
{
    internal class VanillaCropParser : CropParser
    {
        public VanillaCropParser(string name = "VanillaCropParser") : base(name)
        {
        }

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
            return Crops;
        }

        //Thanks to Klhoe Leclair for this code
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
            //we care about the grown one for the sprite
            Texture2D[] sprites = new Texture2D[phases.Length + 1];
            Rectangle[] baseRectangle = new Rectangle[phases.Length + 1];
            // harvest method of the crop, we dont care about this
            int harvestMethod = Convert.ToInt32(cropData[5]);
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
                bool final = i == (baseRectangle.Length - (regrow > 0 ? 2 : 1));

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
            if (!isGiantCrop && Helpers.JApi != null)
            {
                Texture2D? tex = null;

                if (Helpers.JApi.TryGetGiantCropSprite(harvest, out var text))
                    tex = text.Value;

                if (tex != null)
                {
                    isGiantCrop = true;
                    giantSprite = new(tex, tex.Bounds);
                }
            }

            WorldDate? startDate = null;
            WorldDate? endDate = null;

            // TODO: Handle weird crops with a gap.

            foreach (string season in seasons)
            {
                WorldDate start = null;
                WorldDate end = null;

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
                    Helpers.Monitor.Log($"Invalid season for crop {id} (harvest:{harvest}): {season}", LogLevel.Warn);
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
                isTrellisCrop: raisedSeeds,
                isGiantCrop: isGiantCrop,
                giantSprite: giantSprite,
                seeds: new Item[] { new SObject(id, 1) },
                phases: phases.Select(int.Parse).ToArray(),
                phaseSprites: sprites,
                regrow: regrow,
                isPaddyCrop: paddyCrop,
                startDate: startDate,
                endDate: endDate,
                seasons: seasons.Select(s => (Season)Enum.Parse(typeof(Season), s, true)).ToArray(),
                harvestChanceValues: harvestValues
            );
            return crop;
        }
    }
}