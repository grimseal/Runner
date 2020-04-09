using System;
using System.Collections.Generic;
using System.Linq;
using Game.Base;
using Game.Level.Object;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "New CellConfig", menuName = "Cell Config", order = 51)]
    public class CellConfig : LevelObjectConfig
    {
        [SerializeField]
        private CellNeighborRestriction[] restrictions;

        public bool CheckPlacement(float chance, Vector2Int coordinates, Grid<CellConfig> cells)
        {
            return CheckChance(chance) && CheckRestrictions(coordinates, cells);
        }
        
        public bool CheckRestrictions(Vector2Int coordinates, Grid<CellConfig> cells)
        {
            return restrictions == null ||
                   restrictions.Length < 1 ||
                   restrictions.All(restriction =>
                       RestrictionIsPassed(restriction, coordinates, cells));
        }
        
        private static bool RestrictionIsPassed(CellNeighborRestriction restriction, Vector2Int coordinates, 
            Grid<CellConfig> cells)
        {
            if (restriction.cells == null || restriction.cells.Length < 1) return true;
            var c = CellNeighborRestriction.GetDirectionCoordinates(restriction.direction, coordinates);
            switch (restriction.condition)
            {
                case CellNeighborRestriction.Condition.Attend:
                    return cells.Withing(c) && restriction.cells.Contains(cells.Get(c));
                case CellNeighborRestriction.Condition.Forbidden:
                    return !cells.Withing(c) || !restriction.cells.Contains(cells.Get(c));
            }
            return true;
        }

        [Serializable]
        private struct CellNeighborRestriction
        {
            public enum Direction
            {
                Ahead,
                Behind,
                Left,
                Right
            }
        
            public enum Condition
            {
                Attend,
                Forbidden
            }

            public Condition condition;
            public Direction direction;
            public CellConfig[] cells;



            public static Vector2Int GetDirectionCoordinates(Direction direction, Vector2Int form)
            {
                return form + dirVector[direction];
            }


            private static Dictionary<Direction, Vector2Int> dirVector = new Dictionary<Direction, Vector2Int>
            {
                {Direction.Ahead, Vector2Int.up},
                {Direction.Behind, Vector2Int.down},
                {Direction.Left, Vector2Int.left},
                {Direction.Right, Vector2Int.right},
            };
        }
    }


    
}