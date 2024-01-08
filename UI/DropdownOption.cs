using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfitCalculator.helper;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProfitCalculator.UI
{
    internal class DropdownOption : BaseOption
    {
        /*********
        ** Accessors
        *********/
        public int RequestWidth { get; set; }
        public int MaxValuesAtOnce { get; set; } = 5;
        public Texture2D Texture { get; set; } = Game1.mouseCursors;
        public Rectangle BackgroundTextureRect { get; set; } = OptionsDropDown.dropDownBGSource;
        public Rectangle ButtonTextureRect { get; set; } = OptionsDropDown.dropDownButtonSource;

        public string Value
        {
            get => this.Choices[this.ActiveChoice];
            set { if (this.Choices.Contains(value)) this.ActiveChoice = Array.IndexOf(this.Choices, value); }
        }

        public int Width => Math.Max(300, Math.Min(300, this.RequestWidth));

        public int Height => 44;

        public string Label => this.Labels[this.ActiveChoice];

        public int ActiveChoice { get; set; }

        public int ActivePosition { get; set; }
        public string[] Choices { get; set; } = new[] { "null" };

        public string[] Labels { get; set; } = new[] { "null" };

        public Action<string> ValueSetter;
        public bool Dropped;

        public static DropdownOption ActiveDropdown;
        public static int SinceDropdownWasActive = 0;

        public DropdownOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<string[]> choices,
            Func<string[]> labels,
            Func<string> valueGetter,
            Action<string> valueSetter
        ) : base(x, y, 0, 0, name, label, label)
        {
            this.Choices = choices();
            this.Labels = labels();
            this.ActiveChoice = Array.IndexOf(this.Choices, valueGetter());
            ValueSetter = valueSetter;
            ClickableComponent.bounds.Width = this.Width;
            ClickableComponent.bounds.Height = this.Height;
        }

        /// <inheritdoc />
        public override string ClickedSound => "shwip";

        /*********
        ** Public methods
        *********/

        /// <inheritdoc />
        public override void Update()
        {
            bool justClicked = false;
            if (this.Clicked && ActiveDropdown == null)
            {
                justClicked = true;
                this.Dropped = true;
            }

            if (this.Dropped)
            {
                if (Constants.TargetPlatform != GamePlatform.Android)
                {
                    if ((Mouse.GetState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released ||
                         Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && Game1.oldPadState.Buttons.A == ButtonState.Released)
                        && !justClicked)
                    {
                        Game1.playSound("drumkit6");
                        this.Dropped = false;
                    }
                }
                else
                {
                    if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released ||
                         Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && Game1.oldPadState.Buttons.A == ButtonState.Released)
                        && !justClicked)
                    {
                        Game1.playSound("drumkit6");
                        this.Dropped = false;
                    }
                }

                int tall = Math.Min(this.MaxValuesAtOnce, this.Choices.Length - this.ActivePosition) * this.Height;
                int drawY = Math.Min((int)this.Position.Y, Game1.uiViewport.Height - tall);
                var bounds2 = new Rectangle((int)this.Position.X, drawY, this.Width, this.Height * this.MaxValuesAtOnce);
                if (bounds2.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    int choice = (Game1.getOldMouseY() - drawY) / this.Height;
                    this.ActiveChoice = choice + this.ActivePosition;
                    this.ValueSetter(this.Choices[this.ActiveChoice]);
                }
            }

            if (this.Dropped)
            {
                DropdownOption.ActiveDropdown = this;
                DropdownOption.SinceDropdownWasActive = 3;
            }
            else
            {
                if (DropdownOption.ActiveDropdown == this)
                    DropdownOption.ActiveDropdown = null;
                this.ActivePosition = Math.Min(this.ActiveChoice, this.Choices.Length - this.MaxValuesAtOnce);
            }
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            if (this.Dropped)
                this.ActivePosition = Math.Min(Math.Max(this.ActivePosition - (direction / 120), 0), this.Choices.Length - this.MaxValuesAtOnce);
            else
                DropdownOption.ActiveDropdown = null;
        }

        public override void Draw(SpriteBatch b)
        {

            IClickableMenu.drawTextureBox(b, this.Texture, this.BackgroundTextureRect, (int)this.Position.X, (int)this.Position.Y, this.Width - 48, this.Height, Color.White, 4, false, 0.97f);
            b.DrawString(Game1.smallFont, this.Label, new Vector2(this.Position.X + 4, this.Position.Y + 8), Game1.textColor);
            b.Draw(this.Texture, new Vector2(this.Position.X + this.Width - 48, this.Position.Y), this.ButtonTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);

            if (this.Dropped)
            {
                int maxValues = this.MaxValuesAtOnce;
                int start = this.ActivePosition;
                int end = Math.Min(this.Choices.Length, start + maxValues);
                int tall = Math.Min(maxValues, this.Choices.Length - this.ActivePosition) * this.Height;
                int drawY = Math.Min((int)this.Position.Y, Game1.uiViewport.Height - tall);
                IClickableMenu.drawTextureBox(b, this.Texture, this.BackgroundTextureRect, (int)this.Position.X, drawY, this.Width - 48, tall, Color.White, 4, false);
                for (int i = start; i < end; ++i)
                {
                    if (i == this.ActiveChoice)
                        b.Draw(Game1.staminaRect, new Rectangle((int)this.Position.X + 4, drawY + (i - this.ActivePosition) * this.Height, this.Width - 48 - 8, this.Height), null, Color.Wheat, 0, Vector2.Zero, SpriteEffects.None, 0.981f);
                    b.DrawString(Game1.smallFont, this.Labels[i], new Vector2(this.Position.X + 4, drawY + (i - this.ActivePosition) * this.Height + 8), Game1.textColor, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                }
            }
        }

        public override void beforeReceiveLeftClick(int x, int y)
        {
            base.beforeReceiveLeftClick(x, y);
            //if outside the bounds of the dropdown, close it and set dropped to false, and active to null
            if (this.Clicked && !this.ClickableComponent.containsPoint(x, y))
            {
                this.Clicked = false;
                this.Dropped = false;
                DropdownOption.ActiveDropdown = null;
            }
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            this.beforeReceiveLeftClick(x, y);
            //if it is clicked. execute the click
            if (this.ClickableComponent.containsPoint(x, y))
            {
                if (this.Dropped || this.Clicked)
                {
                    this.Dropped = false;
                    this.Clicked = false;
                }
                else this.executeClick();
            }
        }
    }
}