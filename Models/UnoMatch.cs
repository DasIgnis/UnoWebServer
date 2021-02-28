using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class UnoMatch: IMatch
    {
        public Guid Id { get; set; }
        public List<Guid> Players { get; set; }
        public UnoMatch()
        {
            Players = new List<Guid>();
        }

        public Guid getId()
        {
            return Id;
        }

        public List<Guid> getPlayers()
        {
            return Players;
        }
    }
}
