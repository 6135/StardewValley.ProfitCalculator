using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using JsonAssets;
using DynamicGameAssets;
using ProfitCalculator.main;
using StardewValley;
using SObject = StardewValley.Object;
using StardewValley.Locations;
#nullable enable

namespace ProfitCalculator
{
    public class Helpers
    {
        public static IModHelper Helper { get; set; }
        public static IMonitor Monitor { get; set; }

        public static IApi? JApi { get; set; }

        public static IDynamicGameAssetsApi? DApi { get; set; }

        public static Texture2D AppIcon { get; set; }

        public static void Initialize(IModHelper _helper, IMonitor _monitor, IApi? jApi = null, IDynamicGameAssetsApi? dApi = null)
        {
            Helper = _helper;
            Monitor = _monitor;
            AppIcon = _helper.ModContent.Load<Texture2D>(Path.Combine("assets", "app_icon.png"));
            JApi = jApi;
            DApi = dApi;
        }

        public static int GetSeasonDays(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return 28;

                case Season.Summer:
                    return 28;

                case Season.Fall:
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
            Spring = 0,
            Summer = 1,
            Fall = 2,
            Winter = 3,
            Greenhouse = 4
        }

        public enum ProduceType
        {
            Raw,
            Keg,
            Cask
        }

        public enum FertilizerQuality
        {
            None = 0,
            Basic = 1,
            Quality = 2,
            Deluxe = 3,
            SpeedGro = -1,
            DeluxeSpeedGro = -2,
            HyperSpeedGro = -3
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

        //get all fertilizer quality translated names
        public static string[] GetAllTranslatedFertilizerQualities()
        {
            string[] names = Enum.GetNames(typeof(FertilizerQuality));
            string[] translatedNames = new string[names.Length];
            foreach (string name in names)
            {
                translatedNames[Array.IndexOf(names, name)] = GetTranslatedName(name);
            }
            return translatedNames;
        }

        public static int FertilizerPrices(FertilizerQuality fq)
        {
            return fq switch
            {
                FertilizerQuality.None => 0,
                FertilizerQuality.Basic => 100,
                FertilizerQuality.Quality => 150,
                FertilizerQuality.Deluxe => 200,
                FertilizerQuality.SpeedGro => 100,
                FertilizerQuality.DeluxeSpeedGro => 150,
                FertilizerQuality.HyperSpeedGro => 200,
                _ => 0,
            };
        }
        public static List<SeedShop> GetSeedShopList()
        {
            //TODO: Implement ability to select shop price
            /*   List<SeedShop> seedShops = new List<SeedShop>();
            foreach (GameLocation location in Game1.locations)
            {
                if (location is SeedShop shop)
                {
                    seedShops.Add(shop);
                }
            }
            return seedShops;*/
            return null;
        }

        //get cheapest price from all seed shops
        public static int GetCheapestPriceFromAllSeedShops(int parentSheetIndex /*aka ID*/)
        {
            //TODO: Implement ability to select shop price
            /*List<SeedShop> seedShops = GetSeedShopList();
            int cheapestPrice = 0;
            foreach (SeedShop shop in seedShops)
            {
                if (shop.stock.ContainsKey(parentSheetIndex))
                {
                    int price = shop.stock[parentSheetIndex][0];
                    if (cheapestPrice == 0 || price < cheapestPrice)
                    {
                        cheapestPrice = price;
                    }
                }
            }
            return cheapestPrice;*/
            return 0;
        }
    }
}