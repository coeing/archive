using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DokoApp.Logic.Game
{
    public class Card : IEquatable<Card>
    {
        /// <summary>
        ///   Value of Jack.
        /// </summary>
        public static int Jack = 11;

        /// <summary>
        ///   Value of Queen.
        /// </summary>
        public static int Queen = 12;

        /// <summary>
        ///   Value of King.
        /// </summary>
        public static int King = 12;

        /// <summary>
        ///   Value of Ace.
        /// </summary>
        public static int Ace = 13;

        public CardColor Color { get; set; }

        public int Value { get; set; }


        public bool Equals(Card other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Color == other.Color && this.Value == other.Value;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Color, this.Value);
        }
    }
}
