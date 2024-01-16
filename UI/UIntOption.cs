using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Linq;

namespace ProfitCalculator.ui
{
    public class UIntOption : TextOption
    {
        protected readonly Func<uint> Max;
        protected readonly Func<uint> Min;
        protected readonly bool EnableClamping;

        public UIntOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<uint> valueGetter,
            Func<uint> max,
            Func<uint> min,
            Action<string> valueSetter,
            bool enableClamping = true
        ) : base(x, y, name, label, () => valueGetter().ToString(), valueSetter)
        {
            this.Max = max;
            this.Min = min;
            this.EnableClamping = enableClamping;
        }

        /*********
                ** Accessors
                *********/

        public bool IsValid => int.TryParse(this.ValueGetter(), out _);

        /*********
        ** Protected methods
        *********/

        /// <inheritdoc />
        protected override void ReceiveInput(string str)
        {
            bool valid = true;
            //number uintbox not clamped should be able to take any positive number, to the max of uint, and 0.
            //Should not be able to take negative numbers and should not be able to take decimals or empty string, if char not valid then dont add it to string
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (!char.IsDigit(c) && !(c == '-' && this.ValueGetter() == "" && i == 0))
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
                return;
            //if the parsed string equals to utin 0 then set to 0, this should allow for easy clearing of the uintbox by typing 0 and being able to type a new number after that
            if (uint.Parse(this.ValueGetter() + str) == 0)
            {
                this.ValueSetter("0");
                return;
            }
            //if clamping is enabled then clamp the value to the min and max
            if (this.EnableClamping)
            {
                uint val = Math.Clamp(uint.Parse(this.ValueGetter() + str), this.Min(), this.Max());
                this.ValueSetter(val.ToString());
            }
            else
            {
                this.ValueSetter(this.ValueGetter() + str);
            }
        }

        public override void RecieveCommandInput(char command)
        {
            if (command == '\b' && this.ValueGetter().Length > 0)
            {
                Game1.playSound("tinyWhip");
                //if length is 1 then set to 0 or if multiple 0s then set to 0, else remove last char
                if (this.ValueGetter().Length == 1 || this.ValueGetter().All(c => c == '0'))
                    this.ValueSetter("0");
                else
                    this.ValueSetter(this.ValueGetter().Substring(0, this.ValueGetter().Length - 1));
            }
        }

        //allow adding or subtracting from the uintbox by using the arrow keys
        public override void RecieveSpecialInput(Keys key)
        {
            if (key == Keys.Up)
            {
                uint val = Math.Clamp(uint.Parse(this.ValueGetter()) + 1, this.Min(), this.Max());
                this.ValueSetter(val.ToString());
            }
            else if (key == Keys.Down)
            {
                uint val = Math.Clamp(uint.Parse(this.ValueGetter()) - 1, this.Min(), this.Max());
                this.ValueSetter(val.ToString());
            }
        }

        //after select is set to false, set the value to the value getter
        public override void beforeReceiveLeftClick(int x, int y)
        {
            base.beforeReceiveLeftClick(x, y);
            if (!this.Selected && this.EnableClamping)
                this.ValueSetter(
                    Math.Clamp(
                        uint.Parse(this.ValueGetter()),
                        this.Min(),
                        this.Max()
                    ).ToString()
                );
        }
    }
}