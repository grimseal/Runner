using System;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "New LevelConfig", menuName = "Level Config", order = 51)]
    public class LevelConfig : ScriptableObject
    {
        public CellConfig[] floors;
        public CellConfig[] walls; 
        public CellConfig[] obstacles;
        public ItemConfig[] items;

        public CellConfig[] allCellsConfigs
        {
            get
            {
                var arr = new CellConfig[floors.Length + walls.Length + obstacles.Length];
                Array.Copy(floors, 0, arr, 
                    0, floors.Length);
                Array.Copy(walls, 0, arr, 
                    floors.Length, walls.Length);
                Array.Copy(obstacles, 0, arr, 
                    floors.Length + walls.Length, obstacles.Length);
                return arr;
            }
        }
    }
}