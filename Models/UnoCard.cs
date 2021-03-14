using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class UnoCard : IEquatable<UnoCard>
    {
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
    }

    public enum UnoCardType
    {
        Numeric = 0,
        Reverse = 1,
        Skip,
        TakeTwo,
        TakeFourChooseColor,
        ChooseColor
    }

    public enum UnoCardColor
    {
        Red = 0,
        Green = 1,
        Blue,
        Yellow
    }
}
