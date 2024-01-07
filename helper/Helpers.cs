using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobileProfitCalculator.helper
{
    public class Helpers
    {
        private static IModHelper helper;
        private static Texture2D appIcon;

        public static Texture2D AppIcon { get => appIcon; set => appIcon = value; }

        public static void Initialize(IModHelper _helper)
        {
            helper = _helper;
            AppIcon = helper.ModContent.Load<Texture2D>(Path.Combine("assets", "app_icon.png"));
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
        private string getTranslatedName(string str)
        {
            //convert string to lowercase
            str = str.ToLower();
            return helper.Translation.Get(str);
        }

        public string getTranslatedSeason(Season season)
        {
            return getTranslatedName(season.ToString());
        }

        public string getTranslatedProduceType(ProduceType produceType)
        {
            return getTranslatedName(produceType.ToString());
        }

        public string getTranslatedFertilizerQuality(FertilizerQuality fertilizerQuality)
        {
            return getTranslatedName(fertilizerQuality.ToString());
        }


    }
}