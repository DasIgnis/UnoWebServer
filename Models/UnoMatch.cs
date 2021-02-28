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

        public UnoMatch()
        {
            Players = new List<Guid>();
            Deck = GenerateDeck();
            Discharge = new List<UnoCard>();
            Hands = new Dictionary<Guid, List<UnoCard>>();
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
        }

        public List<UnoCard> GetHand(Guid player)
        {
            return Hands[player];
        }

        public UnoCard GetCurrentCard()
        {
            return Discharge.Last();
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
}
