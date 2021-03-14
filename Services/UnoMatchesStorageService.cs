using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Models;

namespace UnoServer.Services
{
    public class UnoMatchesStorageService
    {
        private List<Guid> _matchQueue = new List<Guid>();
        private List<Guid> _matchedUsers = new List<Guid>();
        private List<UnoMatch> _matches = new List<UnoMatch>();
        private Dictionary<Guid, string> _users = new Dictionary<Guid, string>();
        private List<UnoMatchDetails> _passedMatches = new List<UnoMatchDetails>();

        private const int MAXIMUM_MATCH_COUNT = 3;

        public void Enqueue(Guid user, string name)
        {
            _users.Add(user, name);
            _matchQueue.Add(user);
        }

        public bool IsEnqued(Guid user)
        {
            return _matchQueue.Contains(user);
        }

        public Guid? TryStartMatch(Guid user, Guid? opponent)
        {
            if (_matchedUsers.Contains(user))
            {
                UnoMatch existing = _matches.Find(x => x.Players.Any(player => player.Equals(user)));
                return existing?.Id;
            }

            if (!_matchQueue.Contains(user) || _matchQueue.Count == 1)
            {
                _matchedUsers.Add(user);
                return null;
            }

            _matchQueue.Remove(user);

            if (!opponent.HasValue || !_matchQueue.Contains(opponent.Value))
            {
                int opponentIndex = 0;
                bool matchOverflow = false;

                do
                {
                    opponent = _matchQueue[opponentIndex];
                    matchOverflow = _passedMatches
                        .Where(match => match.Players.Contains(opponent.Value) && match.Players.Contains(user))
                        .Count() == MAXIMUM_MATCH_COUNT;
                    opponentIndex++;
                } while (matchOverflow || opponentIndex < _matchQueue.Count);
            }
            _matchQueue.Remove(opponent.Value);

            _matchedUsers.Add(user);
            _matchedUsers.Add(opponent.Value);

            UnoMatch match = new UnoMatch(Guid.NewGuid(), new List<Guid> { user, opponent.Value });

            _matches.Add(match);

            return match.Id;
        }

        public UnoMatch FindMatch(Guid matchId)
        {
            return _matches.Find(x => x.Id.Equals(matchId));
        }

        public void FinishMatch(Guid matchId)
        {
            UnoMatch match = _matches.Find(x => x.Id.Equals(matchId));

            if (match == null)
            {
                return;
            }

            _matches.Remove(match);
            _passedMatches.Add(new UnoMatchDetails
            {
                Players = match.Players,
                Winner = match.CurrentPlayer
            });

            match.Players.ForEach(player =>
            {
                _matchedUsers.Remove(player);
                _matchQueue.Add(player);
            });

        }
    }
}
