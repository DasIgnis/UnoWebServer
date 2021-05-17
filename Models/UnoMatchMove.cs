using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class UnoMatchMove
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public virtual User Player { get; set; }
        public virtual UnoCard DeckCard { get; set; }
        public virtual List<UnoCard> Move { get; set; }
        public UnoCardColor SelecteColor { get; set; }
    }
}
