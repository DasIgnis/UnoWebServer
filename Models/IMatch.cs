﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Models
{
    interface IMatch
    {
        Guid getId();
        List<Guid> getPlayers();
    }
}
