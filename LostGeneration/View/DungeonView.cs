using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using LostGeneration.Model;


namespace LostGeneration.View {
    class DungeonView {
        private AutoTileMap tileMap;

        public DungeonView(int tileWidth, int tileHeight) {
            
        }

        public void Attach(Dungeon dungeon) {
            Terrain[,] terrain = new Terrain[dungeon.Width, dungeon.Height];
            for (int j = 0; j < dungeon.Height; j++) {
                for (int i = 0; i < dungeon.Width; i++) {
                    terrain[i, j] = dungeon.GetCell(i, j).Terrain;
                }
            }

            
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera) {
            tileMap.Draw(spriteBatch, camera);
        }
    }
}
