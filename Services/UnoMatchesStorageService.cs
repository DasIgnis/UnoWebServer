using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Models;

namespace UnoServer.Services
{
    public class UnoMatchesStorageService: IMatchesStorageService
    {
        private List<Guid> _matchQueue = new List<Guid>();
        private List<Guid> _matchedUsers = new List<Guid>();
        private List<UnoMatch> _matches = new List<UnoMatch>();

        public void Enqueue(Guid user)
        {
            _matchQueue.Add(user);
        }

        public bool IsEnqued(Guid user)
        {
            return _matchQueue.Contains(user);
        }

        public Guid? TryStartMatch(Guid user)
        {
            if (_matchedUsers.Contains(user))
            {
                UnoMatch existing = _matches.Find(x => x.getPlayers().Any(player => player.Equals(user)));
                return existing?.getId();
            }

            if (!_matchQueue.Contains(user) || _matchQueue.Count == 1)
            {
                return null;
            }

            _matchQueue.Remove(user);
            Guid opponent = _matchQueue.First();
            _matchQueue.Remove(opponent);

            _matchedUsers.Add(user);
            _matchedUsers.Add(opponent);

            UnoMatch match = new UnoMatch
            {
                Id = Guid.NewGuid(),
                Players = new List<Guid>
                {
                    user,
                    opponent
                }
            };

            _matches.Add(match);

            return match.Id;
        }
    }
}
