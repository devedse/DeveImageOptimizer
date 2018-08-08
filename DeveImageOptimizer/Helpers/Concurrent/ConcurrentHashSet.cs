using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeveImageOptimizer.Helpers.Concurrent
{
    public class ConcurrentHashSet<T> : IEnumerable<T>
    {
        private ConcurrentDictionary<T, byte> internalDict;
        private const byte DefaultDictValue = 0;

        public ConcurrentHashSet()
        {
            internalDict = new ConcurrentDictionary<T, byte>();
        }

        public ConcurrentHashSet(IEnumerable<T> values)
        {
            internalDict = new ConcurrentDictionary<T, byte>(values.Select(t => new KeyValuePair<T, byte>(t, DefaultDictValue)));
        }

        public bool Add(T value)
        {
            return TryAdd(value);
        }

        public bool TryAdd(T value)
        {
            return internalDict.TryAdd(value, DefaultDictValue);
        }

        public bool Contains(T value)
        {
            return internalDict.ContainsKey(value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in internalDict)
            {
                yield return item.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
