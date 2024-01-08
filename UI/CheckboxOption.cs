using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfitCalculator.helper;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProfitCalculator.UI
{
    internal class CheckboxOption : BaseOption
    {
        public Texture2D Texture { get; set; } = Game1.mouseCursors;
        public Rectangle CheckedTextureRect { get; set; } = OptionsCheckbox.sourceRectChecked;
        public Rectangle UncheckedTextureRect { get; set; } = OptionsCheckbox.sourceRectUnchecked;

        private readonly Func<bool> ValueGetter;
        private readonly Action<bool> ValueSetter;

        /// <inheritdoc />
        public override string ClickedSound => "drumkit6";

        public CheckboxOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<bool> valueGetter,
            Action<bool> valueSetter)
            : base(x, y, 0, 0, name, label, label)
        {
            ClickableComponent.bounds.Width = OptionsCheckbox.sourceRectChecked.Width * 4;
            ClickableComponent.bounds.Height = OptionsCheckbox.sourceRectChecked.Height * 4;
            ValueGetter = valueGetter;
            ValueSetter = valueSetter;
        }

        public override void Draw(SpriteBatch b)
        {
            Helpers.Monitor.Log($"Drawing checkbox option {this.ValueGetter()}", LogLevel.Debug);
            b.Draw(
                this.Texture,
                this.Position,
                this.ValueGetter() ? this.CheckedTextureRect : this.UncheckedTextureRect,
                Color.White,
                0,
                Vector2.Zero,
                4,
                SpriteEffects.None,
                0
            );
            Game1.activeClickableMenu?.drawMouse(b);
        }

        public override void executeClick()
        {
            this.ValueSetter(!this.ValueGetter());
            Game1.playSound(this.ClickedSound);
        }
    }
}