using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeveImageOptimizer.Helpers.Concurrent
{
    public class ConcurrentSortedSet<T> : IEnumerable<T>
    {
        private readonly SortedSet<T> internalSet;
        private readonly object lockject = new object();

        public ConcurrentSortedSet()
        {
            internalSet = new SortedSet<T>();
        }

        public ConcurrentSortedSet(IEnumerable<T> values)
        {
            internalSet = new SortedSet<T>(values);
        }

        public bool Add(T value)
        {
            return TryAdd(value);
        }

        public bool TryAdd(T value)
        {
            lock (lockject)
            {
                return internalSet.Add(value);
            }
        }

        public bool Contains(T value)
        {
            lock (lockject)
            {
                return internalSet.Contains(value);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (lockject)
            {
                return internalSet.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (lockject)
            {
                return internalSet.ToList().GetEnumerator();
            }
        }
    }
}
