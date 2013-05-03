using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.Rendering.Basics.Materials;
using Delta.Utilities.Datatypes;
using DokoApp.Logic;
using DokoApp.Logic.Game;

namespace DeltaPrototype.UI
{
    class UICard : UIElement
    {
        private Card card;

        public Card Card 
        {
            get
            {
                return this.card;
            }

            set
            {
                if (value == this.card)
                {
                    return;
                }

                this.card = value;

                this.UpdateUI();
            }
        }

        private Point position;

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

        private Material2DColored Material { get; set; }

        public void UpdateUI()
        {
            this.DrawArea = new Rectangle(this.Position, new Size(UISettings.CardWidth, UISettings.CardHeight));
            this.Material = MaterialManager.GetCardMaterial(this.Card);
        }

        public override void Draw()
        {
            this.Material.Draw(this.DrawArea);
        }
    }
}
