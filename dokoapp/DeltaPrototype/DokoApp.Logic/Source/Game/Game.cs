using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolBag.Debug;

namespace DokoApp.Logic.Game
{
    public class TableDeck : Deck
    {
        public CardColor Suit
        {
            get
            {
                return this.Cards.Count > 0 ? this.Cards[0].Color : CardColor.Invalid;
            }
        }
    }

    public class Game
    {
        /// <summary>
        ///   Players.
        /// </summary>
        public List<Player> Players = new List<Player>();

        /// <summary>
        ///   Current round.
        /// </summary>
        public Round CurrentRound { get; set; }

        private Deck gameCards;

        public Game()
        {
            this.gameCards = new Deck(
                new List<Card> 
                {
                    new Card { Color = CardColor.Heart, Value = 10 },
                    new Card { Color = CardColor.Heart, Value = 10 },
                    
                    new Card { Color = CardColor.Club, Value = Card.Queen },
                    new Card { Color = CardColor.Club, Value = Card.Queen },
                    new Card { Color = CardColor.Spade, Value = Card.Queen },
                    new Card { Color = CardColor.Spade, Value = Card.Queen },
                    new Card { Color = CardColor.Heart, Value = Card.Queen },
                    new Card { Color = CardColor.Heart, Value = Card.Queen },
                    new Card { Color = CardColor.Diamond, Value = Card.Queen },
                    new Card { Color = CardColor.Diamond, Value = Card.Queen },
                    
                    new Card { Color = CardColor.Club, Value = Card.Jack },
                    new Card { Color = CardColor.Club, Value = Card.Jack },
                    new Card { Color = CardColor.Spade, Value = Card.Jack },
                    new Card { Color = CardColor.Spade, Value = Card.Jack },
                    new Card { Color = CardColor.Heart, Value = Card.Jack },
                    new Card { Color = CardColor.Heart, Value = Card.Jack },
                    new Card { Color = CardColor.Diamond, Value = Card.Jack },
                    new Card { Color = CardColor.Diamond, Value = Card.Jack },
                    
                    new Card { Color = CardColor.Diamond, Value = Card.Ace },
                    new Card { Color = CardColor.Diamond, Value = Card.Ace },
                    new Card { Color = CardColor.Diamond, Value = 10 },
                    new Card { Color = CardColor.Diamond, Value = 10 },
                    new Card { Color = CardColor.Diamond, Value = Card.King },
                    new Card { Color = CardColor.Diamond, Value = Card.King },
                    new Card { Color = CardColor.Diamond, Value = 9 },
                    new Card { Color = CardColor.Diamond, Value = 9 },
            
                    new Card { Color = CardColor.Heart, Value = Card.Ace },
                    new Card { Color = CardColor.Heart, Value = Card.Ace },
                    new Card { Color = CardColor.Heart, Value = 10 },
                    new Card { Color = CardColor.Heart, Value = 10 },
                    new Card { Color = CardColor.Heart, Value = Card.King },
                    new Card { Color = CardColor.Heart, Value = Card.King },
                    new Card { Color = CardColor.Heart, Value = 9 },
                    new Card { Color = CardColor.Heart, Value = 9 },
                    
                    new Card { Color = CardColor.Spade, Value = Card.Ace },
                    new Card { Color = CardColor.Spade, Value = Card.Ace },
                    new Card { Color = CardColor.Spade, Value = 10 },
                    new Card { Color = CardColor.Spade, Value = 10 },
                    new Card { Color = CardColor.Spade, Value = Card.King },
                    new Card { Color = CardColor.Spade, Value = Card.King },
                    new Card { Color = CardColor.Spade, Value = 9 },
                    new Card { Color = CardColor.Spade, Value = 9 },
                    
                    new Card { Color = CardColor.Club, Value = Card.Ace },
                    new Card { Color = CardColor.Club, Value = Card.Ace },
                    new Card { Color = CardColor.Club, Value = 10 },
                    new Card { Color = CardColor.Club, Value = 10 },
                    new Card { Color = CardColor.Club, Value = Card.King },
                    new Card { Color = CardColor.Club, Value = Card.King },
                    new Card { Color = CardColor.Club, Value = 9 },
                    new Card { Color = CardColor.Club, Value = 9 },
                }
            );
        }

        public void Start()
        {
            // Create new round.
            this.CurrentRound = new Round();
            this.CurrentRound.Game = this;

            // Deal out first round.
            this.CurrentRound.Deal();
        }

        public List<Card> CardValues = new List<Card> 
        { 
            new Card { Color = CardColor.Heart, Value = 10 },

            new Card { Color = CardColor.Club, Value = Card.Queen },
            new Card { Color = CardColor.Spade, Value = Card.Queen },
            new Card { Color = CardColor.Heart, Value = Card.Queen },
            new Card { Color = CardColor.Diamond, Value = Card.Queen },

            new Card { Color = CardColor.Club, Value = Card.Jack },
            new Card { Color = CardColor.Spade, Value = Card.Jack },
            new Card { Color = CardColor.Heart, Value = Card.Jack },
            new Card { Color = CardColor.Diamond, Value = Card.Jack },

            new Card { Color = CardColor.Diamond, Value = Card.Ace },
            new Card { Color = CardColor.Diamond, Value = 10 },
            new Card { Color = CardColor.Diamond, Value = Card.King },
            new Card { Color = CardColor.Diamond, Value = 9 },
            
            new Card { Color = CardColor.Heart, Value = Card.Ace },
            new Card { Color = CardColor.Heart, Value = 10 },
            new Card { Color = CardColor.Heart, Value = Card.King },
            new Card { Color = CardColor.Heart, Value = 9 },

            new Card { Color = CardColor.Spade, Value = Card.Ace },
            new Card { Color = CardColor.Spade, Value = 10 },
            new Card { Color = CardColor.Spade, Value = Card.King },
            new Card { Color = CardColor.Spade, Value = 9 },

            new Card { Color = CardColor.Club, Value = Card.Ace },
            new Card { Color = CardColor.Club, Value = 10 },
            new Card { Color = CardColor.Club, Value = Card.King },
            new Card { Color = CardColor.Club, Value = 9 },
        };

        private int LastTrumpIndex = 12;

        /// <summary>
        ///   Returns the value for the passed card in the passed game. The lower the value, the better the card.
        /// </summary>
        /// <param name="card">Card to evaluate.</param>
        /// <returns>Card value.</returns>
        public int GetCardValue(Card card)
        {
            if (card == null)
            {
                return this.CardValues.Count;
            }

            return this.CardValues.FindIndex(c => c.Equals(card));
        }

        public bool IsTrump(Card card)
        {
            if (card == null)
            {
                return false;
            }

            return this.CardValues.FindIndex(c => c.Equals(card)) <= this.LastTrumpIndex;
        }

        /// <summary>
        ///   Returns a deck of cards which contains all available cards.
        /// </summary>
        /// <returns></returns>
        public Deck GetCards()
        {
            return this.gameCards;
        }

        public int CompareCardsByValue(Card lhs, Card rhs)
        {
            return this.GetCardValue(lhs) < this.GetCardValue(rhs) ? -1 : 1;
        }
    }
}
