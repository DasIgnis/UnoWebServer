﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Controllers;

namespace UnoServer.Models
{
    public class UnoMatchDetails
    {
        public List<Guid> Players { get; set; }
        public Guid Winner { get; set; }
    }
}