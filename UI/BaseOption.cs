using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitCalculator.UI
{
    public abstract class BaseOption
    {
        private ClickableComponent clickableComponent;
        public virtual string ClickedSound => null;
        protected bool Clicked;
        public virtual string HoveredSound => null;
        public bool Hover { get; private set; }

        public bool ClickGestured { get; private set; }
        public void setClickableComponent(Vector2 position, Vector2 Size)
        {
            ClickableComponent = new(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)Size.X,
                    (int)Size.Y
                ),
                Name(),
                Name()
            );
        }
        public ClickableComponent ClickableComponent
        {
            get => clickableComponent;
            set
            {
                clickableComponent = value;
                Position = new Vector2(clickableComponent.bounds.X, clickableComponent.bounds.Y);
            }
        }

        /// <summary>The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</summary>
        public Func<string> Tooltip { get; }

        public Func<string> Name { get; set; }
        public Func<string> Label { get; set; }

        public Vector2 Position { get; set; }

        protected BaseOption(int x, int y, int w, int h, Func<string> name, Func<string> label, Func<string> tooltip)
        {
            ClickableComponent =
                new ClickableComponent(
                    new Rectangle(
                        x,
                        y,
                        w,
                        h
                    ),
                    name(),
                    label()
                );
            Tooltip = tooltip;
            this.Name = name;
            this.Label = label;
        }

        public abstract void Draw(SpriteBatch b);

        public virtual void beforeReceiveLeftClick(int x, int y)
        {
        }

        public virtual void ReceiveLeftClick(int x, int y, Action stopSpread)
        {
            this.beforeReceiveLeftClick(x, y);
            //check if x and y are within the bounds of the checkbox
            if (ClickableComponent.containsPoint(x, y))
                this.executeClick();
        }

        public virtual void executeClick()
        {
            Clicked = true;
            if (this.ClickedSound != null)
                Game1.playSound(this.ClickedSound);
        }

        public virtual void Update()
        {
            //nothing here, optional override for subclasses
        }
    }
}