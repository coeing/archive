using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.Source.Entities;
using System.Diagnostics;

namespace Ants.Source.Pathfinding
{
    public class Pathfinder
    {
        private GameContext context;

        private int[,] gScore;
        private int[,] hScore;
        private int[,] fScore;
        private bool[,] explored;

        public Pathfinder(GameContext context)
        {
            this.context = context;
            this.gScore = new int[this.context.GameState.Width, this.context.GameState.Height];
            this.hScore = new int[this.context.GameState.Width, this.context.GameState.Height];
            this.fScore = new int[this.context.GameState.Width, this.context.GameState.Height];
            this.explored = new bool[this.context.GameState.Width, this.context.GameState.Height];
            this.parentMap = new Location[this.context.GameState.Width, this.context.GameState.Height];
        }

        private List<Location> BuildPath(Location currentNode)
        {
            Location parent = this.parentMap[currentNode.Col, currentNode.Row];
            if (parent != null)
            {
                List<Location> path = BuildPath(parent);
                path.Add(currentNode);
                return path;
            }
            else
            {
                return new List<Location> {};
            }
        }

        private Location[,] parentMap;

        public List<Location> FindPath(Location start, Location target)
        {
#if DEBUG
            Log.WriteLine(string.Format("Searching path from {0} to {1}.", start, target));
#endif
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (!this.context.GameState.IsAccessible(target))
            {
#if DEBUG
                Log.WriteLine("Target not reachable.");
#endif
                return null;
            }

            // Check if target is walkable.
            if (!this.context.GameState.GetIsPassable(target))
            {
#if DEBUG
                Log.WriteLine("Target occupied.");
#endif
                return null;
            }

            // Special case: target is one tile away.
            if (this.context.GameState.GetDistance(start, target) == 1)
            {
                if (this.context.GameState[target] == Tile.Food ||
                    !this.context.TurnState.IsFreeNextTurn(target))
                {
#if DEBUG
                    Log.WriteLine("Target blocked.");
#endif
                    return null;
                }
            }

            List<Location> frontier = new List<Location>();

            // Clear parent map.
            //Array.Clear(this.parentMap, 0, this.parentMap.Length);

            // Clear explored map.
            Array.Clear(this.explored, 0, this.explored.Length);
            
            // Add start tile.
            frontier.Add(start);
            this.parentMap[start.Col, start.Row] = null;
            this.gScore[start.Col, start.Row] = 0;
            int hScoreStart = this.ComputeHScore(start, target);
            this.hScore[start.Col, start.Row] = hScoreStart;

            /*Stopwatch stopwatchFindBestNode = new Stopwatch();
            Stopwatch stopwatchAddingNeighbours = new Stopwatch();
            Stopwatch stopwatchComputeHScore = new Stopwatch();
            Stopwatch stopwatchFindNeighbours = new Stopwatch();
            Stopwatch stopwatchCheckNeighbourExplored = new Stopwatch();
            Stopwatch stopwatchCheckNeighbourNextLocation = new Stopwatch();
*/
            bool firstStep = true;
            while (frontier.Count > 0)
            {
                //stopwatchFindBestNode.Start();
                Location node = this.FindBestNode(frontier);
                //stopwatchFindBestNode.Stop();

                if (node.Equals(target))
                {
                    List<Location> path = this.BuildPath(node);

                    //long duration = stopwatch.ElapsedMilliseconds;
                    //long durationFindBestNode = stopwatchFindBestNode.ElapsedMilliseconds;
                    //long durationAddingNeighbours = stopwatchAddingNeighbours.ElapsedMilliseconds;
                    //Log.WriteLine(string.Format("Found path with {0} nodes (Duration: {1}ms, FindBestNode: {2}ms, AddingNeighbours: {3}ms [FindNeighbours: {4}ms, CheckNeighbourExplored: {5}ms, CheckNeighbourNextLocation: {6}ms, ComputeHScore: {7}ms]).", 
                    //    path.Count, duration, durationFindBestNode, durationAddingNeighbours, stopwatchFindNeighbours.ElapsedMilliseconds, stopwatchCheckNeighbourExplored.ElapsedMilliseconds, stopwatchCheckNeighbourNextLocation.ElapsedMilliseconds, stopwatchComputeHScore.ElapsedMilliseconds));
            
#if DEBUG
                    Log.WriteLine(string.Format("Found path with {0} nodes (Duration: {1}ms).",
                        path.Count, stopwatch.ElapsedMilliseconds));
#endif

                    return path;
                }
                //Log.WriteLine(string.Format("Trying node {0}", node));

                // Remove node.
                frontier.Remove(node);

                // Set explored.
                this.explored[node.Col, node.Row] = true;
                
                // Check if visible.
                if (!this.context.GameState.IsVisible(node))
                {
                    continue;
                }

                //stopwatchAddingNeighbours.Start();
                //stopwatchFindNeighbours.Start();
                List<Location> neighbours = this.context.GameState.GetPassableNeighbours(node);
                //stopwatchFindNeighbours.Stop();
                foreach (Location neighbour in neighbours)
                {
                    //stopwatchCheckNeighbourExplored.Start();
                    // Check if already explored.
                    if (this.explored[neighbour.Col, neighbour.Row])
                    {
                        //stopwatchCheckNeighbourExplored.Stop();
                        continue;
                    }
                    //stopwatchCheckNeighbourExplored.Stop();

                    if (firstStep)
                    {
                        // Check if food was spawned right next to ant.
                        if (this.context.GameState[neighbour] == Tile.Food)
                        {
                            this.explored[neighbour.Col, neighbour.Row] = true;
                            continue;
                        }

                        // Check if other ant has already an order to go to this location.
                        // TODO: This does only matter if target is one tile away.
                        //stopwatchCheckNeighbourNextLocation.Start();
                        if (!this.context.TurnState.IsFreeNextTurn(neighbour))
                        {
#if DEBUG
                            //stopwatchCheckNeighbourNextLocation.Stop();
                            Log.WriteLine(string.Format("{0} already occupied by other ant.", neighbour));
#endif
                            this.explored[neighbour.Col, neighbour.Row] = true;
                            continue;
                        }
                        //stopwatchCheckNeighbourNextLocation.Stop();
                    }

                    // Compute g score.
                    int gScore = this.gScore[node.Col, node.Row] + 1;
                    bool betterScore = false;

                    if (!frontier.Contains(neighbour))
                    {
                        // Not in frontier yet.
                        frontier.Add(neighbour);
                        betterScore = true;
                    }
                    else if (gScore < this.gScore[neighbour.Col, neighbour.Row])
                    {
                        // New costs are smaller than old path to neighbour.
                        betterScore = true;
                    }

                    if (betterScore)
                    {
                        this.parentMap[neighbour.Col, neighbour.Row] = node;
                        this.gScore[neighbour.Col, neighbour.Row] = gScore;
                        //stopwatchComputeHScore.Start();
                        int hScore = this.ComputeHScore(neighbour, target);
                        //stopwatchComputeHScore.Stop();
                        this.hScore[neighbour.Col, neighbour.Row] = hScore;
                        this.fScore[neighbour.Col, neighbour.Row] = gScore + hScore;
                    }
                }
                //stopwatchAddingNeighbours.Stop();

                firstStep = false;
            }

#if DEBUG
            Log.WriteLine("ERROR: Found no path.");
#endif
            return null;
        }

        private int ComputeHScore(Location location, Location target)
        {
            return this.context.GameState.GetDistance(location, target);
        }

        private Location FindBestNode(List<Location> frontier)
        {
            Location bestNode = null;
            int bestScore = int.MaxValue;
            foreach (Location node in frontier)
            {
                int score = this.fScore[node.Col, node.Row];
                if (score < bestScore)
                {
                    bestNode = node;
                    bestScore = score;
                }
            }
            return bestNode;
        }
    }
}
