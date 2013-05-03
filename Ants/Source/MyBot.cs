using System;
using System.Collections.Generic;
using Ants.Source.Entities;
using Ants.Source;
using System.Diagnostics;

using System.Linq;

namespace Ants {

	public class MyBot : Bot {

        private GameContext context = new GameContext();

#if DEBUG || STATS
        int minDuration = int.MaxValue;
        int maxDuration = int.MinValue;

        long maxAntTurnDuration = long.MinValue;
#endif

        public override void DoSetup(GameState state)
        {
#if DEBUG || STATS
            Log.Clear();
            Log.WriteLine(string.Format("TurnTime {0}ms, LoadTime {1}ms", state.TurnTime, state.LoadTime));
            Log.WriteLine(string.Format("Seeds: {0}", state.Seed));
#endif

            this.context.GameState = state;

            // Add pathfinder.
            this.context.Pathfinder = new global::Ants.Source.Pathfinding.Pathfinder(this.context);

            // Add turn state.
            this.context.TurnState = new TurnState(this.context.GameState);
        }

        // DoTurn is run once per turn
        public override void DoTurn(GameState state) {

            // Clear turn state.
#if DEBUG || STATS
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            this.NewTurn();
#if DEBUG || STATS
            Log.WriteLine(string.Format("Duration Context NewTurn: {0}ms.", stopwatch.ElapsedMilliseconds));
#endif
            
#if DEBUG
            Log.WriteLine(string.Format("Ants: {0} [{1}].", context.GameState.MyAnts.Count, string.Join(";", context.GameState.MyAnts.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Ants Turn: {0}.", context.GameState.MyAntsTurn.Count, string.Join(";", context.GameState.MyAntsTurn.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Added Ants: {0} [{1}].", context.GameState.AddedMyAnts.Count, string.Join(";", context.GameState.AddedMyAnts.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Killed Ants: {0} [{1}].", context.GameState.KilledMyAnts.Count, string.Join(";", context.GameState.KilledMyAnts.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Visible Hills: {0} [{1}].", context.GameState.MyHills.Count, string.Join(";", context.GameState.MyHills.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Visible Enemy Hills: {0} [{1}].", context.GameState.VisibleEnemyHills.Count, string.Join(";", context.GameState.VisibleEnemyHills.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Known Enemy Hills: {0} [{1}].", context.GameState.KnownEnemyHills.Count, string.Join(";", context.GameState.KnownEnemyHills.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Visible Food: {0}.", context.GameState.VisibleFoodTiles.Count));
            Log.WriteLine(string.Format("Known Food: {0}.", context.GameState.KnownFood.Count));
            Log.WriteLine(string.Format("New Food: {0} [{1}].", context.GameState.NewFood.Count, string.Join(";", context.GameState.NewFood.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Removed Food: {0} [{1}].", context.GameState.RemovedFood.Count, string.Join(";", context.GameState.RemovedFood.Select(entry => entry.ToString()).ToArray())));
            Log.WriteLine(string.Format("Ungathered Food: {0} [{1}].", context.TurnState.UngatheredFood.Count, string.Join(";", context.TurnState.UngatheredFood.Select(entry => entry.ToString()).ToArray())));
#endif


//            context.UnseenTiles = context.GameState.GetFrontier();

#if DEBUG
            Log.WriteLine(string.Format("Unexplored tiles: {0}.", context.TurnState.UnexploredTiles.Count));
            Log.WriteLine(string.Format("Frontier: {0}.", context.GameState.GetFrontier().Count));
#endif

            // Determine main strategy.
            this.DetermineStrategies();

            // loop through all my ants and try to give them orders
#if DEBUG || STATS
            long maxTurnAntTurnDuration = 0;
            int numAntTurns = 0;
            stopwatch = new Stopwatch();
#endif
            while (context.TurnState.IdleAnts.Count > 0)
            {
#if DEBUG || STATS
                stopwatch.Reset();
                stopwatch.Start();
                ++numAntTurns;
#endif

                // Get first ant.
                Ant ant = context.TurnState.IdleAnts.Pop();

                Order order = ant.DoTurn(context);
                if (order != null)
                {
                    context.TurnState.AddOrder(order);
                }

#if DEBUG || STATS
                long durationAntTurn = stopwatch.ElapsedMilliseconds;
                maxTurnAntTurnDuration = Math.Max(durationAntTurn, maxTurnAntTurnDuration);

#if DEBUG
                Log.WriteLine(string.Format("Duration: {0}ms", durationAntTurn));
#endif

#endif

                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 200)
                {   
#if DEBUG
                    Log.WriteLine(string.Format("ERROR: Exit because of timeout."));
#endif
                    break;
                }
            }
            
#if DEBUG
            Log.WriteLine("---------------------------------------------------");
#endif

            // Execute orders. 
#if DEBUG || STATS
            Stopwatch stopwatchExecuteOrders = Stopwatch.StartNew();
#endif

            this.ExecuteOrders();

#if DEBUG || STATS
            Log.WriteLine(string.Format("Duration Execute Orders: {0}ms.", stopwatchExecuteOrders.ElapsedMilliseconds));
#endif

#if DEBUG || STATS
            // Log ungathered food.
            if (this.context.TurnState.UngatheredFood.Count > 0)
            {
                Log.WriteLine(string.Format("Ungathered food: {0}", this.context.TurnState.UngatheredFood.Count));
            }
            if (this.context.TurnState.UnexploredTiles.Count > 0)
            {
                Log.WriteLine(string.Format("Unexplored tiles: {0}", this.context.TurnState.UnexploredTiles.Count));
            }
            if (this.context.TurnState.IdleAnts.Count > 0)
            {
                Log.WriteLine(string.Format("Number of idle ants left: {0}", this.context.TurnState.IdleAnts.Count));
            }
            if (this.context.TurnState.IdleAnts.Count > 0 &&
                (this.context.TurnState.UngatheredFood.Count > 0 ||
                this.context.TurnState.UnexploredTiles.Count > 0))
            {
                Log.WriteLine(string.Format("ERROR: There were ungathered food and idle ants."));
            }

            Log.WriteLine(string.Format("Number of orders: {0}", this.context.TurnState.Orders.Count));

            // Store maximum ant turn duration.
            this.maxAntTurnDuration = Math.Max(maxTurnAntTurnDuration, this.maxAntTurnDuration);

            // Log ant turn info.
            Log.WriteLine(string.Format("Number of ant turns: {0} (Ants: {1})", numAntTurns, this.context.GameState.MyAnts.Count));
            Log.WriteLine(string.Format("Max duration ant turn this turn: {0}ms (max: {1}ms)", maxTurnAntTurnDuration, this.maxAntTurnDuration));

            // Log turn info.
            int duration = state.ElapsedTurnTime;
            this.minDuration = Math.Min(duration, this.minDuration);
            this.maxDuration = Math.Max(duration, this.maxDuration);
            Log.WriteLine(string.Format("Duration: {0}ms (min: {1}ms, max: {2}ms)", duration, this.minDuration, this.maxDuration));
#endif
        }

        private void DetermineStrategies()
        {
            this.context.TurnState.Strategies = new List<IStrategy>();

            // TODO: Quick workaround to go for the enemy hills in the end game.
            if (context.GameState.MyAnts.Count > 100)
            {
                this.context.TurnState.Strategies.Add(new DestroyEnemyHillStrategy());
                this.context.TurnState.Strategies.Add(new ExplorerStrategy());
                this.context.TurnState.Strategies.Add(new GatherFoodStrategy());
                this.context.TurnState.Strategies.Add(new WanderStrategy());
            }
            else if (context.GameState.MyAnts.Count > 50)
            {
                this.context.TurnState.Strategies.Add(new DestroyEnemyHillStrategy());
                this.context.TurnState.Strategies.Add(new GatherFoodStrategy());
                this.context.TurnState.Strategies.Add(new ExplorerStrategy());
                this.context.TurnState.Strategies.Add(new WanderStrategy());
            }
            else
            {
                this.context.TurnState.Strategies.Add(new GatherFoodStrategy());
                this.context.TurnState.Strategies.Add(new DestroyEnemyHillStrategy());
                this.context.TurnState.Strategies.Add(new ExplorerStrategy());
                this.context.TurnState.Strategies.Add(new WanderStrategy());
            }
        }

        private void ExecuteOrders()
        {
            foreach (Order order in this.context.TurnState.Orders)
            {
                ICollection<Direction> directions = this.context.GameState.GetDirections(order.Ant, order.NextLocation);
                if (directions.Count != 1)
                {
#if DEBUG
                    Log.WriteLine(string.Format("ERROR: Assumed that locations {0} and {1} are neighbours.", order.Ant, order.NextLocation));
#endif
                    continue;
                }

                // Remove food from ungathered food list.
                if (order.Turns == 0)
                {
                    if (order.Type == OrderType.Food)
                    {
                        if (this.context.TurnState.UngatheredFood.RemoveAll(entry => entry == order.Target) == 0)
                        {
#if DEBUG
                            Log.WriteLine(string.Format("ERROR: Got order for food which wasn't ungathered: {0}.", order));
#endif
                        }
                    }
                    if (order.Type == OrderType.Explore)
                    {
                        if (this.context.TurnState.UnexploredTiles.RemoveAll(entry => entry == order.Target) == 0)
                        {
#if DEBUG
                            Log.WriteLine(string.Format("ERROR: Got order for exploring which wasn't unexplored: {0}.", order));
#endif
                        }
                    }
                }

                Direction direction = directions.First();
                order.Ant.Move(order.NextLocation, direction);
            }
        }


        public void NewTurn()
        {
            // Setup next turn map.
            this.context.TurnState.AntsNextTurn = new bool[this.context.GameState.Height, this.context.GameState.Width];
            foreach (Ant ant in this.context.GameState.MyAnts)
            {
                this.context.TurnState.AntsNextTurn[ant.Row, ant.Col] = true;
            }

#if DEBUG
            Log.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++");
#endif

            // Update next location of orders.
            List<Order> oldOrders = new List<Order>(this.context.TurnState.Orders);
            this.context.TurnState.Orders.Clear();
            foreach (Order order in oldOrders)
            {
                if (order.ShouldBeRemoved(this.context))
                {
                    if (order.Type == OrderType.Food)
                    {
                        // Add to ungathered food.
                        this.context.TurnState.UngatheredFood.Add(order.Target);
                    }
                    if (order.Type == OrderType.Explore)
                    {
                        // Add to unexplored tiles.
                        this.context.TurnState.UnexploredTiles.Add(order.Target);
                    }

                    continue;
                }

                // Keep order.
#if DEBUG
                Log.WriteLine(string.Format("Keeping order: {0}.", order));
#endif
                ++order.Turns;
                this.context.TurnState.AddOrder(order);
            }
#if DEBUG
            Log.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++");
#endif

            // Fill idle ants.
            foreach (Ant ant in this.context.GameState.MyAnts)
            {
                if (this.context.TurnState.Orders.Any(order => order.Ant == ant))
                {
                    continue;
                }

                this.context.TurnState.IdleAnts.Push(ant);
            }

            // Fill ungatherd food.
            this.context.TurnState.UngatheredFood.RemoveAll(food => this.context.GameState.RemovedFood.Contains(food));
            this.context.TurnState.UngatheredFood.AddRange(this.context.GameState.NewFood);

            // Update unexplored tiles.
            this.context.TurnState.UnexploredTiles.RemoveAll(tile => this.context.GameState.RemovedFrontierTiles.Contains(tile));
            this.context.TurnState.UnexploredTiles.AddRange(this.context.GameState.AddedFrontierTiles);
        }
		
		public static void Main (string[] args) {
			new Ants().PlayGame(new MyBot());
		}

	}
	
}