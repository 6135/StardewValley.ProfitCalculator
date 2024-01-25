using System.Collections.Generic;

namespace ProfitCalculator.main
{
    /// <summary>
    ///    Abstract class for parsing crop data from a file.
    /// </summary>
    public interface ICropParser
    {
        /// <summary>
        ///  Builds a dictionary of crops from a file or code.
        /// </summary>
        /// <returns> Dictionary of crops. </returns>
        public abstract Dictionary<string, Crop> BuildCrops();
    }
}