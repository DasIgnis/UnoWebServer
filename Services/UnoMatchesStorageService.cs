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

        private IdentityContext _context;

        private const int MAXIMUM_MATCH_COUNT = 3;

        public UnoMatchesStorageService(IdentityContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _matchQueue = _context.Users
                .Where(x => _context.Matches.All(match => match.Finished || !match.Players.Contains(x)))
                .Select(x => x.ExternalId)
                .ToList();
            _matchedUsers = _context.Users
                .Where(x => !_matchQueue.Contains(x.ExternalId))
                .Select(x => x.ExternalId)
                .ToList();
        }

        public Guid Enqueue(Guid user, string name)
        {
            User existing = _context.Users.Where(x => name == x.UserName).FirstOrDefault();

            if (existing == null)
            {
                _context.Users.Add(
                    new User
                    {
                        ExternalId = user,
                        Name = name
                    });

                _context.SaveChanges();
                return user;
            } else
            {
                return existing.ExternalId;
            }
        }

        public bool IsEnqued(Guid user)
        {
            return _matchQueue.Contains(user);
        }

        public Guid? TryStartMatch(Guid user, Guid? opponent)
        {
            User initiatior = _context.Users.Where(x => x.ExternalId == user).FirstOrDefault();

            if (initiatior == null)
            {
                return null;
            }

            if (_matchedUsers.Contains(user))
            {
                DbMatch existing = _context.Matches
                    .Where(x => x.Players.Any(player => player.Equals(user)))
                    .FirstOrDefault();

                return existing?.Id;
            }

            if (_matchQueue.Count == 1)
            {
                return null;
            }

            if (!opponent.HasValue || !_matchQueue.Contains(opponent.Value))
            {
                int opponentIndex = 0;
                bool matchOverflow = false;

                do
                {
                    if (_matchQueue[opponentIndex] == initiatior.ExternalId)
                    {
                        opponentIndex++;
                        continue;
                    }

                    opponent = _matchQueue[opponentIndex];

                    matchOverflow = _context
                        .Matches
                        .Where(match =>
                            match.Finished
                            && match.Players.Any(dbuser => dbuser.ExternalId == opponent)
                            && match.Players.Any(dbuser => dbuser.ExternalId == user)
                        )
                        .Count() == MAXIMUM_MATCH_COUNT;
                    opponentIndex++;
                } while (matchOverflow || opponentIndex < _matchQueue.Count);
            }

            User opponentor = _context.Users.Where(x => x.ExternalId == opponent.Value).FirstOrDefault();

            UnoMatch match = new UnoMatch(Guid.NewGuid(), new List<Guid> { user, opponent.Value });
            DbMatch dbMatch = new DbMatch
            {
                Id = match.Id,
                Finished = false,
                Deck = match.Deck,
                Discharge = match.Discharge,
                Players = new List<User> { initiatior, opponentor },
                CurrentPlayer = initiatior
            };

            _context.Matches.Add(dbMatch);

            match.Hands
                .Select(mHand => new DbHand
                {
                    Id = Guid.NewGuid(),
                    Match = dbMatch,
                    User = mHand.Key == initiatior.ExternalId ? initiatior : opponentor,
                    Hand = mHand.Value
                })
                .ToList()
                .ForEach(hand => _context.Hands.Add(hand));

            _context.SaveChanges();

            return dbMatch.Id;
        }

        public UnoMatch FindMatch(Guid matchId)
        {
            DbMatch match = _context.Matches
                .Where(x => x.Id == matchId)
                .FirstOrDefault();

            if (match == null)
            {
                return null;
            }

            List<DbHand> hands = _context.Hands.Where(hand => hand.Match.Id == matchId).ToList();

            return new UnoMatch(match, hands);
        }

        public void FinishMatch(Guid matchId)
        {
            DbMatch dbMatch = _context
                .Matches
                .Where(x => x.Id == matchId)
                .FirstOrDefault();

            if (dbMatch == null)
            {
                return;
            }

            User winner = _context.Users.Find(dbMatch.CurrentPlayer);

            if (winner != null)
            {
                dbMatch.Winner = winner;
            }

            dbMatch.Finished = true;
            dbMatch.Discharge.Clear();
            dbMatch.Deck.Clear();

            List<DbHand> hands = _context.Hands.Where(x => x.Match.Id == dbMatch.Id).ToList();
            hands.ForEach(h => _context.Hands.Remove(h));

            _context.SaveChanges();
        }

        public List<UnoMatchDetails> GetPassedMatches()
        {
            return _context
                .Matches
                .Where(match => match.Finished)
                .Select(
                match => new UnoMatchDetails
                {
                    Players = match.Players,
                    Winner = match.Winner
                }
                ).ToList();
        }

        public void SaveMove(UnoMatch match)
        {
            DbMatch dbMatch = _context
                .Matches
                .Where(x => x.Id == match.Id)
                .FirstOrDefault();

            if (dbMatch == null)
            {
                return;
            }

            dbMatch.Backlog = match.Backlog;
            dbMatch.CurrentColor = match.CurrentColor;
            dbMatch.CurrentPlayer = dbMatch.Players.Where(x => x.ExternalId == match.CurrentPlayer).FirstOrDefault();
            dbMatch.Deck = match.Deck;
            dbMatch.Discharge = match.Discharge;

            List<DbHand> hands = _context
                .Hands
                .Where(h => h.Match.Id == match.Id)
                .ToList();

            hands.ForEach(hand =>
            {
                if (match.Hands.ContainsKey(hand.User.ExternalId))
                {
                    hand.Hand = match.Hands[hand.User.ExternalId];
                }
            });

            _context.SaveChanges();
        }
    }
}
