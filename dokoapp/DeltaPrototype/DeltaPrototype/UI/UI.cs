using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.Utilities.Datatypes;
using Delta.Rendering.Basics.Materials;

namespace DeltaPrototype.UI
{
    class UI
    {
        private List<UIElement> uiElements = new List<UIElement>();

        public List<UIElement> UIElements
        {
            get
            {
                return this.uiElements;
            }
        }

        public void Draw()
        {
            foreach (UIElement uiElement in this.uiElements)
            {
                uiElement.Draw();
            }
        }
    }
}
