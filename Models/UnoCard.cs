using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Services;

namespace UnoServer.Models
{
    public class UnoCard : IEquatable<UnoCard>
    {
        public Guid Id { get; set; }
        public UnoCardType Type { get; set; }
        public int NumberValue { get; set; }
        public UnoCardColor Color { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnoCard);
        }

        public bool Equals(UnoCard other)
        {
            if (Type != UnoCardType.ChooseColor || Type != UnoCardType.TakeFourChooseColor)
            {
                return other != null
                    && Type == other.Type
                    && NumberValue == other.NumberValue
                    && Color == other.Color;
            }
            else
            {
                return other != null
                    && Type == other.Type;
            }
        }

        public override string ToString()
        {
            return $"{Type.GetDescription()} {NumberValue} {Color.GetDescription()}";
        }
    }

    public enum UnoCardType
    {
        [Description("Number")]
        Numeric = 0,
        [Description("Reverse")]
        Reverse = 1,
        [Description("Skip move")]
        Skip,
        [Description("Take two")]
        TakeTwo,
        [Description("Take four and choose color")]
        TakeFourChooseColor,
        [Description("Choose color")]
        ChooseColor
    }

    public enum UnoCardColor
    {
        [Description("Red")]
        Red = 0,
        [Description("Green")]
        Green = 1,
        [Description("Blue")]
        Blue,
        [Description("Yellow")]
        Yellow
    }
}
