using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolBag.Debug;

namespace DokoApp.Logic.Game
{
    public class Round
    {
        /// <summary>
        ///   Game this round belongs to.
        /// </summary>
        public Game Game { get; set; }

        public TableDeck TableDeck { get; set; }

        /// <summary>
        ///   Index of current player.
        /// </summary>
        private int currentPlayerIdx;

        public Player CurrentPlayer
        {
            get
            {
                return this.Game.Players[this.currentPlayerIdx];
            }
        }

        /// <summary>
        ///   Index of player who started the turn.
        /// </summary>
        private int startPlayerIdx = 0;

        /// <summary>
        ///   Deals the cards to the players.
        /// </summary>
        public void Deal()
        {
            // Clear player decks.
            foreach (Player player in this.Game.Players)
            {
                player.Deck.Clear();
            }

            // Get cards.
            Deck gameDeck = this.Game.GetCards();

            // Shuffle cards.
            gameDeck.Shuffle();

            // Deal cards.
            int playerIdx = this.startPlayerIdx;
            foreach (Card card in gameDeck.Cards)
            {
                this.Game.Players[playerIdx].Deck.Cards.Add(card);
                playerIdx = (playerIdx + 1) % this.Game.Players.Count;
            }

            // Sort player decks.
            foreach (Player player in this.Game.Players)
            {
                player.Deck.Sort(this.Game.CompareCardsByValue);
            }

            // Set first player's turn.
            this.currentPlayerIdx = this.startPlayerIdx;
            this.CurrentPlayer.OnTurn(this.Game, this);
        }

        public void DoTurn(Player player, Card card)
        {
            // Check if player is current player.
            Assertion.Exception(this.Game.Players.FindIndex(p => p == player) == this.currentPlayerIdx, "It's not the turn of player {0}", player.Name);

            // Check if player has card.
            Assertion.Exception(player.Deck.Cards.Contains(card), "Player {0} doesn't have card {1}", player.Name, card);

            // Take card from player.
            player.Deck.Cards.Remove(card);

            // Put card on table deck.
            this.TableDeck.Cards.Add(card);

            // Set next player.
            this.currentPlayerIdx = (this.currentPlayerIdx + 1) % this.Game.Players.Count;

            // Check if every player did his turn.
            if (this.currentPlayerIdx == this.startPlayerIdx)
            {
                this.OnTurnEnd();
            }
            else
            {
                // Inform next player about turn.
                this.CurrentPlayer.OnTurn(this.Game, this);
            }
        }

        private void OnTurnEnd()
        {
            // Evaluate table deck.
            int highestCardIdx = this.TableDeck.GetHighestCardIdx(this.CompareTableDeckCards);
        }

        /// <summary>
        ///   Compares two cards of the table deck. 
        ///   NOTE(co): Assumes that lhs card was played before rhs card.
        /// </summary>
        /// <param name="lhs">First card to compare.</param>
        /// <param name="rhs">Second card to compare.</param>
        /// <returns>-1 if lhs is worse, 1 if lhs is better than rhs.</returns>
        public int CompareTableDeckCards(Card lhs, Card rhs)
        {
            // Check if same card => First played card wins.
            if (lhs.Equals(rhs))
            {
                return 1;
            }

            // Check if one is trump and one is not.
            bool lhsTrump = this.Game.IsTrump(lhs);
            bool rhsTrump = this.Game.IsTrump(lhs);
            if (lhsTrump && !rhsTrump)
            {
                return 1;
            }
            if (!lhsTrump && rhsTrump)
            {
                return -1;
            }

            // Both trump => Highest wins.
            if (lhsTrump && rhsTrump)
            {
                return this.Game.GetCardValue(lhs) > this.Game.GetCardValue(rhs) ? 1 : -1;
            }

            // Check if one is suit color and other isn't.
            bool lhsSuit = lhs.Color == this.TableDeck.Suit;
            bool rhsSuit = rhs.Color == this.TableDeck.Suit;
            if (lhsSuit && !rhsSuit)
            {
                return 1;
            }
            if (!lhsSuit && rhsSuit)
            {
                return -1;
            }

            // Both are suit or both are non-suit => Highest wins.
            return this.Game.GetCardValue(lhs) > this.Game.GetCardValue(rhs) ? 1 : -1;
        }
    }
}
