using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class UnoCard
    {
        public UnoCardType Type { get; set; }
        public int NumberValue { get; set; }
        public UnoCardColor Color { get; set; }
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
