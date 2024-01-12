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

        protected CropParser(string name = "CroParser")
        {
            Name = name;
        }

        public abstract Dictionary<string, Crop> BuildCrops();
    }
}