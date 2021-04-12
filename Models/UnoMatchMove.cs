using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class UnoMatchMove
    {
        public Guid Player { get; set; }
        public UnoCard DeckCard { get; set; }
        public List<UnoCard> Move { get; set; }
        public UnoCardColor SelecteColor { get; set; }
    }
}
