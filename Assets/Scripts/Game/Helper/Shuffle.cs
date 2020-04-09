using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Helper
{
    public static class Shuffle<T>
    {
        public static Queue<T> ToQueue(IEnumerable<T> indexes)
        {
            var queue = new Queue<T>();
            var shuffled = indexes.ToArray().OrderBy(x => Random.value);
            foreach (var item in shuffled) queue.Enqueue(item);
            return queue;
        }
        
        public static List<T> ToList(IEnumerable<T> indexes)
        {
            return indexes.ToArray().OrderBy(x => Random.value).ToList();
        }
    }
}