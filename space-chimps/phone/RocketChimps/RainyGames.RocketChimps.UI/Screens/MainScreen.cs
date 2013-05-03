using System;
using System.Net;

using Delta.Rendering;
using Delta.Rendering.Basics.Materials;
using Delta.Utilities.Datatypes;

namespace RainyGames.RocketChimps.UI.Screens
{
    public class MainScreen : Screen
    {
        private Material2D backgroundMaterial = new Material2D("title_screen");

        public override void Draw()
        {
            this.backgroundMaterial.Draw(Rectangle.FromCorners(Point.Zero, Point.One));
        }
    }
}
