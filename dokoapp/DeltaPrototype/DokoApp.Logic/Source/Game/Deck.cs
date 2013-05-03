using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ToolBag.Collections.List;

namespace DokoApp.Logic.Game
{
    public class Deck
    {
        /// <summary>
        ///   Cards.
        /// </summary>
        private List<Card> cards;

        public Deck(List<Card> cards)
        {
            this.cards = cards;
        }

        public Deck()
        {
            this.cards = new List<Card>();
        }

        /// <summary>
        ///   Cards.
        /// </summary>
        public List<Card> Cards { get { return this.cards; } }

        /// <summary>
        ///   Sorts the deck with the passed comparison method.
        /// </summary>
        public void Sort(Comparison<Card> comparer)
        {
            this.cards.Sort(comparer);
        }

        /// <summary>
        ///   Returns the index of the highest card in the deck.
        /// </summary>
        /// <returns>Index of highest card. -1 if there is no card in the deck.</returns>
        public int GetHighestCardIdx(Comparison<Card> comparer)
        {
            int highestCardIdx = -1;
            Card highestCard = null;
            for (int index = 0; index < this.cards.Count; ++index)
            {
                Card card = this.cards[index];
                if (highestCard == null ||
                    comparer(highestCard, card) > 0)
                {
                    highestCard = card;
                    highestCardIdx = index;
                }
            }
            return highestCardIdx;
        }

        /// <summary>
        ///   Clears the deck.
        /// </summary>
        public void Clear()
        {
            this.cards.Clear();
        }

        /// <summary>
        ///   Shuffles the deck.
        /// </summary>
        public void Shuffle()
        {
            this.cards.Shuffle();
        }
    }
}
