using System.Collections.Generic;
using Ants.Source.Entities;
using Ants.Source;
using System.Linq;

namespace Ants
{
    public enum OrderType
    {
        Move,

        Food,
        Explore,
        FightHill,
        Wandering,
    }

    public class Order
    {
        public override string ToString()
        {
            return string.Format("{0}: {1} at {2}", this.Ant, this.Type, this.Target);
        }

        /// <summary>
        ///   Type.
        /// </summary>
        public OrderType Type;

        /// <summary>
        ///   Ant which this order is for.
        /// </summary>
        public Ant Ant;

        /// <summary>
        ///   Long term target.
        /// </summary>
        public Location Target;

        /// <summary>
        ///   Distance to target.
        /// </summary>
        public float DistanceToTarget;

        /// <summary>
        ///   Next move location.
        /// </summary>
        public Location NextLocation { get { return this.PathToTarget.First(); }}

        /// <summary>
        ///   Path to target;
        /// </summary>
        public List<Location> PathToTarget;

        /// <summary>
        ///   Number of turns this order existed.
        /// </summary>
        public int Turns;

        public bool ShouldBeRemoved(GameContext context)
        {
            if (this.Type == OrderType.Wandering ||
                this.Type == OrderType.Move)
            {
                return true;
            }

            // Check if ant got killed.
            if (context.GameState.KilledMyAnts.Contains(this.Ant))
            {
#if DEBUG
                Log.WriteLine(string.Format("Removing order because ant was killed: {0}.", this));
#endif
                return true;
            }

            if (this.Type == OrderType.Food)
            {
                // Check if food was removed.
                if (context.GameState.RemovedFood.Contains(this.Target))
                {
#if DEBUG
                    Log.WriteLine(string.Format("Removing order because food was removed: {0}.", this));
#endif
                    return true;
                }
            }
            else if (this.Type == OrderType.Explore)
            {
                // Check if new food available.
                if (context.GameState.NewFood.Count > 0 &&
                    context.GameState.NewFood.Any(food => context.GameState.GetStraightSquareDistance(food, this.Ant) < context.GameState.ViewRadius2))
                {
#if DEBUG
                    Log.WriteLine(string.Format("Removing order because new food found: {0}.", this));
#endif
                    return true;
                }
                // Check if tile got explored by me.
                if (context.GameState.RemovedFrontierTiles.Contains(this.Target) &&
                    context.GameState.GetStraightSquareDistance(this.Ant, this.Target) > context.GameState.ViewRadius2)
                {
#if DEBUG
                    Log.WriteLine(string.Format("Removing order because tile was explored: {0}.", this));
#endif
                    return true;
                }
            }

            // Check if path contains more nodes.
            if (this.PathToTarget.Count < 2)
            {
#if DEBUG
                Log.WriteLine(string.Format("{0}Removing order because target reached: {1}.", this.Type == OrderType.Food ? "Error:" : "", this));
#endif
                return true;
            }

            // Remove first path node.
            this.PathToTarget.RemoveAt(0);

            // Check if path of order is still walkable.
            if (!this.Ant.DoMovePositionNeighbour(context, this.PathToTarget.First()))
            {
#if DEBUG
                Log.WriteLine(string.Format("Path to order target has to be recomputed: {0}.", this));
#endif

                // Search new path.
                if (!this.Ant.DoMovePosition(context, this.Target, ref this.PathToTarget))
                {
#if DEBUG
                    Log.WriteLine(string.Format("Removing order because path couldn't be continued: {0}.", this));
#endif
                    return true;
                }
            }

            return false;
        }
    }

    public class TurnState
    {
        public Stack<Ant> IdleAnts = new Stack<Ant>();

        public List<Order> Orders = new List<Order>();

        public bool[,] AntsNextTurn;

        public List<IStrategy> Strategies;

        public List<Location> UngatheredFood = new List<Location>();

        public List<Location> UnexploredTiles = new List<Location>();

        public bool IsFreeNextTurn(Location location)
        {
            // Check if other ant has already an order to go to this location.
            return !this.AntsNextTurn[location.Row, location.Col];
        }

        private IGameState gameState;

        public TurnState(IGameState gameState)
        {
            this.gameState = gameState;
        }

        public void AddOrder(Order order)
        {
            // Remove old orders.
            // TODO: This need to be made more performant.
            Order oldOrder = this.Orders.FirstOrDefault(entry => entry.Ant == order.Ant);
            if (oldOrder != null)
            {
                // Remove old order.
                this.Orders.Remove(oldOrder);

                // Move ant back on next turn map.
                this.AntsNextTurn[oldOrder.NextLocation.Row, oldOrder.NextLocation.Col] = false;
#if VERBOSE
                Log.WriteLine(string.Format("FREE {0}.", oldOrder.NextLocation));
#endif
            }
            else
            {
                this.AntsNextTurn[order.Ant.Row, order.Ant.Col] = false;
#if VERBOSE
                Log.WriteLine(string.Format("FREE {0}.", order.Ant));
#endif
            }

#if DEBUG
            Log.WriteLine(string.Format("Adding order: {0}.", order));
#endif

            this.Orders.Add(order);

            if (this.AntsNextTurn[order.NextLocation.Row, order.NextLocation.Col])
            {
#if DEBUG
                Log.WriteLine(string.Format("ERROR: Order says ant {0} should move to a tile {1} which is already occupied.", order.Ant, order.NextLocation));
#endif
            }

            // Move ant on next turn map.
            this.AntsNextTurn[order.NextLocation.Row, order.NextLocation.Col] = true;
#if VERBOSE
            Log.WriteLine(string.Format("BLOCKED {0}.", order.NextLocation));
#endif
        }

        public void CancelOrder(Order order)
        {
#if DEBUG
            Log.WriteLine(string.Format("Removing order: {0}.", order));
#endif

            // Remove other order.
            this.IdleAnts.Push(order.Ant);

            // Keep order to move to new location, but remove target.
            order.Type = OrderType.Move;
            order.Target = null;
        }
    }
}