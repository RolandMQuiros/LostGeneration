using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model {
    abstract class AttackSkill {
        public string Name { get; private set; }

        public AttackSkill(string name) {
            Name = name;
        }

        public abstract int Apply(Fighter attacker, Fighter target, int damage, byte elements);
    }
}
