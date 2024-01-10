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
            RetrieveCropList();

        }

        public void SetSettings(uint day, uint maxDay, uint minDay, Season season, ProduceType produceType, FertilizerQuality fertilizerQuality, bool payForSeeds, bool payForFertilizer, uint maxMoney, bool useBaseStats)
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
                foreach (KeyValuePair<string, Crop> crop in parser.BuildCrops())
                {
                    crops.Add(crop.Key, crop.Value);
                }
            }
            //print crop list
            foreach (KeyValuePair<string, Crop> crop in crops)
            {
                Monitor.Log(crop.Value.ToString(), LogLevel.Debug);
            }
        }

        public void refreshCropList()
        {
            crops = new();
            RetrieveCropList();
        }
    }
}