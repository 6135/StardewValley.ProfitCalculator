using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfitCalculator.Utils;

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
                foreach (KeyValuePair<string, Crop> cr in parser.BuildCrops())
                {
                    Crop crop = cr.Value;
                    crops.Add(cr.Key, crop);
                    /*Monitor.Log($"Added crop: {cr.Value.Name} Id: {cr.Key} Seed: {cr.Value.Seeds[0].ParentSheetIndex} ValueWithStats: {crop.Price * this.getAverageValueForCropAfterModifiers()} #Harvests: {cr.Value.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day)}", LogLevel.Debug);*/
                }
            }
        }

        public void refreshCropList()
        {
            crops = new();
            RetrieveCropList();
        }

        public List<Crop> RetrieveCropsAsOrderderList()
        {
            // sort crops by profit
            // return list
            List<Crop> cropList = new();
            if (crops.Count == 0)
            {
                RetrieveCropList();
            }
            foreach (KeyValuePair<string, Crop> crop in crops)
            {
                cropList.Add(crop.Value);
            }
            cropList.Sort((x, y) => y.Price.CompareTo(x.Price));
            foreach (Crop crop in cropList)
            {
                Monitor.Log($"OC: {crop.Name} Id: {crop.Id} Seed: {crop.Seeds[0].ParentSheetIndex} ValueWithStats: {crop.Price * this.getAverageValueForCropAfterModifiers()} #Harvests: {crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day)} TotalProfit: {totalCropProfit(crop)} " +
                    $"ppd: {totalCropProfitPerDay(crop)} " +
                    $"tfn: {totalFertilizerNeeded(crop)} " +
                    $"tfnpd: {totalFertilzerCostPerDay(crop)} " +
                    $"tsn: {totalSeedsNeeded(crop)} " +
                    $"tsc: {totalSeedsCost(crop)} " +
                    $"tscpd: {totalSeedsCostPerDay(crop)}", LogLevel.Debug);
            }
            return cropList;
        }

        public List<CropInfo> RetrieveCropInfos()
        {
            if (crops.Count == 0)
            {
                RetrieveCropList();
            }
            List<CropInfo> cropInfos = new();
            foreach (Crop crop in crops.Values)
            {
                cropInfos.Add(RetrieveCropInfo(crop));
            }
            return cropInfos;
        }

        private CropInfo RetrieveCropInfo(Crop crop)
        {
            double totalProfit = totalCropProfit(crop);
            double profitPerDay = totalCropProfitPerDay(crop);
            double totalSeedLoss = totalSeedsCost(crop);
            double seedLossPerDay = totalSeedsCostPerDay(crop);
            double totalFertilizerLoss = totalFertilizerCost(crop);
            double fertilizerLossPerDay = totalFertilzerCostPerDay(crop);
            ProduceType produceType = ProduceType;
            int duration = crop.TotalAvailableDays(Season, (int)Day);
            int totalHarvests = crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day);
            int growthTime = crop.Days;
            int regrowthTime = crop.Regrow;
            int productCount = crop.MinHarvests;
            double chanceOfExtraProduct = crop.AverageExtraCropsFromRandomness();
            double chanceOfNormalQuality = getCropBaseQualityChance();
            double chanceOfSilverQuality = getCropSilverQualityChance();
            double chanceOfGoldQuality = getCropGoldQualityChance();
            double chanceOfIridiumQuality = getCropIridiumQualityChance();
            return new CropInfo(crop, totalProfit, profitPerDay, totalSeedLoss, seedLossPerDay, totalFertilizerLoss, fertilizerLossPerDay, produceType, duration, totalHarvests, growthTime, regrowthTime, productCount, chanceOfExtraProduct, chanceOfNormalQuality, chanceOfSilverQuality, chanceOfGoldQuality, chanceOfIridiumQuality);
        }

        #region Crop Profit Calculations

        private double totalCropProfit(Crop crop)
        {
            double averageValue = crop.Price * this.getAverageValueForCropAfterModifiers();//only applies to first produce
            double totalProfitFromFirstProduce = averageValue;
            double averageExtraCrops = crop.AverageExtraCropsFromRandomness();
            double totalProfitFromAllProduce = (crop.MinHarvests - 1 >= 0 ? crop.MinHarvests - 1 : 0) * crop.Price;
            totalProfitFromAllProduce += crop.Price * averageExtraCrops;

            if (!UseBaseStats && Game1.player.professions.Contains(Farmer.tiller))
            {
                totalProfitFromAllProduce *= 1f; //TODO: Re-add 1.1f;
            }
            return (totalProfitFromFirstProduce + totalProfitFromAllProduce) * crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day);
        }

        private double totalCropProfitPerDay(Crop crop)
        {
            double totalProfit = totalCropProfit(crop);
            if (totalProfit == 0)
            {
                return 0;
            }
            double totalCropProfitPerDay = totalProfit / crop.TotalAvailableDays(Season, (int)Day);
            return totalCropProfitPerDay;
        }

        private int totalFertilizerNeeded(Crop crop)
        {
            if (Season == Season.Greenhouse || crop.Seasons.Length > 1)
                return 1;
            else
            {
                return (int)Math.Ceiling(crop.TotalAvailableDays(Season, (int)Day) / 28.0);
            }
        }

        private int totalFertilizerCost(Crop crop)
        {
            int fertNeeded = totalFertilizerNeeded(crop);
            int fertCost = Utils.FertilizerPrices(FertilizerQuality);
            return fertNeeded * fertCost;
        }

        private double totalFertilzerCostPerDay(Crop crop)
        {
            int fertCost = totalFertilizerCost(crop);
            if (fertCost == 0)
            {
                return 0;
            }
            double totalFertilizerCostPerDay = (double)fertCost / (double)crop.TotalAvailableDays(Season, (int)Day);
            return totalFertilizerCostPerDay;
        }

        private int totalSeedsNeeded(Crop crop)
        {
            if (crop.Regrow > 0 && crop.TotalAvailableDays(Season, (int)Day) > 0)
                return 1;
            else return crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day);
        }

        private int totalSeedsCost(Crop crop)
        {
            int seedsNeeded = totalSeedsNeeded(crop);
            int seedCost = crop.Seeds[0].salePrice();
            return seedsNeeded * seedCost;
        }

        private double totalSeedsCostPerDay(Crop crop)
        {
            int seedCost = totalSeedsCost(crop);
            if (seedCost == 0)
            {
                return 0;
            }
            double totalSeedsCostPerDay = (double)seedCost / (double)crop.TotalAvailableDays(Season, (int)Day);
            return totalSeedsCostPerDay;
        }

        #endregion Crop Profit Calculations

        #region Crop Modifer Value Calculations

        public void printCropChanceTablesForAllFarmingLevels()
        {
            int backupFarmingLevel = farmingLevel;
            Monitor.Log("|Farming Level\tBase Chance\tSilver Chance\tGold Chance\tIridium Chance\tAvg Value|", LogLevel.Debug);
            for (int i = 0; i < 15; i++)
            {
                farmingLevel = i;
                double chanceForGoldQuality = getCropGoldQualityChance();
                double chanceForSilverQuality = getCropSilverQualityChance();
                double chanceForIridiumQuality = getCropIridiumQualityChance();
                double chanceForBaseQuality = getCropBaseQualityChance();
                double averageValue = getAverageValueForCropAfterModifiers();

                Monitor.Log(
                    $"|{farmingLevel}\t\t\t   "
                    + $"{(chanceForBaseQuality * 100).ToString("##")}\t\t"
                    + $"{(chanceForSilverQuality * 100).ToString("##")}\t\t"
                    + $"{(chanceForGoldQuality * 100).ToString("##")}\t\t"
                    + $"{(chanceForIridiumQuality * 100).ToString("##")}\t\t"
                    + $"{averageValue}|"
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

            //apply farm level quality modifiers
            double chanceForGoldQuality = getCropGoldQualityChance();
            double chanceForSilverQuality = getCropSilverQualityChance();
            double chanceForIridiumQuality = getCropIridiumQualityChance();
            double chanceForBaseQuality = getCropBaseQualityChance();
            //calculate average value modifier for price
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

        private double getCropBaseGoldQualityChance(double limit = 9999999999)
        {
            int fertilizerQualityLevel = ((int)FertilizerQuality) > 0 ? ((int)FertilizerQuality) : 0;
            double part1 = (0.2 * (farmingLevel / 10.0)) + 0.01;
            double part2 = 0.2 * (fertilizerQualityLevel * ((farmingLevel + 2) / 12.0));
            return Math.Min(limit, part1 + part2);
        }

        private double getCropBaseQualityChance()
        {
            return FertilizerQuality >= FertilizerQuality.Deluxe ? 0f : Math.Max(0f, 1f - (this.getCropIridiumQualityChance() + this.getCropGoldQualityChance() + this.getCropSilverQualityChance()));
        }

        private double getCropSilverQualityChance()
        {
            return FertilizerQuality >= FertilizerQuality.Deluxe ? 1f - (this.getCropIridiumQualityChance() + this.getCropGoldQualityChance()) : (1f - this.getCropIridiumQualityChance()) * (1f - this.getCropBaseGoldQualityChance()) * Math.Min(0.75, 2 * this.getCropBaseGoldQualityChance());
        }

        private double getCropGoldQualityChance()
        {
            return this.getCropBaseGoldQualityChance(1f) * (1f - this.getCropIridiumQualityChance());
        }

        private double getCropIridiumQualityChance()
        {
            return FertilizerQuality >= FertilizerQuality.Deluxe ? getCropBaseGoldQualityChance() / 2.0 : 0f;
        }

        #endregion Crop Modifer Value Calculations
    }
}