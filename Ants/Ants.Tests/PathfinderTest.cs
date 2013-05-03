using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants.Source.Pathfinding;
using Ants.Source;

namespace Ants.Tests
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für PathfinderTest
    /// </summary>
    [TestClass]
    public class PathfinderTest
    {
        private GameContext testContext;

        private Pathfinder testPathfinder;

        public PathfinderTest()
        {
            GameState gameState = new GameState(10, 10, 10, 10, 100, 100, 100, 1);
            this.testContext = new GameContext { GameState = gameState, TurnState = new TurnState(gameState) };
            this.testPathfinder = new Pathfinder(this.testContext);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ruft den Textkontext mit Informationen über
        ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Zusätzliche Testattribute
        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestPathfinderSimple()
        {
            Location start = new Location(0, 0);
            Location target = new Location(1, 0);
            List<Location> path = this.testPathfinder.FindPath(start, target);
            Assert.AreEqual(1, path.Count);
            Assert.AreEqual(target, path[0]);
        }
    }
}
