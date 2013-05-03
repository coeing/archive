using System;
using System.IO;
using System.Collections.Generic;
using Ants.Source.Entities;
using Ants.Source;
using System.Diagnostics;

namespace Ants {

	public class Ants {
		
		public static readonly Location North = new Location(-1, 0);
		public static readonly Location South = new Location(1, 0);
		public static readonly Location West = new Location(0, -1);
		public static readonly Location East = new Location(0, 1);
		
		public static IDictionary<Direction, Location> Aim = new Dictionary<Direction, Location> {
			{ Direction.North, North},
			{ Direction.East, East},
			{ Direction.South, South},
			{ Direction.West, West}
		};
		
		private const string READY = "ready";
		private const string GO = "go";
		private const string END = "end";
		
		private GameState state;


		public void PlayGame(Bot bot) {

			List<string> input = new List<string>();
			
			try {
				while (true) {
					string line = System.Console.In.ReadLine().Trim().ToLower();
					
					if (line.Equals(READY)) {
						ParseSetup(input);
						FinishTurn();
                        input.Clear();
                        bot.DoSetup(state);
                    }
                    else if (line.Equals(GO))
                    {        
#if DEBUG
                        ++this.turn;                
                        Log.WriteLine("################################################################");
                        Log.WriteLine(string.Format("NEW TURN {0}", this.turn));
#endif

						state.StartNewTurn();
						ParseUpdate(input);
						bot.DoTurn(state);
						FinishTurn();
						input.Clear();
					} else if (line.Equals(END)) {
						break;	
					} else {
						input.Add(line);
					}
				}
			} catch (Exception e) {
				#if DEBUG
					FileStream fs = new FileStream("debug.log", System.IO.FileMode.Append, System.IO.FileAccess.Write);
					StreamWriter sw = new StreamWriter(fs);
					sw.WriteLine(e);
					sw.Close();
					fs.Close();
				#endif
			}

        }

        /// <summary>
        ///   Turn number.
        /// </summary>
        public int turn = 0;
		
		// parse initial input and setup starting game state
		private void ParseSetup(List<string> input) {
			int width = 0, height = 0;
			int turntime = 0, loadtime = 0;
			int viewradius2 = 0, attackradius2 = 0, spawnradius2 = 0;
            long seed = 0;
			
			foreach (string line in input) {
				if (line.Length <= 0) continue;
				
				string[] tokens = line.Split();
				string key = tokens[0];
				
				if (key.Equals(@"cols")) {
					width = int.Parse(tokens[1]);
				} else if (key.Equals(@"rows")) {
					height = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"player_seed"))
                {
                    seed = 1;// TODO: This crashes at the moment. Int64.Parse(tokens[1]);
				} else if (key.Equals(@"turntime")) {
					turntime = int.Parse(tokens[1]);
				} else if (key.Equals(@"loadtime")) {
					loadtime = int.Parse(tokens[1]);
				} else if (key.Equals(@"viewradius2")) {
					viewradius2 = int.Parse(tokens[1]);
				} else if (key.Equals(@"attackradius2")) {
					attackradius2 = int.Parse(tokens[1]);
				} else if (key.Equals(@"spawnradius2")) {
					spawnradius2 = int.Parse(tokens[1]);
				}
			}
			
			this.state = new GameState(width, height, 
			                           turntime, loadtime,
                                       viewradius2, attackradius2, spawnradius2, seed);
		}
		
		// parse engine input and update the game state
		private void ParseUpdate(List<string> input)
        {
#if DEBUG || STATS
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			
			foreach (string line in input) {
				if (line.Length <= 0) continue;
								
				string[] tokens = line.Split();
				
				if (tokens.Length >=3) {
					int row = int.Parse(tokens[1]);
					int col = int.Parse(tokens[2]);
					
					if (tokens[0].Equals("a")) {
						state.AddAnt(row, col, int.Parse(tokens[3]));
					} else if (tokens[0].Equals("f")) {
						state.AddFood(row, col);
					} else if (tokens[0].Equals("r")) {
						state.RemoveFood(row, col);
					} else if (tokens[0].Equals("w")) {
						state.AddWater(row, col);
					} else if (tokens[0].Equals("d")) {
                        state.DeadAnt(row, col, int.Parse(tokens[3]));
					} else if (tokens[0].Equals("h")) {
						state.AntHill (row, col, int.Parse(tokens[3]));
					}
				}
            }

#if DEBUG || STATS
            Log.WriteLine(string.Format("Duration Parse Update: {0}ms.", stopwatch.ElapsedMilliseconds));
#endif

            state.Update();
		}

		private void FinishTurn () {
			System.Console.Out.WriteLine(GO);
		}
		
	}
}