using System.Collections.Generic;
using Delta.Engine;
using Delta.Engine.Dynamic;
using Delta.Rendering.Basics.Materials;
using Delta.Rendering.Basics.Fonts;
using Delta.Utilities.Datatypes;
using Delta.InputSystem;
using Delta.Utilities.Helpers;

using RainyGames.RocketChimps.UI.Screens;

namespace RocketChimps
{
    /// <summary>
    /// Game class, which is the entry point for this game. Manages all the game
    /// logic and displays everything. More complex games do this differently.
    /// </summary>
    class Game : DynamicModule
    {
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
        ///   Current screen.
        /// </summary>
        private Screen screen;

        /// <summary>
        /// Just load the DeltaEngineLogo image from the SimpleGameExample sample
        /// content project. Or if that is not available the DeltaEngineLogo image
        /// fallback content project, which is always available.
        /// </summary>
        private readonly Material2DColored gameObject =
            new Material2DColored("DeltaEngineLogo");
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

        #region Constructor
        /// <summary>
        /// Constructor, do additional initialization code here if needed.
        /// </summary>
        public Game()
            : base("Simple Game", typeof(Application))
        {
            // Note: Normally in a game you would define those commands via the
            // InputSettings.xml in the ContentManager, don't use the commands here!
            Input.Commands[Command.UIClick].Add(delegate
            {
                Application.BackgroundColor = Color.DarkBlue;
                welcomeText = "You made it! Yay. Now write some game code :)";
            });
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

            // Start with main screen.
            this.screen = new MainScreen();
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
            Font.Default.Draw("FPS: " + Time.Fps, Rectangle.FromCorners(Point.Zero, Point.One));

            // Draw the trail and then the current object
            for (int num = 0; num < lastPositions.Count; num++)
            {
                DrawGameObject(lastPositions[num], lastTimeMs[num], lastColors[num],
                    num / (float)lastPositions.Count);
            }
            DrawGameObject(drawLocation, Time.Milliseconds, Color.White, 1.0f);

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
            Font.Default.Draw(welcomeText, Rectangle.FromCenter(new Point(0.5f,
                ScreenSpace.DrawArea.Bottom - Font.Default.LineHeight / 2), 1.0f));

            // Draw screen.
            this.screen.Draw();
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
            gameObject.BlendColor = new Color(color, alpha);
            gameObject.Draw(Rectangle.FromCenter(position, gameObject.Size + 0.5f *
                gameObject.Size * MathHelper.Sin(milliseconds / 20.0f)),
                milliseconds / 10.0f);
        }
        #endregion
    }
}
