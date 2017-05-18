using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib {
    public class Log {
        private readonly List<string> log = new List<string>();
        private readonly Action<string> writer;
        private int lastCount;

        public Log(Action<string> writer) {
            this.writer = writer;
        }


        public void Add(string message) => log.Add(message ?? "");

        public void Clear() => log.Clear();

        public void Write() {
            foreach (var message in log) {
                foreach (var m in message.Split('\n')) writer(m);
            }
        }

        public void Flush() {
            var add = Math.Max(0, lastCount - log.Count);
            lastCount = log.Count;
            log.AddRange(Enumerable.Repeat("", add));
            Write();
            Clear();
        }
    }
}