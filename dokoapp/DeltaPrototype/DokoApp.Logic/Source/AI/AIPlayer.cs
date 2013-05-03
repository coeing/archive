using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DokoApp.Logic.Game;
using ToolBag.Debug;

namespace DokoApp.Logic.AI
{
    public class AIPlayer : Player
    {
        /// <summary>
        ///   Called when it's the player's turn.
        /// </summary>
        public override void OnTurn(Game.Game game, Round round)
        {
            Assertion.Exception(this.Deck.Cards.Count > 0, "No cards left.");

            // Choose random possible card.
            Card playCard = null;
            if (round.TableDeck.Cards.Count == 0)
            {
                playCard = this.Deck.Cards[new Random().Next() % this.Deck.Cards.Count];
            }
            else
            {
                Card firstCard = round.TableDeck.Cards[0];

                if (game.IsTrump(firstCard))
                {
                    // Play random trump.
                    foreach (Card card in this.Deck.Cards)
                    {
                        if (game.IsTrump(card))
                        {
                            playCard = card;
                            break;
                        }
                    }
                }
                else
                {
                    // Check if has color.
                    foreach (Card card in this.Deck.Cards)
                    {
                        if (!game.IsTrump(card) &&
                            card.Color == firstCard.Color)
                        {
                            playCard = card;
                            break;
                        }
                    }       
             
                    // Play random card.
                    playCard = this.Deck.Cards[new Random().Next() % this.Deck.Cards.Count];
                }
            }

            round.DoTurn(this, playCard);
        }
    }
}
