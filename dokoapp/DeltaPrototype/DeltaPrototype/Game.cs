using System.Collections.Generic;
using Delta.Engine;
using Delta.Engine.Dynamic;
using Delta.Rendering.Basics.Materials;
using Delta.Rendering.Basics.Fonts;
using Delta.Utilities.Datatypes;
using Delta.InputSystem;
using Delta.Utilities.Helpers;
using DeltaPrototype.UI;
using DokoApp.Logic;
using DokoApp.Logic.Game;
using DokoApp.Logic.AI;

namespace DeltaPrototype
{
    /// <summary>
    /// Game class, which is the entry point for this game. Manages all the game
    /// logic and displays everything. More complex games do this differently.
    /// </summary>
    public class Game : DynamicModule
    {
        /// <summary>
        ///   Game logic.
        /// </summary>
        private DokoApp.Logic.Game.Game game = new DokoApp.Logic.Game.Game();

        #region Constants
        /// <summary>
        /// Update the trail 60 times per second.
        /// </summary>
        private const int NumberOfUpdatesPerSecond = 60;
        /// <summary>
        /// Keep the trail for 2 seconds.
        /// </summary>
        private const int TrailLengthInSeconds = 2;
        /// <summary>
        /// Interpolate the trail to make it look more smooth than the input
        /// allowes (which might jump great distances).
        /// </summary>
        private const int NumberOfInterpolations = 3;
        #endregion

        #region Variables
        /// <summary>
        /// Just load the DeltaEngineLogo image from the SimpleGameExample sample
        /// content project. Or if that is not available the DeltaEngineLogo image
        /// fallback content project, which is always available.
        /// </summary>
        private readonly Material2DColored gameObject =
            new Material2DColored("card_heart_02");
        /// <summary>
        /// Welcome text, which can be changed once a UIClick happened.
        /// </summary>
        private string welcomeText =
            "Press Space, GamePad, Click or Touch to see the hidden message";
        /// <summary>
        /// Start drawing in the middle, but allow moving around with cursors or
        /// any input device returning position change (mouse, touch, gamepad).
        /// </summary>
        private Point drawLocation = Point.Half;

        /// <summary>
        /// Last positions for the trail that is slowly fading out.
        /// </summary>
        readonly List<Point> lastPositions = new List<Point>();
        /// <summary>
        /// Last times (in milliseconds) for the trail to reconstruct the size
        /// and rotation values.
        /// </summary>
        readonly List<long> lastTimeMs = new List<long>();
        /// <summary>
        /// Last colors for the trail to make everything a bit more colorful.
        /// </summary>
        readonly List<Color> lastColors = new List<Color>();
        /// <summary>
        /// Current color used for drawing the trail.
        /// </summary>
        private Color currentColor = Color.Random;
        /// <summary>
        /// Next color we are fading to for the trail.
        /// </summary>
        private Color nextColor = Color.Random;
        #endregion

        UI.UI ui;
        Deck testDeck;

        #region Constructor
        /// <summary>
        /// Constructor, do additional initialization code here if needed.
        /// </summary>
        public Game()
            : base("Simple Game", typeof(Application))
        {
            // Note: Normally in a game you would define those commands via the
            // InputSettings.xml in the ContentManager, don't use the commands here!
            Input.Commands[Command.UIClick].Add(command => this.OnUIClick(command));
            CommandDelegate moveDrawLocation = delegate(CommandTrigger command)
            {
                //Log.Info("Position=" + command.Position + ", from " + command.Button);
                drawLocation = command.Position;//.Movement / 10.0f;
            };
            Input.Commands[Command.CameraRotateLeft].Add(moveDrawLocation);
            Input.Commands[Command.CameraRotateRight].Add(moveDrawLocation);
            Input.Commands[Command.CameraRotateUp].Add(moveDrawLocation);
            Input.Commands[Command.CameraRotateDown].Add(moveDrawLocation);
            Input.Commands[Command.QuitTest].Add(delegate
            {
                Application.Quit();
            });

            this.ui = new UI.UI();

            // Create test deck.
            this.testDeck = new Deck();
            this.testDeck.Cards.Add(new Card { Color = CardColor.Heart, Value = 2 });
            this.testDeck.Cards.Add(new Card { Color = CardColor.Spade, Value = 6 });
            this.testDeck.Cards.Add(new Card { Color = CardColor.Diamond, Value = 8 });
            this.testDeck.Cards.Add(new Card { Color = CardColor.Club, Value = 11 });

            this.gameObject = new Material2DColored("card_heart_02"); //materialManager.GetCardMaterial(new Card { Color = CardColor.Heart, Value = 2 });

            // Add players.
            this.game.Players.Add(new Player { Name = "Christian" });
            this.game.Players.Add(new AIPlayer { Name = "Tobi" });
            this.game.Players.Add(new AIPlayer { Name = "Ali" });
            this.game.Players.Add(new AIPlayer { Name = "Frank" });

            // Start game.
            this.game.Start();

            // Add deck of first player.
            this.playerDeck = new UIDeck()
            {
                Deck = this.game.Players[0].Deck,
                Position = new Point(ScreenSpace.DrawArea.Center.X, ScreenSpace.DrawArea.Bottom - UISettings.CardHeight * 0.5f),
                Orientation = Orientation.Horizontal
            };
            this.ui.UIElements.Add(this.playerDeck);
        }

