using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGeneration {
    class GenPlayground {
        public int[] DrunkenWalk(int width, int height, float ratio) {
            Random random = new Random();

            int open = 0;
            int total = width * height;

            int[] ret = new int[total];
            int idx = random.Next(0, total);
            int d = 0;
            int direction;

            while ((float)open / total < ratio) {
                direction = random.Next(1, 5);

                switch (direction) {
                    case 1:
                        d = -width;
                        break;
                    case 2:
                        d = width;
                        break;
                    case 3:
                        d = -1;
                        break;
                    case 4:
                        d = 1;
                        break;
                }

                if (idx + d >= 0 && idx + d < total) {
                    idx += d;
                    ret[idx] = 1;
                    open++;
                }
            }

            return ret;
        }

        
    }
}
