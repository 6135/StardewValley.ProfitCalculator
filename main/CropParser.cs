using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitCalculator.main
{
    internal abstract class CropParser
    {
        public readonly string Name;

        protected Dictionary<string, Crop> Crops;

        protected CropParser(string name = "CroParser", Dictionary<string, Crop> crops = null)
        {
            Name = name;
            if (crops is null)
            {
                Crops = new();
            }
            else
            {
                Crops = crops;
            }
        }

        public abstract Dictionary<string, Crop> BuildCrops();
    }
}