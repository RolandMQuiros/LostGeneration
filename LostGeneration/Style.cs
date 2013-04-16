using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration.Model {
    class Style {
        public string Name;
        public string Description;
        public AttackElement AttackElement;
        public SupportElement SupportElement;
        public AttackSkill AttackSkill;
        public LinkSkill LinkSkill;

        private Dictionary<byte, Affinity> affinities;

        public Style() {
            AttackElement = AttackElement.None;
            SupportElement = SupportElement.None;
            AttackSkill = null;
            LinkSkill = null;
            affinities = new Dictionary<byte, Affinity>();
        }

        public Affinity GetAffinity(Element[] elements) {
            byte encoding = getEncoding(elements);
            Affinity affinity = Affinity.None;

            if (affinities.ContainsKey(encoding)) {
                affinity = affinities[encoding];
            }

            return affinity;
        }

        public void SetAffinity(byte elements, Affinity affinity) {
            affinities[elements] = affinity;
        }

        public void SetAffinity(Element[] elements, Affinity affinity) {
            byte encoding = getEncoding(elements);
            affinities[encoding] = affinity;
        }

        private byte getEncoding(Element[] elements) {
            byte encoding = 0;
            foreach (Element e in elements) {
                encoding |= (byte)(1 << (int)e);
            }

            return encoding;
        }
    }
}
