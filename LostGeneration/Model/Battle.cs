using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model {
    class Battle {
        private static int sCounter = 0;

        public EventHandler Ended;
        public EventHandler<BattleEventArgs> FighterAdded;

        public int ID { get; private set; }

        private List<Fighter> fighters;
        private Queue<Fighter> turnQueue;
        private Fighter currentFighter;

        public Battle() {
            ID = sCounter++;
            fighters = new List<Fighter>();
            turnQueue = new Queue<Fighter>();
            currentFighter = null;
        }

        public bool Step() {
            // Check battle end conditions
            if (checkEnd()) {
                Ended(this, EventArgs.Empty);
                return false;
            }

            // Fill the turn queue and sort it by speed
            if (turnQueue.Count() == 0) {
                if (fighters.Count() == 0) {
                    return true;
                }

                turnQueue = new Queue<Fighter>(fighters);
                turnQueue.OrderBy(fighter => fighter.Stats.Speed);
            }

            // Assign first fighter
            if (currentFighter == null) {
                currentFighter = turnQueue.Dequeue();
                currentFighter.PreStep();
            } else if (!currentFighter.IsAlive() || currentFighter.Step(this)) {
                currentFighter = null;
            }

            return false;
        }

        public void AddFighter(Fighter fighter) {
            fighters.Add(fighter);
            FighterAdded(this, new BattleEventArgs {
                Fighter = fighter
            });
        }

        public bool HasFighter(Fighter fighter) {
            return fighters.Contains(fighter);
        }

        private bool checkEnd() {
            // If there exists two opposing teams with living fighters,
            // the battle continues

            if (fighters.Count() == 0) {
                return true;
            }

            bool opposition = true;

            // O(N^2) :(
            foreach (Fighter f in fighters) {
                foreach (Fighter o in fighters) {
                    if (f == o) {
                        continue;
                    } else if (f.IsAlive() && o.IsAlive() && !f.Team.IsFriendly(o.Team)) {
                        opposition = false;
                        break;
                    }
                }
            }

            return opposition;
        }
    }
}
