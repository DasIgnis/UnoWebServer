using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Services;

namespace UnoServer.Models
{
    public class UnoMatch
    {
        public Guid Id { get; set; }
        public List<Guid> Players { get; set; }
        public List<UnoCard> Deck { get; set; }
        public List<UnoCard> Discharge { get; set; }
        public Dictionary<Guid, List<UnoCard>> Hands { get; set; }
        public Guid CurrentPlayer { get; set; }
        public UnoCardColor CurrentColor { get; set; }

        public List<UnoMatchMove> Backlog { get; set; }

        public UnoMatch()
        {
            Players = new List<Guid>();
            Deck = GenerateDeck();
            Discharge = new List<UnoCard>();
            Hands = new Dictionary<Guid, List<UnoCard>>();
            Backlog = new List<UnoMatchMove>();
        }

        public UnoMatch(Guid id, List<Guid> players)
        {
            Id = id;
            Players = players;

            Deck = GenerateDeck();
            Discharge = new List<UnoCard>();
            Hands = new Dictionary<Guid, List<UnoCard>>();

            Players.ForEach(player => {
                Hands.Add(player, Deck.Take(7).ToList());
                Deck = Deck.Skip(7).ToList();
            });

            UnoCard first = Deck.First();
            while (first.Type != UnoCardType.Numeric)
            {
                Deck.RemoveAt(0);
                Deck.Add(first);
                first = Deck.First();
            }
            Deck.RemoveAt(0);
            Discharge.Add(first);
            CurrentPlayer = Players.First();
            Backlog = new List<UnoMatchMove>();
        }

        public UnoMatch(DbMatch dbMatch, List<DbHand> hands)
        {
            Id = dbMatch.Id;
            Players = dbMatch.Players.Select(x => x.ExternalId).ToList();
            Deck = dbMatch.Deck.ToList();
            Discharge = dbMatch.Discharge.ToList();
            Hands = hands.ToDictionary(hand => hand.User.ExternalId, hand => hand.Hand.ToList());
            CurrentPlayer = dbMatch.CurrentPlayer.ExternalId;
            CurrentColor = dbMatch.CurrentColor;
            Backlog = dbMatch.Backlog.ToList();
        }

        public List<UnoCard> GetHand(Guid player)
        {
            return Hands[player];
        }

        public UnoCard GetCurrentCard()
        {
            return Discharge.Last();
        }

        public bool ValidateMove(List<UnoCard> cards)
        {
            //TODO: check if user hand has all cards in list
            int currentValidatingCard = 0;
            UnoCard lastCard = GetCurrentCard();
            bool validationFlag = true;

            do
            {
                UnoCard currentCard = cards[currentValidatingCard];
                //TODO: validation
                currentValidatingCard++;
            } while (currentValidatingCard < cards.Count());

            return validationFlag;
        }

