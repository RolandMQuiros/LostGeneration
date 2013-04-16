using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LostGeneration.View {
    class DungeonViewOld {
        public const int TileSize = 64;

        private struct Tile {
            public Vector2 Position;
            public Texture2D Texture;
            public Vector2 Origin;
        }

        private Game game;
        private Model.Dungeon dungeon;
        private List<Tile> tiles;

        private Texture2D grassTile;
        private Texture2D treeTile;

        private Vector2 right;
        private Vector2 down;

        public DungeonViewOld(Game game) {
            this.game = game;
            dungeon = null;
            tiles = new List<Tile>();
            grassTile = game.Content.Load<Texture2D>("Sprites\\Terrain\\sprGrass01Temp");
            treeTile = game.Content.Load<Texture2D>("Sprites\\Terrain\\sprBushTemp");

            right = new Vector2(0.894427F, -0.447214F);
            down = new Vector2(0.894427F, 0.447214F);
        }

        public void Attach(Model.Dungeon model) {
            dungeon = model;

            // Load terrain sprites
            tiles.Clear();
            for (int j = 0; j < dungeon.Height; j++) {
                for (int i = 0; i < dungeon.Width; i++) {
                    Point point = new Point(i, j);
                    Model.Dungeon.Cell cell = dungeon.GetCell(point);

                    Tile tile = new Tile();
                    tile.Position = FieldToViewCoordinates(point);

                    if (cell.Terrain == Model.Terrain.Floor) {
                        tile.Origin = new Vector2(64.0F, 32.0F);
                        tile.Texture = grassTile;
                    } else if (cell.Terrain == Model.Terrain.Wall) {
                        tile.Origin = new Vector2(64.0F, 80.0F);
                        tile.Texture = treeTile;
                    }

                    tiles.Add(tile);
                }
            }
            tiles.Sort(CompareTiles);
        }

        public Vector2 FieldToViewCoordinates(Point point) {
            Vector2 pt = new Vector2(TileSize * point.X, TileSize * point.Y);

            Vector2 a = Vector2.Multiply(right, pt.X);
            Vector2 b = Vector2.Multiply(down, pt.Y);

            Vector2 ret = Vector2.Add(a, b);

            return ret;
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (Tile t in tiles) {
                spriteBatch.Draw(t.Texture, t.Position, null, Color.White, 0.0F, t.Origin, 1.0F, SpriteEffects.None, 0.0F);
            }
        }

        private static int CompareTiles(Tile x, Tile y) {
            if (x.Position.Y < y.Position.Y) {
                return -1;
            } else if (x.Position.Y == y.Position.Y) {
                return 0;
            }
            return 1;
        }
    }
}
