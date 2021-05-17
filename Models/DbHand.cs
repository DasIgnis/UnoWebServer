using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class DbHand
    {
        public Guid Id { get; set; }
        public virtual DbMatch Match { get; set; }
        public virtual User User { get; set; }
        public virtual IList<UnoCard> Hand { get; set; }
    }
}