        public MoveStatus Move(
            List<UnoCard> cards, 
            UnoCardColor color, 
            IdentityContext context)
        {

            DbMatch dbMatch = context.Matches.Where(x => x.Id == Id).FirstOrDefault();
            List<DbHand> hands = context.Hands.Where(x => x.Match.Id == Id).ToList();
            if (dbMatch == null)
            {
                return MoveStatus.WRONG_MOVE;
            }

            User player;

            if (cards.Count == 0)
            {
                bool hasMove = false;

                foreach (var card in Hands[CurrentPlayer])
                {
                    //hasMove = hasMove || ValidateMove(new List<UnoCard> { card });
                }

                if (hasMove)
                {
                    return MoveStatus.FAKE_EMPTY_MOVE;
                } 
                else
                {
                    TakeCard(CurrentPlayer, dbMatch, hands);
                    CurrentPlayer = NextPlayer();
                    player = context.Users.Where(x => x.ExternalId == CurrentPlayer).FirstOrDefault();
                    dbMatch.CurrentPlayer = player;
                    return MoveStatus.SUCCESS;
                }
            }

            if (!ValidateMove(cards))
            {
                return MoveStatus.WRONG_MOVE;
            }

            UnoMatchMove move = new UnoMatchMove
            {
                DeckCard = GetCurrentCard(),
                PlayerId = CurrentPlayer,
                Move = cards,
                SelecteColor = color
            };

            Backlog.Add(move);
            dbMatch.Backlog.Add(move);

            DbHand currentPlayerHand = hands.Where(x => x.User.ExternalId == CurrentPlayer).FirstOrDefault();

            if (currentPlayerHand == null)
            {
                return MoveStatus.WRONG_MOVE;
            }

            cards.ForEach(card => {
                Hands[CurrentPlayer].Remove(card);
                var dbCard = currentPlayerHand
                    .Hand
                    .Where(x => x.Color == card.Color && x.NumberValue == card.NumberValue && x.Type == card.Type)
                    .FirstOrDefault();
                currentPlayerHand.Hand.Remove(dbCard);
            });

            if (Hands[CurrentPlayer].Count == 0)
            {
                return MoveStatus.ENDGAME;
            }

            cards.ForEach(card => {
                Discharge.Add(card);
                dbMatch.Discharge.Add(card);
            });

            switch (GetCurrentCard().Type)
            {
                case UnoCardType.Numeric:
                    CurrentPlayer = NextPlayer();
                    break;
                case UnoCardType.Reverse:
                case UnoCardType.Skip:
                    break;
                case UnoCardType.TakeTwo:
                    TakeCard(NextPlayer(), dbMatch, hands);
                    TakeCard(NextPlayer(), dbMatch, hands);
                    break;
                case UnoCardType.TakeFourChooseColor:
                    for (var i = 0; i < 4; i++)
                    {
                        TakeCard(NextPlayer(), dbMatch, hands);
                    }
                    CurrentColor = color;
                    break;
                case UnoCardType.ChooseColor:
                    CurrentColor = color;
                    CurrentPlayer = NextPlayer();
                    break;
            }


            player = context.Users.Where(x => x.ExternalId == CurrentPlayer).FirstOrDefault();
            dbMatch.CurrentPlayer = player;
            dbMatch.CurrentColor = color;

            return MoveStatus.SUCCESS;
        }

        private Guid NextPlayer()
        {
            var index = Players.FindIndex(player => player == CurrentPlayer);
            index = index == Players.Count - 1 ? 0 : index + 1;
            return Players[index];
        }

        private void TakeCard(Guid user, DbMatch dbMatch, List<DbHand> hands)
        {
            if (Deck.Count == 0)
            {
                Deck = Discharge.Shuffle().ToList();
                dbMatch.Deck = Deck;
                Discharge.Clear();
                dbMatch.Discharge.Clear();
            }

            DbHand userHand = hands.Where(x => x.User.ExternalId == user).FirstOrDefault();

            userHand.Hand.Add(dbMatch.Deck.First());
            Hands[user].Add(dbMatch.Deck.First());
            dbMatch.Deck.RemoveAt(0);
            Deck.RemoveAt(0);
        }

        private List<UnoCard> GenerateDeck()
        {
            List<UnoCard> result = new List<UnoCard>();
            
            foreach (UnoCardColor color in Enum.GetValues(typeof(UnoCardColor)))
            {
                for (int i = 0; i < 10; i++)
                {
                    var card = new UnoCard
                    {
                        Type = UnoCardType.Numeric,
                        Color = color,
                        NumberValue = i
                    };
                    result.Add(card);
                    if (i != 0)
                    {
                        result.Add(card);
                    }
                }

                var reverse = new UnoCard
                {
                    Type = UnoCardType.Reverse,
                    Color = color
                };

                result.Add(reverse);
                result.Add(reverse);

                var skip = new UnoCard
                {
                    Type = UnoCardType.Skip,
                    Color = color
                };

                result.Add(skip);
                result.Add(skip);

                var takeTwo = new UnoCard
                {
                    Type = UnoCardType.TakeTwo,
                    Color = color
                };

                result.Add(takeTwo);
                result.Add(takeTwo);
            }

            var chooseColor = new UnoCard
            {
                Type = UnoCardType.ChooseColor
            };

            var takeFour = new UnoCard
            {
                Type = UnoCardType.TakeFourChooseColor
            };

            for (int i = 0; i < 4; i++)
            {
                result.Add(chooseColor);
                result.Add(takeFour);
            }

            return result.Shuffle().ToList();
        }
    }

    public enum MoveStatus
    {
        SUCCESS,
        WRONG_MOVE,
        FAKE_EMPTY_MOVE,
        ENDGAME
    }
}
