using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Linq;
using System.Xml.Linq;
using static ProfitCalculator.Helpers;
using SObject = StardewValley.Object;

#nullable enable

namespace ProfitCalculator.main
{
    // retrives and stores crop data from game files
    public class Crop
    {
        public readonly int Id;
        public readonly Item Item;
        public readonly string Name;
        public Tuple<Texture2D, Rectangle>? Sprite;
        public readonly bool IsTrellisCrop;
        public readonly bool IsGiantCrop;
        public Tuple<Texture2D, Rectangle>? GiantSprite;
        public Item[]? Seeds;
        public int[] Phases;
        public Texture2D?[]? PhaseSprites;
        public readonly int Regrow;
        public readonly bool IsPaddyCrop;

        public WorldDate? StartDate;
        public WorldDate? EndDate;
        public readonly Season[] Seasons;
        public readonly int Days;
        public readonly int Price;
        public readonly double ChanceForExtraCrops;
        public readonly int MaxHarvests;
        public readonly int MinHarvests;
        public readonly int MaxHarvestIncreasePerFarmingLevel;

        public Crop(int id, Item item, string name, Tuple<Texture2D, Rectangle>? sprite, bool isTrellisCrop, bool isGiantCrop, Tuple<Texture2D, Rectangle>? giantSprite, Item[]? seeds, int[] phases, Texture2D?[]? phaseSprites, int regrow, bool isPaddyCrop, WorldDate? startDate, WorldDate? endDate, Season[] seasons, double[] harvestChanceValues)
        {
            Id = id;
            Item = item;
            Name = name;
            Sprite = sprite;
            IsTrellisCrop = isTrellisCrop;
            IsGiantCrop = isGiantCrop;
            GiantSprite = giantSprite;
            Seeds = seeds;
            Phases = phases;
            PhaseSprites = phaseSprites;
            Regrow = regrow;
            IsPaddyCrop = isPaddyCrop;
            StartDate = startDate;
            EndDate = endDate;
            Days = Phases.Sum();
            Price = ((SObject)Item).Price;
            Seasons = seasons;
            MaxHarvests = (int)harvestChanceValues[0];
            MinHarvests = (int)harvestChanceValues[1];
            MaxHarvestIncreasePerFarmingLevel = (int)harvestChanceValues[2];
            ChanceForExtraCrops = harvestChanceValues[3];
        }

        public override bool Equals(object? obj)
        {
            if (obj is Crop crop)
            {
                return crop.Id == Id;
            }
            else return false;
        }

        public override string? ToString()
        {
            return $"{Name} ({Id})"
                + $"\n\tSprite: {Sprite} "
                + $"\tIsTrellisCrop: {IsTrellisCrop}"
                + $"\n\tIsGiantCrop: {IsGiantCrop} "
                + $"\tGiantSprite: {GiantSprite}"
                + $"\n\tSeeds: {Seeds} "
                + $"\tPhases: {Phases}"
                + $"\n\tPhaseSprites: {PhaseSprites} "
                + $"\tRegrow: {Regrow}"
                + $"\n\tIsPaddyCrop: {IsPaddyCrop} "
                + $"\tStartDate: {StartDate}"
                + $"\n\tEndDate: {EndDate} "
                + $"\tDays: {Days} "
                + $"\n\tPrice: {Price}";
        }

        public override int GetHashCode()
        {
            //using FNV-1a hash
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                hash = (hash ^ Id) * p;
                hash = (hash ^ Name.GetHashCode()) * p;
                hash = (hash ^ IsTrellisCrop.GetHashCode()) * p;
                hash = (hash ^ IsGiantCrop.GetHashCode()) * p;
                hash = (hash ^ Regrow) * p;
                hash = (hash ^ IsPaddyCrop.GetHashCode()) * p;
                hash = (hash ^ Days) * p;
                hash = (hash ^ Price) * p;
                hash = (hash ^ ChanceForExtraCrops.GetHashCode()) * p;
                hash = (hash ^ MaxHarvests) * p;
                hash = (hash ^ MinHarvests) * p;
                hash = (hash ^ MaxHarvestIncreasePerFarmingLevel) * p;

                return hash;
            }
        }

        #region Growth Values Calculations

        private float GetAverageGrowthSpeedValueForCrop(FertilizerQuality fertilizerQuality)
        {
            float speedIncreaseModifier = 1.0f;
            if ((int)fertilizerQuality == -1)
            {
                speedIncreaseModifier += 0.1f;
            }
            else if ((int)fertilizerQuality == -2)
            {
                speedIncreaseModifier += 0.25f;
            }
            else if ((int)fertilizerQuality == -3)
            {
                speedIncreaseModifier += 0.33f;
            }
            //if paddy crop then add 0.25f and if profession is agriculturist then add 0.1f
            if (IsPaddyCrop)
            {
                speedIncreaseModifier += 0.25f;
            }
            if (Game1.player.professions.Contains(Farmer.agriculturist))
            {
                speedIncreaseModifier += 0.1f;
            }
            return speedIncreaseModifier;
        }

