using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitCalculator.Options
{
    //checkmark
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message = "Invalid type for Class") : base(message)
        {
        }
    }

    internal class SimpleOption<T> : BaseOption
    {
        /// <summary>The option value type.</summary>
        public Type Type => typeof(T);

        public virtual T Value { get; set; }

        public SimpleOption(int x, int y, int w, int h, Func<string> name, Func<string> label, Func<string> tooltip, T value) : base(x, y, w, h, name, label, tooltip)
        {
            //if Type is bool, then optionType is Checkbox
            //if Type is uint, then optionType is Numberbox
            //if Type is string, then optionType is Textbox
            //if Type is enum, then optionType is Dropdown
            //if Type is uint[], then optionType is BoundNumberbox
            //else throw exception

            if (Type == typeof(bool))
            {
                optionType = OptionType.Checkbox;
            }
            else if (Type == typeof(uint))
            {
                optionType = OptionType.Numberbox;
            }
            else if (Type == typeof(string))
            {
                optionType = OptionType.Textbox;
            }
            else if (Type.IsEnum)
            {
                optionType = OptionType.Dropdown;
            }
            else if (Type == typeof(uint[]))
            {
                optionType = OptionType.BoundNumberbox;
            }
            else
            {
                throw new InvalidTypeException("Invalid type for SimpleOption");
            }

            Value = value;
        }

        public SimpleOption(ClickableComponent clickableComponent, Func<string> name, Func<string> label, Func<string> tooltip, T value) : base(clickableComponent, name, label, tooltip)
        {
            //if Type is bool, then optionType is Checkbox
            //if Type is uint, then optionType is Numberbox
            //if Type is string, then optionType is Textbox
            //if Type is enum, then optionType is Dropdown
            //if Type is uint[], then optionType is BoundNumberbox
            //else throw exception

            if (Type == typeof(bool))
            {
                optionType = OptionType.Checkbox;
            }
            else if (Type == typeof(uint))
            {
                optionType = OptionType.Numberbox;
            }
            else if (Type == typeof(string))
            {
                optionType = OptionType.Textbox;
            }
            else if (Type.IsEnum)
            {
                optionType = OptionType.Dropdown;
            }
            else if (Type == typeof(uint[]))
            {
                optionType = OptionType.BoundNumberbox;
            }
            else
            {
                throw new InvalidTypeException("Invalid type for SimpleOption");
            }

            Value = value;
        }

        public override void Draw(SpriteBatch b)
        {
            //draw the clickable component according to optionType
            switch (optionType)
            {
                case OptionType.Checkbox:
                    DrawCheckbox(b);
                    break;

                case OptionType.Textbox:
                    DrawTextbox(b);
                    break;

                case OptionType.Numberbox:
                    DrawNumberbox(b);
                    break;

                case OptionType.Dropdown:
                    DrawDropdown(b);
                    break;

                case OptionType.BoundNumberbox:
                    DrawNumberbox(b);
                    break;

                default:
                    throw new Exception("Invalid optionType");
            }
        }

        private void DrawCheckbox(SpriteBatch b)
        {
            throw new NotImplementedException();
        }

        private void DrawTextbox(SpriteBatch b)
        {
            throw new NotImplementedException();
        }

        //Can be both bound and unbound, always uint
        private void DrawNumberbox(SpriteBatch b)
        {
            throw new NotImplementedException();
        }

        private void DrawDropdown(SpriteBatch b)
        {
            throw new NotImplementedException();
        }
    }
}