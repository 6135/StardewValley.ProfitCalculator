using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ProfitCalculator.helper
{
    public class Helpers
    {
        public static IModHelper Helper { get; set; }
        public static IMonitor Monitor { get; set; }

        public static Texture2D AppIcon { get ; set; }

        public static void Initialize(IModHelper _helper, IMonitor _monitor)
        {
            Helper = _helper;
            Monitor = _monitor;
            AppIcon = _helper.ModContent.Load<Texture2D>(Path.Combine("assets", "app_icon.png"));
        }

        public static int GetSeasonDays(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return 28;

                case Season.Summer:
                    return 28;

                case Season.Autumn:
                    return 28;

                case Season.Winter:
                    return 28;

                case Season.Greenhouse:
                    return 112;

                default:
                    return 0;
            }
        }

        public enum Season
        {
            Spring,
            Summer,
            Autumn,
            Winter,
            Greenhouse
        }

        public enum ProduceType
        {
            Raw,
            Keg,
            Cask
        }

        public enum FertilizerQuality
        {
            Basic,
            Quality,
            SpeedGro,
            DeluxeSpeedGro,
            None
        }

        //get season translated names
        private static string GetTranslatedName(string str)
        {
            //convert string to lowercase
            str = str.ToLower();
            return Helper.Translation.Get(str);
        }

        public static string GetTranslatedSeason(Season season)
        {
            return GetTranslatedName(season.ToString());
        }

        public static string GetTranslatedProduceType(ProduceType produceType)
        {
            return GetTranslatedName(produceType.ToString());
        }

        public static string GetTranslatedFertilizerQuality(FertilizerQuality fertilizerQuality)
        {
            return GetTranslatedName(fertilizerQuality.ToString());
        }

        //get All translated names
        public static string[] GetAllTranslatedSeasons()
        {
            string[] names = Enum.GetNames(typeof(Season));
            string[] translatedNames = new string[names.Length];
            foreach (string name in names)
            {
                translatedNames[Array.IndexOf(names, name)] = GetTranslatedName(name);
            }
            return translatedNames;
        }

        //get all produce types translated names
        public static string[] GetAllTranslatedProduceTypes()
        {
            string[] names = Enum.GetNames(typeof(ProduceType));
            string[] translatedNames = new string[names.Length];
            foreach (string name in names)
            {
                translatedNames[Array.IndexOf(names, name)] = GetTranslatedName(name);
            }
            return translatedNames;
        }

    }
}