using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnoServer.Services
{
    interface IMatchesStorageService
    {
        void Enqueue(Guid user);
        bool IsEnqued(Guid user);
        Guid? TryStartMatch(Guid user);
    }
}
