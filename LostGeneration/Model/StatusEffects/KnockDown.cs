using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model.StatusEffects {
    class KnockDown : StatusEffect {
        int turns;

        public KnockDown(int turns)
            : base(StatusEffectType.KnockDown) {
                this.turns = turns;
        }

        public override void Stack(StatusEffect other) {
            if (other is KnockDown) {
                KnockDown otherKnockDown = (KnockDown)other;
                turns += otherKnockDown.turns;
            }
        }

        public override void OnAdd(Fighter fighter) {
            fighter.IsDisabled = true;
        }

        public override bool Step(Fighter fighter) {
            // Constantly force the disabling, since another status effect
            // might turn it off
            fighter.IsDisabled = true;

            // Decrement turn counter
            turns--;
            if (turns <= 0) {
                return true;
            }

            return false;
        }

        public override void OnRemove(Fighter fighter) {
            fighter.IsDisabled = false;
        } 
    }
}
