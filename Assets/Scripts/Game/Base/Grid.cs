using System;
using UnityEngine;

namespace Game.Base
{
    public class Grid<T>
    {
        public readonly int width;
        public readonly int height;
        public readonly T[] items;

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;
            items = new T[width * height];
        }

        public bool Set(Vector2Int position, T item)
        {
            return Set(position.x, position.y, item);
        }
        
        public bool Set(int x, int y, T item)
        {
            if (!Withing(x, y)) return false;
            var index = GetIndex(x, y);
            items[index] = item;
            return true;
        }

        public T Get(int x, int y)
        {
            if (!Withing(x, y)) throw new Exception("Out of range");
            var index = GetIndex(x, y);
            return items[index];
        }
        
        public T Get(Vector2Int position)
        {
            return Get(position.x, position.y);
        }
        
        public bool TryGet(Vector2Int position, out T item)
        {
            return TryGet(position.x, position.y, out item);
        }
        
        public bool TryGet(int x, int y, out T item)
        {
            if (!Withing(x, y))
            {
                item = default;
                return false;
            }
            var index = GetIndex(x, y);
            item = items[index];
            return true;
        }

        public bool Withing(Vector2Int position)
        {
            return Withing(position.x, position.y);
        }

        public bool Withing(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private int GetIndex(int x, int y)
        {
            return GetIndex(x, y, width);
        }

        public Vector2Int GetIndexCoordinates(int i)
        {
            return GetCoordinates(i, width);
        }

        public static Vector2Int GetCoordinates(int index, int width)
        {
            var x = index % width;
            var y = (index - x) / width;
            return new Vector2Int(x, y);
        }
        
        public static int GetIndex(int x, int y, int width)
        {
            return y * width + x;
        }
    }
}