        public bool IsAvailableForCurrentSeason(Season currentSeason)
        {
            return Seasons.Contains(currentSeason);
        }

        public int TotalAvailableDays(Season currentSeason, int day)
        {
            int totalAvailableDays = 0;
            if (IsAvailableForCurrentSeason(currentSeason))
            {
                //Each season has 28 days,
                //get index of current season
                int seasonIndex = Array.IndexOf(Seasons, currentSeason);
                //iterate over the array and add the number of days for each season that is later than the current season
                for (int i = seasonIndex + 1; i < Seasons.Length; i++)
                {
                    totalAvailableDays += 28;
                }
                //add the number of days in the current season
                totalAvailableDays += TotalAvailableDaysInCurrentSeason(day);
            }
            return totalAvailableDays;
        }

        public int TotalAvailableDaysInCurrentSeason(int day)
        {
            return 28 - day;
        }

        public int TotalHarvestsWithRemainingDays(Season currentSeason, FertilizerQuality fertilizerQuality, int day)
        {
            int totalHarvestTimes = 0;
            int totalAvailableDays = TotalAvailableDays(currentSeason, day);
            //season is Greenhouse
            if (currentSeason == Season.Greenhouse)
            {
                totalAvailableDays = (28 * 4);
            }
            int growingDays = Days * (int)GetAverageGrowthSpeedValueForCrop(fertilizerQuality);
            if (IsAvailableForCurrentSeason(currentSeason) || currentSeason == Season.Greenhouse)
            {
                if (totalAvailableDays < growingDays)
                    return 0;
                //if the crop regrows, then the total harvest times are 1 for the first harvest and then the number of times it can regrow in the remaining days. We always need to subtract one to account for the day lost in the planting day.
                if (Regrow > 0)
                {
                    totalHarvestTimes = (1 + (totalAvailableDays - 1 - growingDays) / Regrow);
                }
                else
                    totalHarvestTimes = (totalAvailableDays - 1) / growingDays;
            }
            return totalHarvestTimes;
        }

        public double TotalChanceForExtraCrop()
        {
            //TODO: Implement
            return 0f;
        }
        #endregion Growth Values Calculations

        #region Profit Calculations

        public double TotalProfitOverRemainingDays(bool payForSeeds, bool payFertelizer, Season currentSeason, FertilizerQuality fertilizerQuality, int day, int money, double valueModifier)
        {
            double totalProfitOverRemainingDays = 0.0;
            return totalProfitOverRemainingDays;
            //TODO: Implement
        }

        public double TotalProfitPerDayOverRemainingDays(bool payForSeeds, bool payFertelizer, Season currentSeason, FertilizerQuality fertilizerQuality, int day, int money, double valueModifier)
        {
            double totalProfitPerDayOverRemainingDays = 0.0;
            return totalProfitPerDayOverRemainingDays;
            //TODO: Implement
        }

        public double TotalSeedCostOverRemainingDays(bool payForSeeds, bool payFertelizer, Season currentSeason, FertilizerQuality fertilizerQuality, int day, int money, double valueModifier)
        {
            double totalSeedCostOverRemainingDays = 0.0;
            return totalSeedCostOverRemainingDays;
            //TODO: Implement
        }

        public double TotalSeedCostPerDayOverRemainingDays(bool payForSeeds, bool payFertelizer, Season currentSeason, FertilizerQuality fertilizerQuality, int day, int money, double valueModifier)
        {
            double totalSeedCostPerDayOverRemainingDays = 0.0;
            return totalSeedCostPerDayOverRemainingDays;
            //TODO: Implement
        }

        public double TotalFertilizerCostOverRemainingDays(bool payForSeeds, bool payFertelizer, Season currentSeason, FertilizerQuality fertilizerQuality, int day, int money, double valueModifier)
        {
            double totalFertilizerCostOverRemainingDays = 0.0;
            return totalFertilizerCostOverRemainingDays;
            //TODO: Implement
        }

        public double TotalFertilizerCostPerDayOverRemainingDays(bool payForSeeds, bool payFertelizer, Season currentSeason, FertilizerQuality fertilizerQuality, int day, int money, double valueModifier)
        {
            double totalFertilizerCostPerDayOverRemainingDays = 0.0;
            return totalFertilizerCostPerDayOverRemainingDays;
            //TODO: Implement
        }


        #endregion Profit Calculations

        //Profit Per day, Profit per day must take into consideration regrowth time, average price, and the number of days it takes to grow. Also if the time to grow or regrow is greater than the number of days in a season or seasons if multi season crop, then the profit per day should be 0. In case of multiple output multiply this value by the chance to get more than one item. Also it needs to recieve the fertilizer quality.
    }
}