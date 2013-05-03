using System;
using System.Collections.Generic;
using Ants.Source.Entities;
using Ants.Source;
using System.Diagnostics;

using System.Linq;

namespace Ants {
	
	public class GameState : IGameState {
		
		public int Width { get; private set; }
		public int Height { get; private set; }
		
		public int LoadTime { get; private set; }
		public int TurnTime { get; private set; }

        /// <summary>
        ///   Stopwatch to measure elapsed time.
        /// </summary>
        private Stopwatch elapsedTurnTime = new Stopwatch();
		
        /// <summary>
        ///   Elapsed time in the current turn.
        /// </summary>
        public int ElapsedTurnTime
        {
            get
            {
                return (int)this.elapsedTurnTime.ElapsedMilliseconds;
            }
        }

		public int TimeRemaining {
			get {
				return TurnTime - (int)this.elapsedTurnTime.ElapsedMilliseconds;
			}
		}

		public int ViewRadius2 { get; private set; }
		public int AttackRadius2 { get; private set; }
		public int SpawnRadius2 { get; private set; }

        public long Seed { get; private set; }

        public List<Ant> MyAnts { get; private set; }
		public List<Ant> MyAntsTurn { get; private set; }
        public List<AntHill> MyHills { get; private set; }
        public List<Ant> EnemyAnts { get; private set; }
        public List<AntHill> VisibleEnemyHills { get; private set; }
        public List<AntHill> KnownEnemyHills { get; private set; }
        public List<Location> DeadTiles { get; private set; }
        public List<Location> AddedMyAnts { get; private set; }
        public List<Location> KilledMyAnts { get; private set; }
        public List<Location> VisibleFoodTiles { get; private set; }
        public List<Location> KnownFood { get; private set; }
        public List<Location> NewFood { get; private set; }
        public List<Location> RemovedFood { get; private set; }

		public Tile this[Location location] {
			get { return this.map[location.Row, location.Col]; }
		}

		public Tile this[int row, int col] {
			get { return this.map[row, col]; }
		}
		
		private Tile[,] map;
		
		public GameState (int width, int height, 
		                  int turntime, int loadtime, 
		                  int viewradius2, int attackradius2, int spawnradius2, long seed) {
			
			Width = width;
			Height = height;
			
			LoadTime = loadtime;
			TurnTime = turntime;
			
			ViewRadius2 = viewradius2;
			AttackRadius2 = attackradius2;
			SpawnRadius2 = spawnradius2;

            Seed = seed;
                 
            this.MyAnts = new List<Ant>();
            this.MyAntsTurn = new List<Ant>();
            this.MyHills = new List<AntHill>();
            this.EnemyAnts = new List<Ant>();
            this.VisibleEnemyHills = new List<AntHill>();
            this.KnownEnemyHills = new List<AntHill>();
            this.DeadTiles = new List<Location>();
            this.AddedMyAnts = new List<Location>();
            this.KilledMyAnts = new List<Location>();
            this.VisibleFoodTiles = new List<Location>();
            this.KnownFood = new List<Location>();
            this.NewFood = new List<Location>();
            this.RemovedFood = new List<Location>();
			
			map = new Tile[height, width];
			for (int row = 0; row < height; row++) {
				for (int col = 0; col < width; col++) {
					map[row, col] = Tile.Unseen;
				}
			}

            mapAccessed = new bool[height, width];
            this.mapAccessible = new bool[height, width];
            this.vision = new bool[height, width];
            
            // Precompute vision offsets of an ant.
            int maxOffset = (int)Math.Sqrt(viewradius2);
            for (int rowOffset = -maxOffset; rowOffset <= maxOffset; ++rowOffset)
            {
                for (int colOffset = -maxOffset; colOffset <= maxOffset; ++colOffset)
                {
                    int tileSquareDistance = rowOffset * rowOffset + colOffset * colOffset;
                    if (tileSquareDistance <= viewradius2)
                    {
                        this.visionOffsets.Add(new Location(rowOffset % height, colOffset % width));
                    }
                }
            }
		}

