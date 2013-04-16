using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model {
    abstract class LinkSkill {
        public string Name { get; private set; }

        public LinkSkill(string name) {
            Name = name;
        }

        public abstract void Apply(Fighter linker, Fighter target);
        public abstract void End(Fighter linker, Fighter target);
    }
}
