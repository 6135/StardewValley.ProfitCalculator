using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitCalculator.UI
{
    internal class CropBox : BaseOption
    {
        private Texture2D Tex;
        private SpriteFont Font = Game1.dialogueFont;
        private string mainText;

        public CropBox(int x, int y, int w, int h, Func<string> name, Func<string> label, Func<string> tooltip) : base(x, y, w, h, name, label, tooltip)
        {
        }

        public override void Draw(SpriteBatch b)
        {
            b.Draw(
                this.Tex,
                this.Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.25f
                );

            // Copied from game code - caret and https://github.com/spacechase0/StardewValleyMods/blob/develop/SpaceShared/UI/Element.cs#L91
            Vector2 vector2;
            float writeBarOffset = 26f;
            for (vector2 = this.Font.MeasureString(mainText); vector2.X > (float)this.Tex.Width - writeBarOffset; vector2 = this.Font.MeasureString(mainText))
                mainText = mainText.Substring(1);

            b.DrawString(this.Font, mainText, this.Position + new Vector2(16, 12), Game1.textColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.35f);
        }
    }
}