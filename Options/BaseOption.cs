using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitCalculator.Options
{
    public abstract class BaseOption
    {
        public ClickableComponent ClickableComponent { get; set; }

        public enum OptionType
        {
            Checkbox,
            Textbox,
            Numberbox,
            Dropdown,
            BoundNumberbox
        };

        public OptionType optionType { get; set; } = OptionType.Textbox;

        /// <summary>The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</summary>
        public Func<string> Tooltip { get; }

        public Func<string> Name { get; set; }
        public Func<string> Label { get; set; }

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

        protected BaseOption(ClickableComponent clickableComponent, Func<string> name, Func<string> label, Func<string> tooltip)
        {
            ClickableComponent = clickableComponent;
            Tooltip = tooltip;
            this.Name = name;
            this.Label = label;
        }

        /*public abstract void beforeDraw();*/

        public abstract void Draw(SpriteBatch b);

        /*public abstract void beforeReceiveLeftClick();

        public abstract void ReceiveLeftClick(int x, int y);

        public abstract void beforeReceiveRightClick();

        public abstract void ReceiveRightClick(int x, int y);

        public abstract void LeftClickHeld(int x, int y);

        public abstract void LeftClickReleased(int x, int y);

        public abstract void HoverOver(int x, int y);

        public abstract void DrawMouse(SpriteBatch b);*/
    }
}