        private UIDeck playerDeck;

        private void OnUIClick(CommandTrigger command)
        {
            Application.BackgroundColor = Color.DarkBlue;
            welcomeText = "You made it! Yay. Now write some game code :)";

            // Get clicked card.
            Card card = this.playerDeck.GetCard(command.Position);
            if (card == null)
            {
                welcomeText = "No card selected";
            }
            else
            {
                welcomeText = string.Format("Card {0} selected", card);
            }
        }

        #endregion

        #region Run
        /// <summary>
        /// Run game loop, called every frame to do all game logic updating.
        /// Note: put your game logic here, it will be executed each frame.
        /// There is no difference between update code and render code as all
        /// rendering will happen optimized at the end of the frame anyway!
        /// </summary>
        public override void Run()
        {
            // Show FPS
            Font.DrawTopLeftInformation("Fps: " + Time.Fps + " Pos: " + drawLocation);

            // Draw UI.
            this.ui.Draw();

            // Draw the trail and then the current object
            for (int num = 0; num < lastPositions.Count; num++)
            {
                //DrawGameObject(lastPositions[num], lastTimeMs[num], lastColors[num],
                //    num / (float)lastPositions.Count);
            }
            //DrawGameObject(drawLocation, Time.Milliseconds, Color.White, 1.0f);

            // Change color every second
            if (Time.EverySecond)
            {
                currentColor = nextColor;
                nextColor = Color.Random;
            }

            // Remember position for the trail 30 times per second.
            if (Time.CheckEvery(1.0f / NumberOfUpdatesPerSecond))
            {
                while (lastPositions.Count >
                    NumberOfUpdatesPerSecond * TrailLengthInSeconds)
                {
                    lastPositions.RemoveAt(0);
                    lastTimeMs.RemoveAt(0);
                    lastColors.RemoveAt(0);
                }
                float colorPerecentage = (Time.Milliseconds % 1000) / 1000.0f;
                Color newColor = Color.Lerp(currentColor, nextColor,
                    colorPerecentage);
                // Add few interpolated entries between this one and the last, this
                // will make the output look much more smooth :)
                if (lastPositions.Count >= NumberOfInterpolations)
                {
                    // This way the output looks more smooth :)
                    Point p1 = drawLocation;
                    Point p2 = lastPositions[
                        lastPositions.Count - NumberOfInterpolations];
                    long t1 = Time.Milliseconds;
                    long t2 = lastTimeMs[
                        lastTimeMs.Count - NumberOfInterpolations];
                    for (int num = 1; num < NumberOfInterpolations; num++)
                    {
                        float interpolation = num / (float)NumberOfInterpolations;
                        lastPositions.Add(Point.Lerp(p1, p2, interpolation));
                        lastTimeMs.Add(MathHelper.Lerp(t1, t2, interpolation));
                        lastColors.Add(newColor);
                    }
                }
                lastPositions.Add(drawLocation);
                lastTimeMs.Add(Time.Milliseconds);
                lastColors.Add(newColor);
            }

            // And a simple text message
            Font.Default.Draw(welcomeText,
                new Rectangle(0.0f,
                    ScreenSpace.DrawArea.Bottom - Font.Default.LineHeight, 1.0f,
                    Font.Default.LineHeight));
        }
        #endregion

        #region DrawGameObject
        /// <summary>
        /// Draw game object, also used for the trail.
        /// </summary>
        /// <param name="position">Current position</param>
        /// <param name="milliseconds">Time for this object or trail</param>
        /// <param name="color">Color for the trail</param>
        /// <param name="alpha">Alpha for drawing</param>
        private void DrawGameObject(Point position, long milliseconds, Color color,
            float alpha)
        {
            //gameObject.BlendColor = new Color(color, alpha);
            Rectangle drawArea = Rectangle.FromCenter(position,
                gameObject.Size);
            gameObject.Draw(drawArea);
        }
        #endregion
    }
}
