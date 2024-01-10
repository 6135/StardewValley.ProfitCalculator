using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Linq;
using System.Xml.Linq;
using static ProfitCalculator.Helpers;
using SObject = StardewValley.Object;

#nullable enable

namespace ProfitCalculator.main
{
    // retrives and stores crop data from game files
    internal class Crop
    {
        public int Id;
        public Item Item;
        public string Name;
        public Tuple<Texture2D, Rectangle>? Sprite;
        public bool IsTrellisCrop;
        public bool IsGiantCrop;
        public Tuple<Texture2D, Rectangle>? GiantSprite;
        public Item[]? Seeds;
        public int[] Phases;
        public Texture2D?[]? PhaseSprites;
        public int Regrow;
        public bool IsPaddyCrop;

        public WorldDate? StartDate;
        public WorldDate? EndDate;
        public int Days;
        public int Price = -1;
        public bool UsingBaseStats;
        public int FertilizerQuality;

        //prices should be calculated from price, then price x1.25, then price x1.5, then price x2
        public float AveragePriceMultiplier;

        public Crop(int id, Item item, string name, Tuple<Texture2D, Rectangle>? sprite, bool isTrellisCrop, bool isGiantCrop, Tuple<Texture2D, Rectangle>? giantSprite, Item[]? seeds, int[] phases, Texture2D?[]? phaseSprites, int regrow, bool isPaddyCrop, WorldDate? startDate, WorldDate? endDate)
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
        }

        public void setSettings(bool usingBaseStats, int fertilizerQuality)
        {
            this.UsingBaseStats = usingBaseStats;
            this.FertilizerQuality = fertilizerQuality;
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
    }
}