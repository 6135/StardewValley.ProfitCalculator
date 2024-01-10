using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Linq;
using SObject  = StardewValley.Object;
#nullable enable

namespace ProfitCalculator.main
{
    // retrives and stores crop data from game files
    internal class Crop
    {
        private int Id;
        private Item Item;
        private string Name;
        private Tuple<Texture2D, Rectangle>? Sprite;
        private bool IsTrellisCrop;
        private bool IsGiantCrop;
        private Tuple<Texture2D, Rectangle>? GiantSprite;
        private Item[]? Seeds;
        private int[] Phases;
        private Texture2D?[]? PhaseSprites;
        private int Regrow;
        private bool IsPaddyCrop;

        private WorldDate? StartDate;
        private WorldDate? EndDate;
        private int Days;
        private int Price;

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
            Price = ((SObject)Item).salePrice();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Crop)
            {
                Crop crop = (Crop)obj;
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
                + $"\n\tPrice: {Price} {Item.Name} {Item.DisplayName}";
        }

        ///to string method prints out all the data for the crop
        ///
    }
}