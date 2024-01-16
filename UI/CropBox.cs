using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfitCalculator.main;
using StardewValley;

namespace ProfitCalculator.ui
{
    public class CropBox : BaseOption
    {
        private CropInfo cropInfo;
        private SpriteFont Font = Game1.smallFont;
        private string mainText = "PlaceHolder";

        public CropBox(int x, int y, int w, int h, CropInfo crop) : base(x, y, w, h, () => crop.Crop.Name, () => crop.Crop.Name, () => crop.Crop.Name)
        {
            this.mainText = crop.Crop.Name;
            if (this.mainText.Length < 1)
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
            //draw crop sprite in the middle of the box aligned to the left
            int spriteSize = 16;
            int spriteDisplaySize = (int)(spriteSize * 3.25);

            b.Draw(
                Game1.objectSpriteSheet,
                new Rectangle(
                    (int)this.Position.X + 8,
                    (int)this.Position.Y + (this.ClickableComponent.bounds.Height / 2) - (Game1.tileSize / 2) + 6,
                    spriteDisplaySize,
                    spriteDisplaySize
                ),
                Game1.getSourceRectForStandardTileSheet(
                    Game1.objectSpriteSheet,
                    cropInfo.Crop.Id,
                    spriteSize,
                    spriteSize
                ),
                Color.White
            );

            //draw string in middle of box, aligned to the left with a spacing of 2xtilesize from the left
            b.DrawString(
                Font,
                this.mainText,
                new Vector2(
                    this.Position.X + 10 + Game1.tileSize,
                    this.Position.Y + (this.ClickableComponent.bounds.Height / 2) - (Font.MeasureString(this.mainText).Y / 2)
                ),
                Color.Black
           );
            //draw vertical stamina bar separating the text from the right side of the box
            /**/
            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)this.Position.X + Game1.tileSize,
                    (int)this.Position.Y,
                    2,
                    this.ClickableComponent.bounds.Height
                ),
                Color.DarkOrange
            );
        }
    }
}