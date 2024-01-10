using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfitCalculator.Helpers;

namespace ProfitCalculator.main
{
    public class Calculator
    {
        // list of crops
        private Dictionary<string, Crop> crops;

        private CropParser[] cropParser;

        //settings passed in constructor
        private uint Day { get; set; }

        private uint MaxDay { get; set; }
        private uint MinDay { get; set; }
        private Season Season { get; set; }
        private ProduceType ProduceType { get; set; }
        private FertilizerQuality FertilizerQuality { get; set; }
        private bool PayForSeeds { get; set; }
        private bool PayForFertilizer { get; set; }
        private uint MaxMoney { get; set; }
        private bool UseBaseStats { get; set; }
        private double[] PriceMultipliers { get; set; } = new double[4] { 1.0, 1.25, 1.5, 2.0 };
        private double AverageValueForCrop { get; set; }
        private int farmingLevel { get; set; }

        // constructor
        public Calculator()
        {
            // get list of crops
            // get list of crop parsers
            cropParser = new CropParser[]
            {
                new VanillaCropParser()
            };
            crops = new Dictionary<string, Crop>();
        }

        public void SetSettings(uint day, uint maxDay, uint minDay, Season season, ProduceType produceType, FertilizerQuality fertilizerQuality, bool payForSeeds, bool payForFertilizer, uint maxMoney, bool useBaseStats, double[] priceMultipliers = null)
        {
            Day = day;
            MaxDay = maxDay;
            MinDay = minDay;
            Season = season;
            ProduceType = produceType;
            FertilizerQuality = fertilizerQuality;
            PayForSeeds = payForSeeds;
            PayForFertilizer = payForFertilizer;
            MaxMoney = maxMoney;
            UseBaseStats = useBaseStats;
            if (priceMultipliers != null)
                PriceMultipliers = priceMultipliers;
            AverageValueForCrop = getAverageValueForCropAfterModifiers();
            if (useBaseStats)
            {
                farmingLevel = 0;
            }
            else
            {
                farmingLevel = Game1.player.FarmingLevel;
            }
        }

        public void Calculate()
        {
            // calculate profit for each crop
            foreach (KeyValuePair<string, Crop> crop in crops)
            {
                // calculate profit for crop
                // add crop to list
            }
        }

        private void RetrieveCropList()
        {
            foreach (CropParser parser in cropParser)
            {
                foreach (KeyValuePair<string, Crop> cr in parser.BuildCrops(UseBaseStats))
                {
                    Crop crop = cr.Value;
                    crop.setSettings(UseBaseStats, (int)FertilizerQuality);
                    crops.Add(cr.Key, crop);
                }
            }
        }

        public void refreshCropList()
        {
            crops = new();
            RetrieveCropList();
        }

        public void printCropChanceTablesForAllFarmingLevels()
        {
            int backupFarmingLevel = farmingLevel;
            Monitor.Log("|Farming Level\tBase Chance\tSilver Chance\tGold Chance\tIridium Chance\tAvg Value|", LogLevel.Debug);
            for (int i = 0; i < 15; i++)
            {
                farmingLevel = i;
                double chanceForGoldQuality = getCropGoldQualityChance();
                double chanceForSilverQuality = getCropSilverQualityChance(chanceForGoldQuality);
                double chanceForIridiumQuality = getCropIridiumChance(chanceForGoldQuality);
                double chanceForBaseQuality = getCropBaseQualityChance(chanceForSilverQuality, chanceForGoldQuality, chanceForIridiumQuality);
                double averageValue = getAverageValueForCropAfterModifiers();

                Monitor.Log(
                    $"|{farmingLevel}\t\t\t   "
                    + $"{(int)(Math.Round(chanceForBaseQuality,2) * 100)}%\t\t"
                    + $"{(int)(Math.Round(chanceForSilverQuality,2) * 100)}%\t\t"
                    + $"{(int)(Math.Round(chanceForGoldQuality, 2) * 100)}%\t\t"
                    + $"{(int)(Math.Round(chanceForIridiumQuality, 2) * 100)}%\t\t"
                    + $"{averageValue}\t\t|"
                    , LogLevel.Debug);
            }
            farmingLevel = backupFarmingLevel;
        }

        public void printCropChanceTablesForAllFarmingLevelsAndFertilizerType()
        {
            FertilizerQuality backupFertilizerQuality = FertilizerQuality;
            for (int i = 0; i < 4; i++)
            {
                FertilizerQuality = (FertilizerQuality)i;
                Monitor.Log($"Fertilizer: {FertilizerQuality}", LogLevel.Debug);
                printCropChanceTablesForAllFarmingLevels();
            }
            FertilizerQuality = backupFertilizerQuality;
        }
        public double getAverageValueMultiplierForCrop()
        {
            double[] priceMultipliers = PriceMultipliers;
            //apply profession modifiers
            /*for (int i = 0; i < prices.Length; i++)
            {
                prices[i] = getPriceAfterMultipliers(prices[i]);
            }*/
            //apply fertilizer quality modifiers

            //apply farm level quality modifiers
            double chanceForGoldQuality = getCropGoldQualityChance();
            double chanceForSilverQuality = getCropSilverQualityChance(chanceForGoldQuality);
            double chanceForIridiumQuality = getCropIridiumChance(chanceForGoldQuality);
            double chanceForBaseQuality = getCropBaseQualityChance(chanceForSilverQuality, chanceForGoldQuality, chanceForIridiumQuality);
            //calculate average value
            double averageValue = 0f;
            averageValue += chanceForBaseQuality * priceMultipliers[0];
            averageValue += chanceForSilverQuality * priceMultipliers[1];
            averageValue += chanceForGoldQuality * priceMultipliers[2];
            averageValue += chanceForIridiumQuality * priceMultipliers[3];
            return averageValue;
        }

        public double getAverageValueForCropAfterModifiers()
        {
            double averageValue = this.getAverageValueMultiplierForCrop();
            if (!UseBaseStats && Game1.player.professions.Contains(Farmer.tiller))
            {
                averageValue *= 1f;//1.1f;
            }
            return Math.Round(averageValue, 2);
        }

        private double getCropSilverQualityChance(double goldChance)
        {
            return (1f - goldChance) * Math.Min(0.75f, goldChance * 2.0f); //garanteed if fertilizer is used and quality of said fertilizer is 3 or higher
        }

        private double getCropGoldQualityChance()
        {
            int fertilizerQualityLevel = (int)FertilizerQuality;

            if (fertilizerQualityLevel < 0)
            {
                fertilizerQualityLevel = 0;
            }

            return 0.2 * ((double)farmingLevel / 10.0) + 0.2 * (double)fertilizerQualityLevel * (((double)farmingLevel + 2.0) / 12.0) + 0.01;
        }

        private double getCropIridiumChance(double goldChance)
        {
            int fertilizerQualityLevel = (int)FertilizerQuality;

            if (fertilizerQualityLevel < 0)
            {
                fertilizerQualityLevel = 0;
            }
            return fertilizerQualityLevel >= 3 ? (goldChance / 2.0f) : 0; //if fertilizer is used and quality of said fertilizer is 3 or higher
        }

        private double getCropBaseQualityChance(double silverChance, double goldChance, double iridiumChance)
        {
            return 1f - silverChance - goldChance - iridiumChance;
        }
    }
}