		#region State mutators
		public void StartNewTurn () 
        {
			// Reset stopwatch.
            this.elapsedTurnTime.Reset();
            this.elapsedTurnTime.Start();
			
			// clear ant data
			foreach (Location loc in MyAnts) map[loc.Row, loc.Col] = Tile.Land;
            foreach (Location loc in MyHills)
            {

                map[loc.Row, loc.Col] = Tile.Land;
            }

			foreach (Location loc in EnemyAnts) map[loc.Row, loc.Col] = Tile.Land;
			foreach (Location loc in VisibleEnemyHills) map[loc.Row, loc.Col] = Tile.Land;
			foreach (Location loc in DeadTiles) map[loc.Row, loc.Col] = Tile.Land;
			
			MyHills.Clear();
            MyAntsTurn.Clear();
			VisibleEnemyHills.Clear();
			EnemyAnts.Clear();
            this.DeadTiles.Clear();
            this.AddedMyAnts.Clear();
            this.KilledMyAnts.Clear();
			
			// set all visible food to unseen
			foreach (Location loc in VisibleFoodTiles) map[loc.Row, loc.Col] = Tile.Land;

            this.VisibleFoodTiles.Clear();
            this.NewFood.Clear();
            this.RemovedFood.Clear();
		}

		public void AddAnt (int row, int col, int team) {
			map[row, col] = Tile.Ant;
			
			Ant ant = new Ant(row, col, team);
			if (team == 0) {
                this.MyAntsTurn.Add(ant);
                if (!this.MyAnts.Contains(ant))
                {
#if DEBUG
                    Log.WriteLine(string.Format("Own Ant {0} was added.", ant));
#endif
                    this.MyAnts.Add(ant);
                    this.AddedMyAnts.Add(ant);
                }
			} else {
				EnemyAnts.Add(ant);
			}
        }

        public void DeadAnt(int row, int col, int team)
        {
            // food could spawn on a spot where an ant just died
            // don't overwrite the space unless it is land
            if (map[row, col] == Tile.Land ||
                map[row, col] == Tile.Unseen)
            {
                map[row, col] = Tile.Dead;
            }

            // but always add to the dead list
            Ant killedAnt = new Ant(row, col, team);
#if DEBUG
            Log.WriteLine(string.Format("Ant {0} was killed.", killedAnt));
#endif

            Ant ant;
            if (team == 0)
            {
                // Find in own ants.
                ant = this.MyAnts.Find(entry => entry.Equals(killedAnt));
                this.MyAnts.Remove(ant);
                this.KilledMyAnts.Add(ant);
            }
            else
            {
                ant = killedAnt;
            }  

            if (ant == null)
            {
#if DEBUG
                Log.WriteLine(string.Format("ERROR: Got dead ant {0} in team {1}, but wasn't found in ants list.", killedAnt, team));
#endif
            }  

            this.DeadTiles.Add(ant);        
        }

		public void AddFood (int row, int col) {
			map[row, col] = Tile.Food;
            Location foodLocation = new Location(row, col);
            this.VisibleFoodTiles.Add(foodLocation);
            if (!this.KnownFood.Contains(foodLocation))
            {
                this.KnownFood.Add(foodLocation);
                this.NewFood.Add(foodLocation);
            }
		}

		public void RemoveFood (int row, int col) {
			// an ant could move into a spot where a food just was
			// don't overwrite the space unless it is food
            if (map[row, col] == Tile.Food ||
                map[row, col] == Tile.Unseen)
            {
				map[row, col] = Tile.Land;
            }
            Location foodLocation = new Location(row, col);
            if (!this.VisibleFoodTiles.Remove(foodLocation))
            {
#if DEBUG
                Log.WriteLine(string.Format("Tried to remove food which wasn't in visible food tiles."));
#endif
            }

            this.KnownFood.Remove(foodLocation);
            this.RemovedFood.Add(foodLocation);
		}

