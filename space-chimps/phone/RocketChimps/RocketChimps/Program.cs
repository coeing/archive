using Delta.Engine;

namespace RocketChimps
{
    /// <summary>
    /// Program class, just provides a Main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point for the application, will just start the Game class!
        /// </summary>
        public static void Main()
        {
            // Starting a game is as easy as this :)
            Application.Start(new Game());
        }
    }
}
