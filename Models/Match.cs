using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class Match
    {
        public Guid Id { get; set; }
        public List<Guid> Players { get; set; }

        public Match()
        {
            Players = new List<Guid>();
        }
    }
}
