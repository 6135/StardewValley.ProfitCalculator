using System.Collections.Generic;

namespace ProfitCalculator.main
{
    /// <summary>
    ///    Abstract class for parsing crop data from a file.
    /// </summary>
    public abstract class CropParser
    {
        /// <summary> Name of the parser. </summary>
        public readonly string Name;

        /// <summary>
        ///   Constructor for the CropParser class.
        /// </summary>
        /// <param name="name"> Name of the parser. </param>
        protected CropParser(string name = "CroParser")
        {
            Name = name;
        }

        /// <summary>
        ///  Builds a dictionary of crops from a file or code.
        /// </summary>
        /// <returns> Dictionary of crops. </returns>
        public abstract Dictionary<string, Crop> BuildCrops();
    }
}