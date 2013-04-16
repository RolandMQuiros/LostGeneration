using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LostGeneration.Model {
    class Fighter {
        public delegate void FighterHandler(Fighter fighter);

        private static ulong sCounter = 0;

        public ulong ID { get; private set; }
        public string Name;
        public int Health;
        public int Mana;
        public Stats Stats;
        public Style Style;
        public Point Position;
        public Team Team;

        public bool IsDisabled;
        public bool IsEngaged;
        public bool IsRanged;
        public bool TryEscaping;
        public bool CanLink;
        public bool IsLinkable;

        public event EventHandler TurnStarted;
        public event EventHandler<FighterEventArgs> Attacked;
        public event EventHandler<FighterEventArgs> Linked;
        public event EventHandler<FighterEventArgs> Unlinked;
        public event EventHandler<FighterEventArgs> Damaged;
        public event EventHandler<FighterEventArgs> Healed;
        public event EventHandler<FighterEventArgs> Supported;
        public event EventHandler Killed;
        public event EventHandler<FighterEventArgs> Revived;
        public event EventHandler Escaped;
        public event EventHandler<FighterEventArgs> StatusInflicted;
        public event EventHandler<FighterEventArgs> StatusRecovered;

        private Fighter next;
        private List<Fighter> children;
        private Dictionary<Type, StatusEffect> statusEffects;

        public Fighter() {
            ID = sCounter++;
            Health = 0;
            Mana = 0;
            Stats = new Stats();
            Position = new Point();

            IsDisabled = false;
            IsEngaged = false;
            IsRanged = false;
            TryEscaping = false;
            CanLink = false;
            IsLinkable = false;

            next = null;
            children = new List<Fighter>();
            statusEffects = new Dictionary<Type, StatusEffect>();
        }

        public bool IsAlive() {
            return Health <= 0;
        }

        public void PreStep() {
            // Notify listeners that this Fighter's turn has started
            if (IsAlive()) {
                TurnStarted(this, EventArgs.Empty);
            }

            // Apply status effects
            foreach (KeyValuePair<Type, StatusEffect> s in statusEffects) {
                if (s.Value.Step(this)) {
                    statusEffects.Remove(s.Key);
                    s.Value.OnRemove(this);
                    StatusRecovered(this, new FighterEventArgs {
                        Target = this,
                        StatusEffect = s.Value
                    });
                }
            }
        }

        public bool Step(Battle battle) {
            if (IsDisabled) {
                return true;
            }

            return stepInternal(battle);
        }

        public Fighter GetRoot() {
            Fighter root = this;
            while (root.next != null && root.next != root) {
                root = root.next;
                if (root == this) {
                    break;
                }
            }

            return root;
        }

        public void ForEach(FighterHandler func) {
            Dictionary<ulong, bool> explored = new Dictionary<ulong, bool>();
            forEachInternal(explored, func);
        }

        public int ChildrenCount() {
            return children.Count();
        }

        public void Hurt(int damage) {
            if (Health > 0) {
                Health -= damage;
                Damaged(this, new FighterEventArgs {
                    Health = -damage
                });
                if (Health <= 0) {
                    Health = 0;
                    Killed(this, EventArgs.Empty);
                }
            }
        }

        public int Heal(int health) {
            bool wasAlive = IsAlive();
            Health += health;
            if (Health > Stats.MaxHealth) {
                Health = Stats.MaxHealth;
            }
            Healed(this, new FighterEventArgs {
                Health = health
            });

            if (IsAlive() && !wasAlive) {
                Revived(this, new FighterEventArgs {
                    Health = health
                });
            }

            return health;
        }

        public void AddStatusEffect(StatusEffect effect) {
            if (effect != null) {
                Type type = effect.GetType();
                if (statusEffects.ContainsKey(type)) {
                    statusEffects[type].Stack(effect);
                } else {
                    statusEffects[type] = effect;
                    effect.OnAdd(this);
                }

                StatusInflicted(this, new FighterEventArgs {
                    StatusEffect = statusEffects[type]
                });
            }
        }

        protected bool stepInternal(Battle battle) {
            return true;
        }

        private void forEachInternal(Dictionary<ulong, bool> explored, FighterHandler func) {
            if (explored.ContainsKey(ID)) {
                return;
            }

            // Mark this node as explored
            explored.Add(ID, true);

            // Explore children
            foreach (Fighter c in children) {
                c.forEachInternal(explored, func);
            }

            // Apply callback
            func(this);
        }

        private void attack(Fighter target) {
            if (!target.IsAlive()) {
                return;
            }

            // Calculate distance modifiers
            float modifier = 1.0f;

            // Get damage count
            int damage = Stats.Attack;
            if (next == this) {
                damage += Stats.Attack / 2;
            }

            // Fix this calculation later
            byte elements = (byte)(1 << (int)Style.AttackElement);
            foreach (Fighter c in children) {
                if (c != null && c.IsAlive()) {
                    // Accumulate /something/
                    damage += c.Stats.Magic;

                    // Accumulate attack elements
                    elements |= (byte)(1 << (int)Style.AttackElement);
                }
            }

            // Apply damage modifier
            damage = (int)(damage * modifier);

            // Apply attack skill
            if (Style.AttackSkill != null) {
                damage = Style.AttackSkill.Apply(this, target, damage, elements);
            }

            // Unlink
            unlinkAll();

            // Notify observers
            Attacked(this, new FighterEventArgs {
                Target = target,
                Health = -damage
            });
        }

        private void link(Fighter target) {
            if (Team.IsFriendly(target.Team)) {
                // Link to root of target's tree
                Fighter root = target.GetRoot();

                // Check if fighters are already linked
                bool alreadyLinked = false;
                foreach (Fighter c in root.children) {
                    if (this == c) {
                        alreadyLinked = true;
                        break;
                    }
                }

                // Link the two fighters
                if (!alreadyLinked) {
                    next = target;
                    root.children.Add(this);

                    // Apply link skill
                    if (Style.LinkSkill != null) {
                        Style.LinkSkill.Apply(this, target);
                    }

                    // Notify
                    Linked(this, new FighterEventArgs {
                        Target = root
                    });
                }
            }
        }

        private void unlink() {
            if (next != null) {
                Fighter oldNext = next;
                next.children.Remove(this);
                next = null;

                // Notify
                Unlinked(this, new FighterEventArgs {
                    Target = oldNext
                });

                // Remove link skill
                if (Style.LinkSkill == null) {
                    Style.LinkSkill.End(this, oldNext);
                }
            }
        }

        private void unlinkInternal(Fighter fighter) {
            if (fighter.next != null) {
                Fighter oldNext = fighter.next;
                fighter.next = null;

                Unlinked(fighter, new FighterEventArgs {
                    Target = oldNext
                });
            }

            fighter.children.Clear();
        }

        private void unlinkAll() {
            ForEach(unlinkInternal);
        }

        private void support(Fighter target) {
            if (Team.IsFriendly(target.Team)) {
                // Temporary heal
                int healed = target.Heal(1);
                Supported(this, new FighterEventArgs {
                    SupportElement = SupportElement.Heal,
                    Health = healed
                });

                unlinkAll();
            }
        }

        private void hit(Fighter by, int damage, byte elements) {
            ForEach(delegate(Fighter fighter) {
                // Apply damage
                int inflicted = damage - fighter.Stats.Defense;
                Hurt(inflicted);

                // Destroy all links
                unlinkInternal(fighter);
            });
        }


    }
}
