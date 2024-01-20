using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitCalculator.ui
{
    public interface IDrawable
    {
        /// <summary>
        /// Draws the object.
        /// </summary>
        /// <param name="b"> The spritebatch to draw to. </param>
        void Draw(SpriteBatch b);

        /// <summary>
        /// Updates the state of the object.
        /// </summary>
        void Update();

        /// <summary>
        /// Resets the state of the object to its default state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Called when the game window size changes.
        /// </summary>
        void GameWindowSizeChanged();
    }
}