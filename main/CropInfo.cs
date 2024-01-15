using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfitCalculator;

namespace ProfitCalculator.main
{
    public class CropInfo
    {
        public readonly Crop Crop;
        public readonly double TotalProfit;
        public readonly double ProfitPerDay;

        public readonly double TotalSeedLoss;
        public readonly double SeedLossPerDay;

        public readonly double TotalFertilizerLoss;
        public readonly double FertilizerLossPerDay;

        public readonly Utils.ProduceType ProduceType;
        public readonly int Duration;
        public readonly int TotalHarvests;
        public readonly int GrowthTime;
        public readonly int RegrowthTime;
        public readonly int ProductCount;

        public readonly double ChanceOfExtraProduct;
        public readonly double ChanceOfNormalQuality;
        public readonly double ChanceOfSilverQuality;
        public readonly double ChanceOfGoldQuality;
        public readonly double ChanceOfIridiumQuality;

        public CropInfo(Crop crop, double totalProfit, double profitPerDay, double totalSeedLoss, double seedLossPerDay, double totalFertilizerLoss, double fertilizerLossPerDay, Utils.ProduceType produceType, int duration, int totalHarvests, int growthTime, int regrowthTime, int productCount, double chanceOfExtraProduct, double chanceOfNormalQuality, double chanceOfSilverQuality, double chanceOfGoldQuality, double chanceOfIridiumQuality)
        {
            Crop = crop;
            TotalProfit = totalProfit - totalSeedLoss - totalFertilizerLoss;
            ProfitPerDay = profitPerDay - seedLossPerDay - fertilizerLossPerDay;
            TotalSeedLoss = totalSeedLoss;
            SeedLossPerDay = seedLossPerDay;
            TotalFertilizerLoss = totalFertilizerLoss;
            FertilizerLossPerDay = fertilizerLossPerDay;
            ProduceType = produceType;
            Duration = duration;
            TotalHarvests = totalHarvests;
            GrowthTime = growthTime;
            RegrowthTime = regrowthTime;
            ProductCount = productCount;
            ChanceOfExtraProduct = chanceOfExtraProduct;
            ChanceOfNormalQuality = chanceOfNormalQuality;
            ChanceOfSilverQuality = chanceOfSilverQuality;
            ChanceOfGoldQuality = chanceOfGoldQuality;
            ChanceOfIridiumQuality = chanceOfIridiumQuality;
        }

        #region Overloads and Overrides

        public override string ToString()
        { //return object in json format
            return "{" +
                $"\"Crop\": {Crop.Name}," +
                $"\"TotalProfit\": {TotalProfit}," +
                $"\"ProfitPerDay\": {ProfitPerDay}," +
                $"\"TotalSeedLoss\": {TotalSeedLoss}," +
                $"\"SeedLossPerDay\": {SeedLossPerDay}," +
                $"\"TotalFertilizerLoss\": {TotalFertilizerLoss}," +
                $"\"FertilizerLossPerDay\": {FertilizerLossPerDay}," +
                $"\"ProduceType\": {ProduceType}," +
                $"\"Duration\": {Duration}," +
                $"\"TotalHarvests\": {TotalHarvests}," +
                $"\"GrowthTime\": {GrowthTime}," +
                $"\"RegrowthTime\": {RegrowthTime}," +
                $"\"ProductCount\": {ProductCount}," +
                $"\"ChanceOfExtraProduct\": {ChanceOfExtraProduct}," +
                $"\"ChanceOfNormalQuality\": {ChanceOfNormalQuality}," +
                $"\"ChanceOfSilverQuality\": {ChanceOfSilverQuality}," +
                $"\"ChanceOfGoldQuality\": {ChanceOfGoldQuality}," +
                $"\"ChanceOfIridiumQuality\": {ChanceOfIridiumQuality}" +
                "}";
        }

        public override bool Equals(object obj)
        {
            return obj is CropInfo cropInfo &&
                   EqualityComparer<Crop>.Default.Equals(Crop, cropInfo.Crop) &&
                   TotalProfit == cropInfo.TotalProfit &&
                   ProfitPerDay == cropInfo.ProfitPerDay &&
                   TotalSeedLoss == cropInfo.TotalSeedLoss &&
                   SeedLossPerDay == cropInfo.SeedLossPerDay &&
                   TotalFertilizerLoss == cropInfo.TotalFertilizerLoss &&
                   FertilizerLossPerDay == cropInfo.FertilizerLossPerDay &&
                   ProduceType == cropInfo.ProduceType &&
                   Duration == cropInfo.Duration &&
                   TotalHarvests == cropInfo.TotalHarvests &&
                   GrowthTime == cropInfo.GrowthTime &&
                   RegrowthTime == cropInfo.RegrowthTime &&
                   ProductCount == cropInfo.ProductCount &&
                   ChanceOfExtraProduct == cropInfo.ChanceOfExtraProduct &&
                   ChanceOfNormalQuality == cropInfo.ChanceOfNormalQuality &&
                   ChanceOfSilverQuality == cropInfo.ChanceOfSilverQuality &&
                   ChanceOfGoldQuality == cropInfo.ChanceOfGoldQuality &&
                   ChanceOfIridiumQuality == cropInfo.ChanceOfIridiumQuality;
        }

        //override == and != operators
        public static bool operator ==(CropInfo left, CropInfo right)
        {
            return EqualityComparer<CropInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(CropInfo left, CropInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Crop);
            hash.Add(TotalProfit);
            hash.Add(ProfitPerDay);
            hash.Add(TotalSeedLoss);
            hash.Add(SeedLossPerDay);
            hash.Add(TotalFertilizerLoss);
            hash.Add(FertilizerLossPerDay);
            hash.Add(ProduceType);
            hash.Add(Duration);
            hash.Add(TotalHarvests);
            hash.Add(GrowthTime);
            hash.Add(RegrowthTime);
            hash.Add(ProductCount);
            hash.Add(ChanceOfExtraProduct);
            hash.Add(ChanceOfNormalQuality);
            hash.Add(ChanceOfSilverQuality);
            hash.Add(ChanceOfGoldQuality);
            hash.Add(ChanceOfIridiumQuality);
            return hash.ToHashCode();
        }

        #endregion Overloads and Overrides
    }
}