		public void AddWater (int row, int col) {
			map[row, col] = Tile.Water;
		}

		public void AntHill (int row, int col, int team) {

            // There could be an ant on the hill, 
            // so just change tile if there wasn't something specific on the tile before.
            if (map[row, col] == Tile.Land ||
                map[row, col] == Tile.Unseen)
            {
                map[row, col] = Tile.Hill;
            }

			AntHill hill = new AntHill (row, col, team);
            if (team == 0)
                MyHills.Add(hill);
            else
            {
                VisibleEnemyHills.Add(hill);
                if (!this.KnownEnemyHills.Contains(hill))
                {
                    this.KnownEnemyHills.Add(hill);
                }
            }
		}
        #endregion

        public bool IsVisible(Location location)
        {
            return this.map[location.Row, location.Col] != Tile.Unseen;
        }

        public bool IsAccessible(Location location)
        {
            return this.mapAccessible[location.Row, location.Col];
        }

		/// <summary>
		/// Gets whether <paramref name="location"/> is passable or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
		/// <seealso cref="GetIsUnoccupied"/>
		public bool GetIsPassable (Location location) {
			return map[location.Row, location.Col] != Tile.Water;
		}
		
		/// <summary>
		/// Gets whether <paramref name="location"/> is occupied or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
		public bool GetIsUnoccupied (Location location) {
			return GetIsPassable(location) && map[location.Row, location.Col] != Tile.Ant;
		}
		
		/// <summary>
		/// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
		/// </summary>
		/// <param name="location">The starting location.</param>
		/// <param name="direction">The direction to move.</param>
		/// <returns>The new location, accounting for wrap around.</returns>
		public Location GetDestination (Location location, Direction direction) {
			Location delta = Ants.Aim[direction];
			
			int row = (location.Row + delta.Row) % Height;
			if (row < 0) row += Height; // because the modulo of a negative number is negative

			int col = (location.Col + delta.Col) % Width;
			if (col < 0) col += Width;
			
			return new Location(row, col);
        }

        public List<Location> GetPassableNeighbours(Location location)
        {
            List<Location> neighbours = new List<Location>();
            foreach (KeyValuePair<Direction, Location> aim in Ants.Aim)
            {
                Location delta = aim.Value;

                int row = (location.Row + delta.Row) % Height;
                if (row < 0) row += Height; // because the modulo of a negative number is negative

                int col = (location.Col + delta.Col) % Width;
                if (col < 0) col += Width;

                // Check if passable.
                if (map[row, col] == Tile.Water)
                {
                    continue;
                }

                neighbours.Add(new Location(row, col));
            }
            return neighbours;
        }

		/// <summary>
		/// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The first location to measure with.</param>
		/// <param name="loc2">The second location to measure with.</param>
		/// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
		public int GetDistance (Location loc1, Location loc2) {
			int d_row = Math.Abs(loc1.Row - loc2.Row);
			d_row = Math.Min(d_row, Height - d_row);
			
			int d_col = Math.Abs(loc1.Col - loc2.Col);
			d_col = Math.Min(d_col, Width - d_col);
			
			return d_row + d_col;
        }

        public int GetStraightSquareDistance(Location loc1, Location loc2)
        {
            int d_row = Math.Abs(loc1.Row - loc2.Row);
            d_row = Math.Min(d_row, Height - d_row);

            int d_col = Math.Abs(loc1.Col - loc2.Col);
            d_col = Math.Min(d_col, Width - d_col);

            return d_row * d_row + d_col * d_col;
        }

