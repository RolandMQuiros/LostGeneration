using System;
using Microsoft.Xna.Framework;

namespace LostGeneration.Model {
    class EntityEventArgs : EventArgs {
        public Entity Entity;
    }

    class DirectionEventArgs: EventArgs {
        public Direction From;
        public Direction To;
    }

    class PositionEventArgs : EventArgs {
        public Point From;
        public Point To;
    }

    class FighterEventArgs : EventArgs {
        public Fighter Target;
        public int Health;
        public int Mana;
        public Stats Stats;
        public PhysicalElement PhysicalElement;
        public AttackElement AttackElement;
        public SupportElement SupportElement;
        public StatusEffect StatusEffect;
    }

    class CombatantEventArgs : EventArgs {
        
    }
}