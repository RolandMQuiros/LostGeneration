using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LostGeneration.View {
    class AutoTileMap {
        public const int AutoTileCount = 16;

        public int Width {
            get {
                if (tiles == null) { return 0; }
                return tiles.GetLength(1);
            }
        }

        public int Height {
            get {
                if (tiles == null) { return 0; }
                return tiles.GetLength(0); 
            }
        }

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        private Texture2D texture;
        private Rectangle[] subRects;

        private int[,] tiles;

        public string DebugOut;

        public AutoTileMap() {
            texture = null;
            subRects = new Rectangle[AutoTileCount];
        }

        public void SetTexture(Texture2D texture, int tileWidth, int tileHeight, int x = 0, int y = 0) {
            if (texture.Width / tileWidth < AutoTileCount - 1) {
                throw new FormatException("Tile dimensions don't allow for enough tiles.  The given subrectangle " +
                    "width and tile width must allow for " + AutoTileCount + " horizontally-lined tiles.");
            }

            this.texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            for (int i = 0; i < AutoTileCount; i++) {
                subRects[i] = new Rectangle(
                    x + (i * TileWidth), y, TileWidth, TileHeight
                );
            }
        }

        public void SetTexture(Texture2D texture, Point tileSize) {
            SetTexture(texture, tileSize.X, tileSize.Y);
        }
        
        public void SetTexture(Texture2D texture, Point tileSize, Point texCoord) {
            SetTexture(texture, tileSize.X, tileSize.Y, texCoord.X, texCoord.Y);
        }

        public void Create(int[,] grid) {
            int width = grid.GetLength(1);
            int height = grid.GetLength(0);

            int[,] newTiles = new int[height, width];
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    if (grid[j, i] == 0) {
                        InsertTile(newTiles, true, i, j);
                    } else {
                        InsertTile(newTiles, false, i, j);
                    }
                }
            }

            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    string bin = Convert.ToString(newTiles[j, i], 2);
                    Console.Write(bin + "\t");
                }
                Console.WriteLine();
            }

            tiles = newTiles;
        }

        public void SetTile(bool occupied, int x, int y) {
            if (InBounds(x, y, Width, Height)) {
                InsertTile(tiles, occupied, x, y);
            }
        }

        public void SetTile(bool occupied, Point position) {
            SetTile(occupied, position.X, position.Y);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera) {
            if (texture == null || subRects == null || tiles == null) {
                return;
            }

            /* Here, we're taking the rotation of the camera and creating a bounding box over the rotated frame.
               That bounding box will determine which tiles to draw. */

            // Get the four corners of the camera
            Vector3[] points = new Vector3[4] {
                new Vector3(camera.Left, camera.Top, 0.0F),
                new Vector3(camera.Right, camera.Top, 0.0F),
                new Vector3(camera.Left, camera.Bottom, 0.0F),
                new Vector3(camera.Right, camera.Bottom, 0.0F)
            };

            Point topLeft = new Point((int)points[0].X, (int)points[0].Y);
            Point bottomRight = new Point((int)points[0].X, (int)points[0].Y);
            Matrix xform = Matrix.CreateRotationZ(camera.Rotation);

            // Transform the camera
            foreach (Vector3 point in points) {//int i = 0; i < points.Length; i++) {
                Vector3 xformed = Vector3.Transform(point, xform);
                
                // Get the maximum and minimum coordinates in the transformation
                if (xformed.X < topLeft.X) { topLeft.X = (int)Math.Floor(xformed.X); }
                else if (xformed.X > bottomRight.X) { bottomRight.X = (int)Math.Ceiling(xformed.X); }

                if (xformed.Y < topLeft.Y) { topLeft.Y = (int)Math.Floor(xformed.Y); } 
                else if (xformed.Y > bottomRight.Y) { bottomRight.Y = (int)Math.Ceiling(xformed.Y); }
            }

            // Convert our extrema from pixels to tiles
            topLeft.X /= TileWidth;
            topLeft.Y /= TileHeight;
            bottomRight.X = (int)Math.Ceiling(bottomRight.X / (float)TileWidth);
            bottomRight.Y = (int)Math.Ceiling(bottomRight.Y / (float)TileHeight);

            // Draw the tiles
            int tile;
            for (int j = topLeft.Y; j < bottomRight.Y; j++) {
                for (int i = topLeft.X; i < bottomRight.X; i++) {
                    if (i < 0 || i >= Width || j < 0 || j >= Height) {
                        continue;
                    }

                    tile = tiles[j, i];
                    spriteBatch.Draw(texture, new Vector2(i * TileWidth, j * TileHeight), subRects[tile], Color.White);
                }
            }
        }

        private bool InBounds(int x, int y, int width, int height) {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private void InsertTile(int[,] tileGrid, bool occupied, int x, int y) {
            int width = tileGrid.GetLength(1);
            int height = tileGrid.GetLength(0);

            // Make sure we're within the map
            if (!InBounds(x, y, width, height)) {
                return;
            }

            // Set the tile at the given location
            if (occupied) {
                tileGrid[y, x] = 0xF; // All four corners of the tile are filled
            } else {
                tileGrid[y, x] = 0;
            }

            // Then, we check and alter the surrounding tiles
            Point[] sides = new Point[] {
                new Point(x, y + 1),        // Bottom
                new Point(x - 1, y),        // Left
                new Point(x, y - 1),        // Top
                new Point(x + 1, y)         // Right
            };

            Point[] corners = new Point[] {
                new Point(x + 1, y + 1),    // Bottom-Right
                new Point(x - 1, y + 1),    // Bottom-Left
                new Point(x - 1, y - 1),    // Top-Left
                new Point(x + 1, y - 1)     // Top-Right
            };

            int sideBits = 3;
            int cornerBit = 1;
            for (int i = 0; i < sides.Length; i++) {
                if (InBounds(sides[i].X, sides[i].Y, width, height)) {
                    if (occupied) {
                        tileGrid[sides[i].Y, sides[i].X] |= sideBits;
                    } else {
                        tileGrid[sides[i].Y, sides[i].X] &= (~sideBits & 0xF);
                    }
                }

                // Circular shift the side bits
                sideBits = ((sideBits << 1) | (sideBits >> 3)) & 0xF;

                if (InBounds(corners[i].X, corners[i].Y, width, height)) {
                    if (occupied) {
                        tileGrid[corners[i].Y, corners[i].X] |= cornerBit;
                    } else {
                        tileGrid[corners[i].Y, corners[i].X] &= (~cornerBit & 0xF);
                    }
                }

                // Circularly shift the corner bits
                cornerBit = (cornerBit << 1) | (cornerBit >> 3) & 0xF;
            }
        }
    }
}
