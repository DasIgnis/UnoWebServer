using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class DbMatch
    {
        public Guid Id { get; set; }
        public virtual List<User> Players { get; set; }
        public bool Finished { get; set; }
        public virtual User Winner { get; set; }
        public virtual IList<UnoCard> Deck { get; set; }
        public virtual IList<UnoCard> Discharge { get; set; }
        public virtual User CurrentPlayer { get; set; }
        public UnoCardColor CurrentColor { get; set; }
        public virtual IList<UnoMatchMove> Backlog { get; set; }
    }
}
