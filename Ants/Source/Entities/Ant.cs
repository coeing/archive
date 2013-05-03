using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ants.Source.Entities
{
    public interface IStrategy
    {
        Order DoTurn(GameContext context, Ant ant);
    }

    public class LocationDistance : IComparable<LocationDistance>
    {
        public Location Location;

        public float Distance;

        public Order Order;

        #region IComparable<LocationDistance> Member

        public int CompareTo(LocationDistance other)
        {
            if (this.Distance < other.Distance)
            {
                return -1;
            }
            else if (this.Distance == other.Distance)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        #endregion
    }

    public class LocationPath : IComparable<LocationPath>
    {
        public Location Location;

        public List<Location> Path = new List<Location>();

        /// <summary>
        ///   Current order to this location.
        /// </summary>
        public Order Order;

        #region IComparable<LocationDistance> Member

        public int CompareTo(LocationPath other)
        {
            if (this.Path == null)
            {
                if (other.Path == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (other.Path == null)
            {
                return -1;
            }

            if (this.Path.Count < other.Path.Count)
            {
                return -1;
            }
            else if (this.Path.Count == other.Path.Count)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        #endregion
    }

    public class StrategyUtils
    {
        public static List<LocationPath> SortLocationsByPath(GameContext context, Location point, IEnumerable<Location> locations)
        {
#if DEBUG
            if (context == null)
            {
                Log.WriteLine("ERROR: Context is null.");
            }
            if (context.GameState == null)
            {
                Log.WriteLine("ERROR: Game state is null.");
            }
            if (locations == null)
            {
                Log.WriteLine("ERROR: Locations is null.");
            }
            if (point == null)
            {
                Log.WriteLine("ERROR: point is null.");
            }
#endif
            List<LocationDistance> locationDistances = SortLocationsByDistance(context, point, locations);

            List<LocationPath> locationPaths = new List<LocationPath>();
            int minPathDistance = int.MaxValue;
            foreach (LocationDistance locationDistance in locationDistances)
            {
                if (locationDistance.Distance >= minPathDistance)
                {
                    break;
                }

                // Skip locations where the current order's path is shorter than the distance.
                Order order = context.TurnState.Orders.FirstOrDefault(entry => entry.Target == locationDistance.Location);
                if (order != null &&
                    order.PathToTarget.Count <= locationDistance.Distance)
                {
                    continue;
                }

                List<Location> path = context.Pathfinder.FindPath(point, locationDistance.Location);
                if (path == null)
                {
                    continue;
                }

                // Skip if path is longer than the current order's path.
                if (order != null &&
                    order.PathToTarget.Count <= path.Count)
                {
                    continue;
                }

                minPathDistance = Math.Min(minPathDistance, path.Count);
                locationPaths.Add(new LocationPath { Location = locationDistance.Location, Path = path, Order = order });
            }
            locationPaths.Sort();
            return locationPaths;
        }

        public static List<LocationDistance> SortLocationsByDistance(GameContext context, Location point, IEnumerable<Location> locations)
        {
#if DEBUG
            if (context == null)
            {
                Log.WriteLine("ERROR: Context is null.");
            }
            if (context.GameState == null)
            {
                Log.WriteLine("ERROR: Game state is null.");
            }
            if (locations == null)
            {
                Log.WriteLine("ERROR: Locations is null.");
            }
            if (point == null)
            {
                Log.WriteLine("ERROR: point is null.");
            }
#endif

            List<LocationDistance> locationDistances = new List<LocationDistance>();
            foreach (Location location in locations)
            {
#if DEBUG
                if (location == null)
                {
                    Log.WriteLine("ERROR: Location is null.");
                }
#endif

                float distance = context.GameState.GetDistance(point, location);
                locationDistances.Add(new LocationDistance { Location = location, Distance = distance });
            }
            locationDistances.Sort();
            return locationDistances;
        }

        public static bool PickTarget(GameContext context, Ant ant, List<LocationDistance> locations, ref LocationDistance target, ref List<Location> path)
        {
#if DEBUG
            Log.WriteLine(string.Format("Picking target from {0} possible locations.", locations.Count));
#endif

            // Determine target position.
            foreach (LocationDistance location in locations)
            {
                // Check if one ant is already moving to this food location.
                Order otherOrder = context.TurnState.Orders.FirstOrDefault(entry => entry.Target == location.Location);
                if (otherOrder != null)
                {
                    // Check if distance is smaller than the distance for the other ant.
                    if (location.Distance >= otherOrder.DistanceToTarget)
                    {
                        continue;
                    }
                }

                if (ant.DoMovePosition(context, location.Location, ref path))
                {
                    if (otherOrder != null)
                    {
                        context.TurnState.CancelOrder(otherOrder);
                    }

                    target = location;
                    return true;
                }
            }

            return false;
        }

        internal static bool PickTarget(GameContext context, Ant ant, List<LocationPath> locations, ref LocationPath target)
        {
#if DEBUG
            Log.WriteLine(string.Format("Picking target from {0} possible locations.", locations.Count));
#endif

            // Determine target position.
            foreach (LocationPath location in locations)
            {
                if (ant.DoMovePath(context, location.Path))
                {
                    if (location.Order != null)
                    {
                        context.TurnState.CancelOrder(location.Order);
                    }

                    target = location;
                    return true;
                }
            }

            return false;
        }
    }


    public class GatherFoodStrategy : IStrategy
    {
        public Order DoTurn(GameContext context, Ant ant)
        {
#if DEBUG
            Log.WriteLine("Trying to gather food");
#endif
            // Check if any ungathered food.
            if (context.TurnState.UngatheredFood.Count == 0)
            {
                return null;
            }
            /*
            // Get food targets sorted by distance.
            List<LocationPath> foodLocations = StrategyUtils.SortLocationsByPath(context, ant, context.TurnState.UngatheredFood);

            // Determine target position.
            LocationPath target = null;
            if (StrategyUtils.PickTarget(context, ant, foodLocations, ref target))
            {
                return new Order { Type = OrderType.Food, Ant = ant, Target = target.Location, DistanceToTarget = target.Path.Count, PathToTarget = target.Path };
            }
            else
            {
                return null;
            }*/

            // Get food targets sorted by distance.
            List<LocationDistance> foodLocations = StrategyUtils.SortLocationsByDistance(context, ant, context.TurnState.UngatheredFood);

            // Determine target position.
            List<Location> path = null;
            LocationDistance target = null;
            if (StrategyUtils.PickTarget(context, ant, foodLocations, ref target, ref path))
            {
                return new Order { Type = OrderType.Food, Ant = ant, Target = target.Location, DistanceToTarget = target.Distance, PathToTarget = path };
            }
            else
            {
                return null;
            }
        }
    }

    public class DestroyEnemyHillStrategy : IStrategy
    {
        public Order DoTurn(GameContext context, Ant ant)
        {
#if DEBUG
            Log.WriteLine("Trying to destroy enemy hill.");
#endif
            // Check if any enemy hills.
            if (context.GameState.KnownEnemyHills.Count == 0)
            {
                return null;
            }
            /*
            // Get targets sorted by distance.
            List<LocationPath> hillLocations = StrategyUtils.SortLocationsByPath(context, ant, context.GameState.KnownEnemyHills.Cast<Location>());

            // Determine target position.
            LocationPath target = null;
            if (StrategyUtils.PickTarget(context, ant, hillLocations, ref target))
            {
                return new Order { Type = OrderType.FightHill, Ant = ant, Target = target.Location, DistanceToTarget = target.Path.Count, PathToTarget = target.Path };
            }
            else
            {
                return null;
            }*/

            List<LocationDistance> hillLocations = StrategyUtils.SortLocationsByDistance(context, ant, context.GameState.KnownEnemyHills.Cast<Location>());

            // Determine target position.
            List<Location> path = null;
            LocationDistance target = null;
            if (StrategyUtils.PickTarget(context, ant, hillLocations, ref target, ref path))
            {
                return new Order { Type = OrderType.FightHill, Ant = ant, Target = target.Location, DistanceToTarget = target.Distance, PathToTarget = path };
            }
            else
            {
                return null;
            }
        }
    }

    public class WanderStrategy : IStrategy
    {
        public Order DoTurn(GameContext context, Ant ant)
        {
#if DEBUG
            Log.WriteLine("Trying wander around.");
#endif

            // Move randomly.
            Random random = new Random();
            int randomOffset = random.Next(Ants.Aim.Keys.Count);
            for (int index = 0; index < Ants.Aim.Keys.Count; ++index)
            {
                Direction direction = Ants.Aim.Keys.ElementAt((randomOffset + index) % Ants.Aim.Keys.Count);
                Location nextLocation = null;
                if (ant.DoMoveDirection(context, direction, ref nextLocation))
                {
                    return new Order { Type = OrderType.Wandering, Ant = ant, PathToTarget = new List<Location> {nextLocation} };
                }
            }

            return null;
        }
    }

    public class ExplorerStrategy : IStrategy
    {
        public Order DoTurn(GameContext context, Ant ant)
        {
#if DEBUG
            Log.WriteLine("Trying exploring.");
#endif
            // Check if any unseen tiles.
            if (context.TurnState.UnexploredTiles.Count == 0)
            {
                return null;
            }

            // Get unseen locations sorted by distance.
            List<LocationDistance> unseenLocations = StrategyUtils.SortLocationsByDistance(context, ant, context.TurnState.UnexploredTiles);

            // Move to first location which isn't target of another ant.
            List<Location> path = null;
            LocationDistance target = null;
            if (StrategyUtils.PickTarget(context, ant, unseenLocations, ref target, ref path))
            {
                return new Order { Type = OrderType.Explore, Ant = ant, Target = target.Location, DistanceToTarget = target.Distance, PathToTarget = path };
            }
            else
            {
                return null;
            }
        }
    }

    public class Ant : TeamLocation, IEquatable<Ant>, IComparable<Ant>
    {
        public Ant(int row, int col, int team)
            : base(row, col, team)
        {
        }

        public Order DoTurn(GameContext context)
        {
#if DEBUG
            Log.WriteLine("---------------------------------------------------");
            Log.WriteLine(string.Format("ANT {0}.", this));
#endif

#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            // Check if ant can move.
            bool canMove = false;
            foreach (Direction direction in Ants.Aim.Keys)
            {
                Location destination = context.GameState.GetDestination(this, direction);
                if (context.IsWalkableNextTurn(destination))
                {
                    canMove = true;
                    break;
                }
            }
#if DEBUG
            Log.WriteLine(string.Format("Duration move check: {0}ms.", stopwatch.ElapsedMilliseconds));
#endif

            if (!canMove)
            {
                return null;
            }


#if DEBUG
            stopwatch = Stopwatch.StartNew();
#endif

            // Try first possible strategy.
            Order order = null;
            foreach (IStrategy strategy in context.TurnState.Strategies)
            {
                // Try to destroy enemy hill.
                order = strategy.DoTurn(context, this);
                if (order != null)
                {
#if DEBUG
                    Log.WriteLine(string.Format("Strategy {0} at {1}.", strategy.GetType(), order.Target));
#endif
                    break;
                }
            }

#if DEBUG
            Log.WriteLine(string.Format("Duration get order: {0}ms.", stopwatch.ElapsedMilliseconds));
#endif

            return order;
        }

        public bool DoMovePosition(GameContext context, Location location, ref List<Location> path)
        {
            // Find path.
            path = context.Pathfinder.FindPath(this, location);
            if (path == null ||
                path.Count == 0)
            {
                return false;
            }

            return this.DoMovePath(context, path);
        }

        public bool DoMovePath(GameContext context, List<Location> path)
        {
            // Get directions to target.
            Location firstNode = path.First();

#if DEBUG
            Log.WriteLine(string.Format("First path node: {0}", firstNode));
#endif

            // Try move to neighbour.
            if (this.DoMovePositionNeighbour(context, firstNode))
            {
                return true;
            }

#if DEBUG
            Log.WriteLine(string.Format("ERROR: Couldn't move to {0} although there was a path found.", firstNode));
#endif

            return false;
        }

        public bool DoMovePositionNeighbour(GameContext context, Location neighbourLocation)
        {
            if (context.GameState.GetDistance(this, neighbourLocation) != 1)
            {
#if DEBUG
                Log.WriteLine(string.Format("ERROR: Assumed that locations {0} and {1} are neighbours.", this, neighbourLocation));
#endif
                return false;
            }

            ICollection<Direction> possibleDirections = context.GameState.GetDirections(this, neighbourLocation);
            if (possibleDirections.Count != 1)
            {
#if DEBUG
                Log.WriteLine(string.Format("ERROR: Neighbour node {0} returns more than one direction to walk on.", neighbourLocation));
#endif
            }

            // try all the directions            
            foreach (Direction direction in possibleDirections)
            {
                Location nextLocation = null;
                if (this.DoMoveDirection(context, direction, ref nextLocation))
                {
                    return true;
                }
            }

            return false;
        }

        public bool DoMoveDirection(GameContext context, Direction direction, ref Location nextLocation)
        {
            // GetDestination will wrap around the map properly
            // and give us a new location
            Location newLocation = context.GameState.GetDestination(this, direction);

            // GetIsPassable returns true if the location is land
            if (context.GameState[newLocation] == Tile.Food ||
                !context.GameState.GetIsPassable(newLocation))
            {
#if DEBUG
                Log.WriteLine(string.Format("Location {0} occupied when tried to move from {1} in direction {2}.", newLocation, this, direction));
#endif
                return false;
            }

            // Check if other ant has already an order to go to this location.
            if (!context.TurnState.IsFreeNextTurn(newLocation))
            {
#if DEBUG
                Log.WriteLine(string.Format("Location {0} already planned for another ant when tried to move from {1} in direction {2}.", newLocation, this, direction));
#endif
                return false;
            }

            nextLocation = newLocation;

            // stop now, don't give 1 and multiple orders
            return true;
        }

        public void Move(Location neighbourLocation, Direction direction)
        {
            // Output.
            System.Console.Out.WriteLine("o {0} {1} {2}", this.Row, this.Col, direction.ToChar());

            // Move ant.
            this.Row = neighbourLocation.Row;
            this.Col = neighbourLocation.Col;

#if VERBOSE
            Log.WriteLine(string.Format("o {0} {1} {2}", this.Row, this.Col, direction.ToChar()));
#endif
        }

        public bool Equals(Ant other)
        {
            return base.Equals(other);
        }

        #region IComparable<Ant> Member

        public int CompareTo(Ant other)
        {
            return base.CompareTo(other);
        }

        #endregion
    }
}
