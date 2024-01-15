using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfitCalculator.main;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ProfitCalculator.UI
{
    internal class CropBox : BaseOption
    {
        private CropInfo cropInfo;
        private SpriteFont Font = Game1.smallFont;
        private string mainText = "PlaceHolder";

        public CropBox(int x, int y, int w, int h, CropInfo crop) : base(x, y, w, h, () => crop.Crop.Name, () => crop.Crop.Name, () => crop.Crop.Name)
        {
            this.mainText = crop.Crop.Name;
            if(this.mainText.Length < 1)
            {
                this.mainText = "PlaceHolder";
            }
            cropInfo = crop;
        }

        public override void Draw(SpriteBatch b)
        {
            Game1.DrawBox(
                (int)this.Position.X,
                (int)this.Position.Y,
                this.ClickableComponent.bounds.Width,
                this.ClickableComponent.bounds.Height
                );

            b.DrawString(this.Font, mainText, this.Position + new Vector2(16, 12), Game1.textColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.35f);
        }
    }
}