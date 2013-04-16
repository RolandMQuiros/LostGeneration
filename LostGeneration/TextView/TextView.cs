using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LostGeneration.TextView {
    class TextView {
        Model.Dungeon dungeon;

        public TextView() {
            dungeon = new Model.Dungeon();

            int[,] grid = new int[,] {
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
            };

            Model.Terrain[,] data = new Model.Terrain[20, 15];
            for (int j = 0; j < data.GetLength(1); j++) {
                for (int i = 0; i < data.GetLength(0); i++) {
                    switch (grid[i, j]) {
                        case 0:
                            data[i, j] = Model.Terrain.Wall;
                            break;
                        case 1:
                            data[i, j] = Model.Terrain.Floor;
                            break;
                    }
                }
            }

            dungeon.SetGrid(data);
        }

        public void Update() {
            Draw();
        }

        public void Draw() {
            for (int j = 0; j < dungeon.Height; j++) {
                for (int i = 0; i < dungeon.Width; i++) {
                    Point point = new Point(i, j);
                    Model.Dungeon.Cell cell = dungeon.GetCell(point);
                    List<Model.Entity> entities = dungeon.GetEntitiesAt(point);

                    bool entityAt = false;
                    for (int k = 0; k < entities.Count(); k++) {
                        if (entities[i] is Model.Entities.Combatant) {
                            Console.Write('@');
                            entityAt = true;
                            break;
                        }
                    }

                    if (!entityAt) {
                        switch (cell.Terrain) {
                            case Model.Terrain.Floor:
                                Console.Write('.');
                                break;
                            case Model.Terrain.Wall:
                                Console.Write('#');
                                break;
                        }
                    }
                }
                Console.Write('\n');
            }
        }
    }
}