		/// <summary>
		/// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The location to start from.</param>
		/// <param name="loc2">The location to determine directions towards.</param>
		/// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
		public ICollection<Direction> GetDirections (Location loc1, Location loc2) {
			List<Direction> directions = new List<Direction>();
			
			if (loc1.Row < loc2.Row) {
				if (loc2.Row - loc1.Row >= Height / 2)
					directions.Add(Direction.North);
				if (loc2.Row - loc1.Row <= Height / 2)
					directions.Add(Direction.South);
			}
			if (loc2.Row < loc1.Row) {
				if (loc1.Row - loc2.Row >= Height / 2)
					directions.Add(Direction.South);
				if (loc1.Row - loc2.Row <= Height / 2)
					directions.Add(Direction.North);
			}
			
			if (loc1.Col < loc2.Col) {
				if (loc2.Col - loc1.Col >= Width / 2)
					directions.Add(Direction.West);
				if (loc2.Col - loc1.Col <= Width / 2)
					directions.Add(Direction.East);
			}
			if (loc2.Col < loc1.Col) {
				if (loc1.Col - loc2.Col >= Width / 2)
					directions.Add(Direction.East);
				if (loc1.Col - loc2.Col <= Width / 2)
					directions.Add(Direction.West);
			}
			
			return directions;
		}


        public void SetSeen(int row, int col)
        {
            if (this.map[row, col] == Tile.Unseen)
            {
                this.map[row, col] = Tile.Land;
            }
        }

        /*
        public List<Location> GetFrontier()
        {
            List<Location> frontier = new List<Location>();
            for (int row = 0; row < this.Height; ++row)
            {
                for (int col = 0; col < this.Width; ++col)
                {
                    // Check if unseen.
                    if (this.map[row, col] != Tile.Unseen)
                    {
                        continue;
                    }

                    // Check if one of the neighbours 
                }
            }
            return frontier;
        }*/

        /// <summary>
        ///   Helper array for flood fill methods.
        /// </summary>
        private bool[,] mapAccessed;

        /// <summary>
        ///   Map which indicates which tiles are accessible.
        /// </summary>
        private bool[,] mapAccessible;

        public List<Location> GetFrontier()
        {
            return this.frontier;
        }
        
        /// <summary>
        ///   Frontier tiles.
        /// </summary>
        List<Location> frontier = new List<Location>();

        public List<Location> AddedFrontierTiles = new List<Location>();
        public List<Location> RemovedFrontierTiles = new List<Location>();

