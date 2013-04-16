using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace LostGeneration.Model {
    struct Team {
        public uint Bits;
        public uint Allies;

        public Team(uint bits = 0, uint allies = 0) {
            Bits = bits;
            Allies = allies;
        }

        public bool IsFriendly(Team other) {
            return Bits == other.Bits ||
                   (Bits & other.Allies) != 0;
        }

        public bool Equals(Team other) {
            return Bits == other.Bits;
        }
    }
}
