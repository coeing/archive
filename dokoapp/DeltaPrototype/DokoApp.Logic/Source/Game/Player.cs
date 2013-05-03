using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DokoApp.Logic.Game
{
    public class Player
    {
        /// <summary>
        ///   Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   Deck.
        /// </summary>
        public Deck Deck { get; set; }

        /// <summary>
        ///   Constructor.
        /// </summary>
        public Player()
        {
            this.Deck = new Deck();
        }

        /// <summary>
        ///   Called when it's the player's turn.
        /// </summary>
        public virtual void OnTurn(Game game, Round round)
        {
        }
    }
}
