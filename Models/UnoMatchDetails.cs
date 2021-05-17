using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Controllers;

namespace UnoServer.Models
{
    public class UnoMatchDetails
    {
        public virtual List<User> Players { get; set; }
        public User Winner { get; set; }
    }
}
