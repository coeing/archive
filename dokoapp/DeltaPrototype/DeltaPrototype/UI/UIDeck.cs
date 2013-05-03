using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.Utilities.Datatypes;
using DokoApp.Logic;
using DokoApp.Logic.Game;

namespace DeltaPrototype.UI
{
    class UIDeck : UIElement
    {
        public Orientation Orientation { get; set; }

        private Deck deck;

        public Deck Deck
        {
            get
            {
                return this.deck;
            }
            set
            {
                if (value == this.deck)
                {
                    return;
                }

                this.deck = value;

                this.UpdateUI();
            }
        }

        private Point position;

        /// <summary>
        ///   Center position of deck.
        /// </summary>
        public Point Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (value == this.position)
                {
                    return;
                }

                this.position = value;

                this.UpdateUI();
            }
        }

        public void UpdateUI()
        {
            this.Children.Clear();

            // Compute start position.
            Point cardPosition = Position - new Point(this.Deck.Cards.Count * 0.5f * UISettings.CardOffset, UISettings.CardHeight * 0.5f);
            foreach (Card card in this.Deck.Cards)
            {
                UICard uiCard = new UICard() { Card = card, Position = cardPosition };
                this.Children.Add(uiCard);

                cardPosition.X += UISettings.CardOffset;
            }

        }

        /// <summary>
        ///   Returns the card at the passed point.
        /// </summary>
        /// <param name="point">Position to check.</param>
        public Card GetCard(Point point)
        {
            // Get relative point.
            Point relativePoint = point - this.position;
            if (relativePoint.Y < -UISettings.CardHeight * 0.5f || relativePoint.Y > UISettings.CardHeight * 0.5f)
            {
                return null;
            }

            // Check x position.
            float relativeX = relativePoint.X;
            float leftBorder = -this.Deck.Cards.Count * 0.5f * UISettings.CardOffset;
            float rightBorder = this.Deck.Cards.Count * 0.5f * UISettings.CardOffset;
            if (relativeX < leftBorder || relativeX > rightBorder)
            {
                return null;
            }

            int cardIdx = (int)Math.Floor((relativeX - leftBorder) / UISettings.CardOffset);
            return this.Deck.Cards[cardIdx];
        }
    }
}
