using System.Collections;
using System.Collections.Generic;

namespace Lib {
    public class LimitedList<T> : IEnumerable<T> {
        private readonly List<T> items;

        public int Capacity { get; }
        public int Count => items.Count;
        public bool IsFull => items.Count >= Capacity;

        public LimitedList(int capacity) {
            Capacity = capacity;
            items = new List<T>();
        }

        public T this[int index] {
            get { return items[index]; }
            set { items[index] = value; }
        }


        public bool Add(T item)
        {
            if (IsFull) return false;
            items.Add(item);
            return true;
        }

        public bool Remove(T item) {
            return items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator() {
            foreach (var item in items) {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }
}