        public void UpdateAccessibilityMapAndFrontier()
        {
            // Clear maps.
            Array.Clear(this.mapAccessed, 0, this.mapAccessed.Length);
            Array.Clear(this.mapAccessible, 0, this.mapAccessible.Length);

            this.AddedFrontierTiles.Clear();
            this.RemovedFrontierTiles.Clear();

            Stack<Location> stack = new Stack<Location>();
            foreach (Ant ant in this.MyAntsTurn)
            {
                stack.Push(ant);
            }

#if DEBUG
            Log.WriteLine(string.Format("Flood fill starting locations: {0}.", this.MyAntsTurn.Count));
#endif

            while (stack.Count > 0)
            {
                Location location = stack.Pop();

                // Check if already checked.
                if (this.mapAccessed[location.Row, location.Col])
                {
                    continue;
                }

                //Log.WriteLine(string.Format("Checking location {0}.", location));

                this.mapAccessed[location.Row, location.Col] = true;

                // Check if unseen.
                Tile tile = this.map[location.Row, location.Col];
                if (tile == Tile.Unseen)
                {
                    if (!this.frontier.Contains(location))
                    {
                        this.AddedFrontierTiles.Add(location);
                        this.frontier.Add(location);
                    }
                    this.mapAccessible[location.Row, location.Col] = true;
                    continue;
                }
                else
                {
                    if (this.frontier.Remove(location))
                    {
                        this.RemovedFrontierTiles.Add(location);
                    }

                    if (tile != Tile.Water)
                    {
                        this.mapAccessible[location.Row, location.Col] = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                // Add neighbours.
                stack.Push(new Location((location.Row + 1) % this.Height, location.Col));
                stack.Push(new Location((location.Row + this.Height - 1) % this.Height, location.Col));
                stack.Push(new Location(location.Row, (location.Col + 1) % this.Width));
                stack.Push(new Location(location.Row, (location.Col + this.Width - 1) % this.Width));
            }
        }

        /// <summary>
        ///   Precomputed vision offsets of an ant.
        /// </summary>
        private List<Location> visionOffsets = new List<Location>();

        /// <summary>
        ///   Vision of ants.
        /// </summary>
        private bool[,] vision;

        public void Update()
        {
#if DEBUG || STATS
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif

#if DEBUG
            // Check that MyAnts was updated correctly.
            this.MyAnts.Sort();
            this.MyAntsTurn.Sort();
            if (!this.MyAnts.SequenceEqual(this.MyAntsTurn))
            {
                Log.WriteLine(string.Format("ERROR: MyAnts list wasn't updated correctly. Count: MyAnts {0}, MyAntsTurn {1}.", this.MyAnts.Count, this.MyAntsTurn.Count));
                Log.WriteLine(string.Format("MyAnts:     {0}.", string.Join(", ", this.MyAnts.Select(entry => entry.ToString()).ToArray())));
                Log.WriteLine(string.Format("MyAntsTurn: {0}.", string.Join(", ", this.MyAntsTurn.Select(entry => entry.ToString()).ToArray())));
            }
#endif

            // Set visible land tiles.
            Array.Clear(this.vision, 0, this.vision.Length);
            foreach (Ant ant in this.MyAntsTurn)
            {
                foreach (Location visionOffset in this.visionOffsets)
                {
                    Location location = new Location((ant.Row + visionOffset.Row + this.Height) % this.Height, (ant.Col + visionOffset.Col + this.Width) % this.Width);
                    this.vision[location.Row, location.Col] = true;
                    this.SetSeen(location.Row, location.Col);
                }
            }

            // Update known ant hills.
            for (int index = this.KnownEnemyHills.Count - 1; index >= 0; --index)
            {
                AntHill antHill = this.KnownEnemyHills[index];

                // Check if an ant can see the location of the hill currently.
                if (!this.vision[antHill.Row, antHill.Col])
                {
                    continue;
                }

                // Check if hill still exists.
                if (!this.VisibleEnemyHills.Contains(antHill))
                {
                    this.KnownEnemyHills.RemoveAt(index);
                }
            }

            // Update known food tiles.
            for (int index = this.KnownFood.Count - 1; index >= 0; --index)
            {
                Location foodLocation = this.KnownFood[index];

                // Check if an ant can see the location of the hill currently.
                if (!this.vision[foodLocation.Row, foodLocation.Col])
                {
                    continue;
                }

                // Check if food still exists.
                if (this.map[foodLocation.Row, foodLocation.Col] != Tile.Food)
                {
                    this.KnownFood.RemoveAt(index);
                    this.RemovedFood.Add(foodLocation);
                }
            }

            // Compute frontier and accessible tiles.
            this.UpdateAccessibilityMapAndFrontier();

#if DEBUG
            // Check frontier.
            foreach (Location frontierTile in this.frontier)
            {
                if (this.map[frontierTile.Row, frontierTile.Col] == Tile.Water)
                {
                    Log.WriteLine(string.Format("ERROR: Frontier tile was water: {0}.", frontierTile));
                }
            }
#endif

#if DEBUG
            // Check accessibility map.
            for (int row = 0; row < this.Height; ++row)
            {
                for (int col = 0; col < this.Width; ++col)
                {
                    {
                        if (this.mapAccessible[row, col] &&
                            this.map[row, col] == Tile.Water)
                        {
                            Log.WriteLine(string.Format("ERROR: Accessible tile was water: ({0}, {1}).", row, col));
                        }
                    }
                }
            }
#endif

#if DEBUG || STATS
            Log.WriteLine(string.Format("Duration Game State Update: {0}ms.", stopwatch.ElapsedMilliseconds));
#endif
        }
    }
}

