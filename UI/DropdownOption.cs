using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;

namespace ProfitCalculator.ui
{
    public class DropdownOption : BaseOption
    {
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

        public int DropDownBoxWidth => Math.Max(300, Math.Min(300, this.RequestWidth));

        public int DropDownBoxHeight => 44;

        public new string Label => this.Labels[this.ActiveChoice];

        public int ActiveChoice { get; set; }

        public int ActivePosition { get; set; }
        public string[] Choices { get; set; }

        public string[] Labels { get; set; }

        public Action<string> ValueSetter;
        public bool Dropped;

#pragma warning disable S2223 // Non-constant static fields should not be visible
        public static DropdownOption ActiveDropdown = null;
        public static int SinceDropdownWasActive = 0;
#pragma warning restore S2223 // Non-constant static fields should not be visible

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
            ClickableComponent.bounds.Width = this.DropDownBoxWidth;
            ClickableComponent.bounds.Height = this.DropDownBoxHeight;
        }

        public override string ClickedSound => "shwip";

        public override void Update()
        {
            base.Update();
            bool justClicked = false;

            if (this.Clicked && DropdownOption.ActiveDropdown == null)
            {
                justClicked = true;
                this.Dropped = true;
            }

            if (this.Dropped)
            {
                if (Constants.TargetPlatform != GamePlatform.Android)
                {
                    //print all checked values

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
                int tall = Math.Min(this.MaxValuesAtOnce, this.Choices.Length - this.ActivePosition) * this.DropDownBoxHeight;
                int drawY = Math.Min((int)this.Position.Y, Game1.uiViewport.Height - tall);
                var bounds2 = new Rectangle((int)this.Position.X, drawY, this.DropDownBoxWidth, this.DropDownBoxHeight * this.MaxValuesAtOnce);
                if (bounds2.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    int choice = (Game1.getOldMouseY() - drawY) / this.DropDownBoxHeight;
                    this.ActiveChoice = choice + this.ActivePosition;
                    this.ValueSetter(this.Choices[this.ActiveChoice]);
                }

                DropdownOption.ActiveDropdown = this;
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
            IClickableMenu.drawTextureBox(
                b, this.Texture,
                this.BackgroundTextureRect,
                (int)this.Position.X,
                (int)this.Position.Y,
                this.DropDownBoxWidth - 48,
                this.DropDownBoxHeight,
                Color.White,
                4,
                false,
                0.5f);//small clicable initial box

            b.DrawString(
                Game1.smallFont,
                this.Label,
                new Vector2(this.Position.X + 4, this.Position.Y + 8),
                Game1.textColor,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.55f
             ); //Selected text
            b.Draw(
                this.Texture,
                new Vector2(this.Position.X + this.DropDownBoxWidth - 48, this.Position.Y),
                this.ButtonTextureRect,
                Color.White,
                0,
                Vector2.Zero,
                4,
                SpriteEffects.None,
                0f
            ); //Dropdown arrow

            if (this.Dropped)
            {
                int maxValues = this.MaxValuesAtOnce;
                int start = this.ActivePosition;
                int end = Math.Min(this.Choices.Length, start + maxValues);
                int tall = Math.Min(maxValues, this.Choices.Length - this.ActivePosition) * this.DropDownBoxHeight;
                int drawY = Math.Min((int)this.Position.Y, Game1.uiViewport.Height - tall);
                IClickableMenu.drawTextureBox(
                    b,
                    this.Texture,
                    this.BackgroundTextureRect,
                    (int)this.Position.X,
                    drawY,
                    this.DropDownBoxWidth - 48,
                    tall,
                    Color.White, 4,
                    false,
                    0.6f); // Dropdown box with options
                for (int i = start; i < end; ++i)
                {
                    if (i == this.ActiveChoice)
                        b.Draw(
                            Game1.staminaRect,
                            new Rectangle((int)this.Position.X + 4,
                            drawY + (i - this.ActivePosition) * this.DropDownBoxHeight,
                            this.DropDownBoxWidth - 48 - 8, this.DropDownBoxHeight),
                            null,
                            Color.Wheat,
                            0,
                            Vector2.Zero,
                            SpriteEffects.None,
                            0.65f
                        ); // Selected option
                    b.DrawString(
                        Game1.smallFont,
                        this.Labels[i],
                        new Vector2(this.Position.X + 4, drawY + (i - this.ActivePosition) * this.DropDownBoxHeight + 8),
                        Game1.textColor,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.7f
                    );
                }
            }
        }

        public override void ReceiveLeftClick(int x, int y, Action stopSpread)
        {
            //if ClickMeantToCloseDropdown is false then open dropdown
            if (ClickableComponent.containsPoint(x, y) && !this.Dropped && !this.Clicked)
            {
                this.executeClick();
            }
            else if (this.Dropped || this.Clicked)
            {
                this.Dropped = false;
                this.Clicked = false;
                stopSpread();
            }
        }
    }
}