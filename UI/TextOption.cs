using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfitCalculator.helper;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ProfitCalculator.UI
{
    internal class TextOption : BaseOption, IKeyboardSubscriber
    {
        private Texture2D Tex = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        private SpriteFont Font = Game1.smallFont;
        protected bool SelectedImpl;

        protected readonly Func<string> ValueGetter;
        protected readonly Action<string> ValueSetter;

        public bool Selected
        {
            get => this.SelectedImpl;
            set
            {
                if (this.SelectedImpl == value)
                    return;

                this.SelectedImpl = value;
                if (this.SelectedImpl)
                    Game1.keyboardDispatcher.Subscriber = this;
                else
                {
                    if (Game1.keyboardDispatcher.Subscriber == this)
                        Game1.keyboardDispatcher.Subscriber = null;
                }
            }
        }


        public TextOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<string> valueGetter,
            Action<string> valueSetter
         ) : base(x, y, 192, 48, name, label, label)
        {
            this.ValueGetter = valueGetter;
            this.ValueSetter = valueSetter;
        }

        public void setTexture(Texture2D tex)
        {
            this.Tex = tex;
            ClickableComponent.bounds.Width = tex.Width;
            ClickableComponent.bounds.Height = tex.Height;
        }

        public void setFont(SpriteFont font)
        {
            this.Font = font;
        }
        public override void Draw(SpriteBatch b)
        {
            b.Draw(this.Tex, this.Position, Color.White);

            // Copied from game code - caret and https://github.com/spacechase0/StardewValleyMods/blob/develop/SpaceShared/UI/Element.cs#L91
            string text = this.ValueGetter();
            Vector2 vector2;
            float writeBarOffset = 26f;
            for (vector2 = this.Font.MeasureString(text); vector2.X > (float)this.Tex.Width - writeBarOffset; vector2 = this.Font.MeasureString(text))
                text = text.Substring(1);
            if (DateTime.UtcNow.Millisecond % 1000 >= 500 && this.Selected)
                b.Draw(
                    Game1.staminaRect,
                    new Rectangle(
                        (int)this.Position.X + 16 + (int)vector2.X + 2,
                        (int)this.Position.Y + 8,
                        4,
                        32
                    ),
                    Game1.textColor
                );

            b.DrawString(this.Font, text, this.Position + new Vector2(16, 12), Game1.textColor);
        }

        /// <inheritdoc />
        public void RecieveTextInput(char inputChar)
        {

            this.ReceiveInput(inputChar.ToString());

            // Copied from game code
            switch (inputChar)
            {
                case '"':
                    return;

                case '$':
                    Game1.playSound("money");
                    break;

                case '*':
                    Game1.playSound("hammer");
                    break;

                case '+':
                    Game1.playSound("slimeHit");
                    break;

                case '<':
                    Game1.playSound("crystal");
                    break;

                case '=':
                    Game1.playSound("coin");
                    break;

                default:
                    Game1.playSound("cowboy_monsterhit");
                    break;
            }
        }

        /// <inheritdoc />
        public void RecieveTextInput(string text)
        {

            this.ReceiveInput(text);
        }

        /// <inheritdoc />
        public virtual void RecieveCommandInput(char command)
        {
            if (command == '\b' && this.ValueGetter().Length > 0)
            {
                Game1.playSound("tinyWhip");
                this.ValueSetter(this.ValueGetter().Substring(0, this.ValueGetter().Length - 1));
            }
        }

        /// <inheritdoc />
        public virtual void RecieveSpecialInput(Keys key)
        {
        }

        /*********
        ** Protected methods
        *********/

        protected virtual void ReceiveInput(string str)
        {
            //this.String += str; to value setter and getter
            this.ValueSetter(this.ValueGetter() + str);
        }

        public override void beforeReceiveLeftClick(int x, int y)
        {
            base.beforeReceiveLeftClick(x, y);
            if (this.Selected && !this.ClickableComponent.containsPoint(x, y))
            {
                this.Selected = false;
            }
        }

        public override void executeClick()
        {
            base.executeClick();
            this.Selected = true;
        }
    }
}