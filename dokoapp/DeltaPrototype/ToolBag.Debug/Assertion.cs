using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ToolBag.Debug
{
    /// <summary>
    ///   Class with helper functions to do assertions.
    /// </summary>
    public static class Assertion
    {
        /// <summary>
        ///   Checks a condition and throws an exception if the condition is NOT met.
        /// </summary>
        /// <param name="condition">Condition to check.</param>
        /// <param name="exception">Exception to throw.</param>
        public static void Exception(bool condition, Exception exception)
        {
            if (condition)
            {
                return;
            }

            throw exception;
        }

        /// <summary>
        ///   Checks a condition and throws an exception if the condition is NOT met.
        /// </summary>
        /// <param name="condition">Condition to check.</param>
        /// <param name="message">Exception message.</param>
        public static void Exception(bool condition, string message)
        {
            Exception(condition, new Exception(message));
        }

        /// <summary>
        ///   Checks a condition and throws an exception if the condition is NOT met.
        /// </summary>
        /// <param name="condition">Condition to check.</param>
        /// <param name="format">Exception message format.</param>
        /// <param name="args">Message arguments.</param>
        public static void Exception(bool condition, string format, params object[] args)
        {
            Exception(condition, string.Format(format, args));
        }
    }
}
