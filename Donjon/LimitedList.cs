using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Donjon.Entities;

namespace Donjon {
    class LimitedList<T> : IEnumerable<T> {
        private List<T> items;

        public int Capacity { get; }
        public int Count => items.Count;

        public LimitedList(int capacity) {
            Capacity = capacity;
            items = new List<T>();
        }

        public T this[int index] {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public bool Add(T item) {
            if (items.Count < Capacity) {
                items.Add(item);
                return true;
            }
            return false;
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