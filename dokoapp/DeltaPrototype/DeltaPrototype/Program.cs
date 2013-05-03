using Delta.Engine;

namespace DeltaPrototype
{
    /// <summary>
    /// Program class, just provides a Main entry point.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The entry point for the application, will just start the Game class!
        /// </summary>
        static void Main()
        {
            // Starting a game is as easy as this :)
            Application.Start(new Game());
        }
    }
}
