using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model {
    public enum StatusEffectType {
        None,
        Poison,
        Paralyze,
        Silence,
        Pariah,
        Blindness,
        Weaken,
        KnockDown,
        Buff,
        Steel,
        Haste,
        Inspire,
        Skeptic,
        Barrier,
        Mirror,
        Sponge,
        Void
    }

    abstract class StatusEffect {
        public StatusEffectType Type;

        public StatusEffect(StatusEffectType type) {
            Type = type;
        }

        public abstract void Stack(StatusEffect other);
        public abstract void OnAdd(Fighter fighter);
        public abstract void OnRemove(Fighter fighter);
        public abstract bool Step(Fighter fighter);
    }
}
