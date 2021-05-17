using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    public class User: IdentityUser
    {
        public string Name { get; set; }
        public Guid ExternalId { get; set; }
    }
}
