using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donjon
{
    class LimitedList<T> : IEnumerable<T>
    {
        private List<T> items;

        public int Capacity { get; }
        public int Count => items.Count;

        public LimitedList(int capacity)
        {
            Capacity = capacity;
            items = new List<T>();
        }

        public bool Add(T item)
        {
            if (items.Count < Capacity)
            {
                items.Add(item);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
