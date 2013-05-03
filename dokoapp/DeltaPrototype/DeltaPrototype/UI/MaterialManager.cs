using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.Rendering.Basics.Materials;
using DokoApp.Logic;
using DokoApp.Logic.Game;

namespace DeltaPrototype.UI
{
    static class MaterialManager
    {
        public static Dictionary<Card, Material2DColored> cardMaterials = new Dictionary<Card, Material2DColored>();

        public static Material2DColored GetCardMaterial(Card card)
        {
            // Check if material already loaded.
            Material2DColored material = null;
            if (!cardMaterials.TryGetValue(card, out material))
            {
                // Compose material name.
                string materialName = string.Format("card_{0}_{1:d2}", card.Color.ToString().ToLower(), card.Value);

                // Create material.
                material = new Material2DColored(materialName);

                // Store material.
                cardMaterials[card] = material;
            }

            return material;
        }
    }
}
