using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.Utilities.Datatypes;

namespace DeltaPrototype.UI
{
    class UIElement
    {
        public List<UIElement> Children = new List<UIElement>();

        protected Rectangle DrawArea { get; set; }

        public virtual void Draw()
        {
            // Draw children.
            foreach (UIElement child in this.Children)
            {
                child.Draw();
            }
        }
    }
}
