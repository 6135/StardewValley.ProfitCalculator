using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewValley.Menus;
using ProfitCalculator.main;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ProfitCalculator.ui
{
    /// <summary>
    ///   Hover details for each crop in the profit calculator.
    /// </summary>
    public class CropHoverBox : IDisposable, IDrawable
    {
        private bool isOpen;
        private int windowWidth;
        private int windowHeight;
        private int x;
        private int y;

        private Rectangle drawBox;
        private readonly CropInfo cropInfo;

        public CropHoverBox(CropInfo cropInfo)
        {
            isOpen = false;
            windowWidth = 100;
            windowHeight = 100;
            x = 0;
            y = 0;
            drawBox = new(x, y, windowWidth, windowHeight);
            this.cropInfo = cropInfo;
        }

        /// <inheritdoc/>

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            isOpen = false;
        }

        /// <inheritdoc/>

        public void Draw(SpriteBatch b)
        {
            if(isOpen)
            {
                IClickableMenu.drawTextureBox(
                    b,
                    Game1.menuTexture,
                    new Rectangle(0, 256, 60, 60),
                    drawBox.X,
                    drawBox.Y,
                    300,
                    100,
                    Color.White,
                    1f
                );

            }
        }

        /// <inheritdoc/>

        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>

        public void Update()
        {
            //x and y set to near the mouse
            //if mouse is near the edge of the screen, move the box to the other side of the mouse
            Rectangle safeArea = Utility.getSafeArea();

            int mouseX = Game1.getMouseX() + Game1.tileSize;
            int mouseY = Game1.getMouseY() + Game1.tileSize;


            if (mouseX + windowWidth > safeArea.Right)
                x = mouseX - windowWidth;
            else
                x = mouseX;

            if (mouseY + windowHeight > safeArea.Bottom)
                y = mouseY - windowHeight;
            else
                y = mouseY;

            //if the box is off the screen, move it back on
            /*if (x < safeArea.Left)
                x = safeArea.Left;
            if (y < safeArea.Top)
                y = safeArea.Top;*/

            drawBox = new(
                x,
                y,
                windowWidth,
                windowHeight
            );
        }

        /// <inheritdoc/>

        public void GameWindowSizeChanged()
        {
        }

        public void Open(bool open = false)
        {
            isOpen = open;
        }
    }
}