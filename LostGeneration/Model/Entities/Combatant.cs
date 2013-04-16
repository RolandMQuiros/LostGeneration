using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model.Entities {
    class Combatant : Entity {
        public EventHandler<CombatantEventArgs> JoinedBattle;
        public EventHandler<CombatantEventArgs> FighterAdded;

        public List<Fighter> Fighters;
        public Team Team;
        public bool IsEngaged { get; private set; }

        private Battle battle;

        public Combatant(Dungeon dungeon, int x, int y, Direction direction = Direction.South)
            : base(dungeon, x, y, direction)
        {
            IsSolid = true;
            Team = new Team();
            IsEngaged = false;

            battle = null;
        }

        public override void Interact(Entity other) {
            if (other is Combatant) {
                Combatant otherCombatant = (Combatant)other;

                // If interacting with an enemy
                if (!Team.IsFriendly(otherCombatant.Team)) {
                    IsEngaged = true;

                    // Join the enemy's battle, or create a new one
                    if (otherCombatant.battle == null) {
                        battle = new Battle();
                    } else {
                        battle = otherCombatant.battle;
                    }

                    // Add fighters to the battle
                    foreach (Fighter f in Fighters) {
                        battle.AddFighter(f);
                    }

                    // We want the battle to notify us when it ends, 
                    // so we can disengage
                    battle.Ended += OnBattleEnd;

                    // Notify
                    JoinedBattle(this, new CombatantEventArgs {
                        Battle = battle
                    });
                }
            }
        }

        public override bool Step() {
            if (IsEngaged) {
                return true;
            }

            return false;
        }

        private void OnBattleEnd(object sender, EventArgs eventArgs) {
            battle = null;
            IsEngaged = false;
        }
    }
}
