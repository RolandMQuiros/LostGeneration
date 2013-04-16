using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LostGeneration.Model {
    class Dungeon {
        public struct Cell {
            public Terrain Terrain;
            public bool IsVisible;
            public bool IsExplored;

            public Cell(Terrain terrain = Terrain.None, bool isVisible = false, bool isExplored = false) {
                Terrain = terrain;
                IsVisible = isVisible;
                IsExplored = isExplored;
            }
        }

        public event EventHandler<EntityEventArgs> EntityAdded;
        public event EventHandler<EntityEventArgs> EntityRemoved;

        public int Width {
            get {
                if (grid == null) { return 0; }
                return grid.GetLength(0);
            }
        }

        public int Height {
            get {
                if (grid == null) { return 0; }
                return grid.GetLength(1);
            }
        }

        private Cell[,] grid;
        private HashSet<Entity> entities;
        private Entity currentEntity;
        private IEnumerator<Entity> entityIter;
        private Dictionary<Point, List<Entity>> entityTable;
        private Queue<Entity> addedEntities;
        private Queue<Entity> removedEntities;

        public Dungeon() {
            grid = null;
            entities = new HashSet<Entity>();
            entityIter = null;
            entityTable = new Dictionary<Point, List<Entity>>();
            addedEntities = new Queue<Entity>();
            removedEntities = new Queue<Entity>();
        }

        public void SetGrid(Terrain[,] newGrid) {
            grid = new Cell[newGrid.GetLength(0), newGrid.GetLength(1)];
            for (int j = 0; j < newGrid.GetLength(1); j++) {
                for (int i = 0; i < newGrid.GetLength(0); i++) {
                    Cell cell = new Cell(newGrid[i, j], false, false);
                    grid[i, j] = cell;
                }
            }
        }

        public void SetGrid(Cell[,] newGrid) {
            grid = newGrid;
        }

        public void SetCell(Cell cell, Point point) {
            if (InBounds(point)) {
                grid[point.X, point.Y] = cell;
            }
        }

        public void SetCell(Cell cell, int x, int y) {
            if (InBounds(x, y)) {
                grid[x, y] = cell;
            }
        }

        public bool InBounds(Point point) {
            return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
        }

        public bool InBounds(int x, int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public List<Entity> GetEntitiesAt(Point point) {
            List<Entity> ret = new List<Entity>();
            if (InBounds(point) && entityTable.ContainsKey(point)) {
                ret = entityTable[point];
            }
            return ret;
        }

        public List<Entity> GetEntitiesAt(int x, int y) {
            return GetEntitiesAt(new Point(x, y));
        }

        public Cell GetCell(Point point) {
            Cell cell = new Cell();
            if (InBounds(point)) {
                cell = grid[point.X, point.Y];
            }
            return cell;
        }

        public Cell GetCell(int x, int y) {
            Cell cell = new Cell();
            if (InBounds(x, y)) {
                cell = grid[x, y];
            }
            return cell;
        }

        public void AddEntity(Entity entity) {
            addedEntities.Enqueue(entity);
            //EntityAdded(this, new EntityEventArgs { Entity = entity });
        }

        public void RemoveEntity(Entity entity) {
            removedEntities.Enqueue(entity);
        }

        public bool MoveEntity(Entity entity, Point start, Point end) {
            if (start == null || end == null || !InBounds(end) || start.Equals(end)) {
                return false;
            }

            // Retrieve the list of entities at the start position
            List<Entity> entitiesAtStart;
            if (entityTable.ContainsKey(start)) {
                entitiesAtStart = entityTable[start];
            } else {
                // Add a new list for empty positions
                entitiesAtStart = new List<Entity>();
                entityTable.Add(start, entitiesAtStart);
            }

            // Remove the entity from the start bucket
            if (entitiesAtStart.Remove(entity)) {
                entity.Position = end;

                // Retrieve the list of entities at the end position
                List<Entity> entitiesAtEnd;
                if (entityTable.ContainsKey(end)) {
                    entitiesAtEnd = entityTable[end];
                } else {
                    // Adds a new list for empty positions
                    entitiesAtEnd = new List<Entity>();
                    entityTable.Add(end, entitiesAtEnd);
                }

                // Add the entity to the end bucket
                entitiesAtEnd.Add(entity);

                // Move successful
                return true;
            }

            // Move failed
            return false;
        }

        public bool Step() {
            // Add entities to the Set and Table
            while (addedEntities.Count() > 0) {
                Entity toAdd = addedEntities.Dequeue();
                if (entities.Add(toAdd)) {
                    List<Entity> entitiesAt;
                    if (entityTable.ContainsKey(toAdd.Position)) {
                        entitiesAt = entityTable[toAdd.Position];
                    } else {
                        entitiesAt = new List<Entity>();
                        entityTable.Add(toAdd.Position, entitiesAt);
                    }

                    entitiesAt.Add(toAdd);
                    EntityAdded(this, new EntityEventArgs { Entity = toAdd });
                }
            }

            // Clean up any removed entities
            while (removedEntities.Count() > 0) {
                Entity toRemove = removedEntities.Dequeue();
                if (entities.Remove(toRemove)) {
                    List<Entity> entitiesAt;
                    if (entityTable.ContainsKey(toRemove.Position)) {
                        entitiesAt = entityTable[toRemove.Position];
                        entitiesAt.Remove(toRemove);
                        EntityRemoved(this, new EntityEventArgs { Entity = toRemove });
                    }
                }
            }

            // If there are no entities to process, end cycle
            if (entities.Count() == 0) {
                return true;
            }

            // Grab the next entity
            if (currentEntity == null) {
                if (entityIter != null && entityIter.MoveNext()) {
                    currentEntity = entityIter.Current;
                    currentEntity.PreStep();
                } else {
                    entityIter = entities.GetEnumerator();
                }
            }

            // Run the entity's turn
            if (currentEntity != null && currentEntity.Step()) {
                currentEntity.PostStep();
                currentEntity = null;

                // Current entity's turn ends
                return true;
            }
            
            // Current entity's turn continues
            return false;
        }
    }
}
