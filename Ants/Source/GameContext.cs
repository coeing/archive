using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.Source.Pathfinding;
using Ants.Source.Entities;

namespace Ants.Source
{
    public class GameContext
    {
        public GameState GameState;

        public TurnState TurnState;

        public Pathfinder Pathfinder;

        public bool IsWalkableNextTurn(Location location)
        {
            // Check if walkable.
            if (!this.GameState.GetIsPassable(location))
            {
                return false;
            }

            // Check if other ant has already an order to go to this location.
            if (!this.TurnState.IsFreeNextTurn(location))
            {
                return false;
            }

            return true;
        }
    }
}
