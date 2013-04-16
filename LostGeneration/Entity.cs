using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LostGeneration.Model {
    class Entity {
        private static int ms_idCounter = 0;

        public event EventHandler<PositionEventArgs> Moved;
        public event EventHandler<DirectionEventArgs> DirectionChanged;

        public int ID { get; private set; }
        public bool RemoveMe;
        public bool IsActive;
        public bool IsSolid;
        public Point Position;
        public Direction Direction;

        public int X {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public int Y {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        protected Dungeon dungeon;

        public Entity(Dungeon dungeon, int x = 0, int y = 0, Direction direction = Direction.South) {
            ID = ms_idCounter++;
            IsActive = true;
            RemoveMe = false;
            IsSolid = false;
            Position = new Point(x, y);
            Direction = direction;

            this.dungeon = dungeon;
        }
        
        public virtual void Interact(Entity other) { }
        
        public bool SetPosition(int x, int y) {
            Point destination = new Point(x, y);
            return SetPosition(destination);
        }

        /// <summary>
        /// Moves this Entity to a new position in the Dungeon, if that new position is available.
        /// A position is considered unavailable if it is ouside the bounds of the Dungeon, or if it's occupied by a
        /// wall.
        /// 
        /// Raises Moved if successful.
        /// </summary>
        /// <param name="destination">The new position we're attempting to move to</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool SetPosition(Point destination) {
            // Check if the new position is a wall
            Dungeon.Cell cell = dungeon.GetCell(destination);
            if (cell.Terrain == Terrain.Wall) {
                return false;
            }

            // Check for other entities in the new position
            List<Entity> entities = dungeon.GetEntitiesAt(destination);
            bool canMove = true;
            foreach (Entity other in entities) {
                // Make sure we don't interact with ourselves.  That's nasty.
                if (this == other) {
                    continue;
                }

                // Interact with the other entity
                other.Interact(this);
                Interact(other);
                canMove &= IsSolid && other.IsSolid;
            }

            // Move our filthy selves, if we can take it
            if (canMove && dungeon.MoveEntity(this, Position, destination)) {
                Point start = Position;
                Position = destination;

                // Notify listeners of our groove
                Moved(this, new PositionEventArgs { From = start, To = destination });

                // Move successful
                return true;
            }

            // Move failed
            return false;
        }

        /// <summary>
        /// Moves this Entity to a new position relative to its current one.
        /// 
        /// Will return if the move was successful, i.e. the destination is within the field's boundaries and there
        /// are no walls or solid entities.
        /// 
        /// Raises DirectionChanged if direction is unlocked, and Moved if the move was successful.
        /// </summary>
        /// <param name="direction">Direction to move</param>
        /// <param name="distance">Number of cells to move</param>
        /// <param name="lockDirection">Whether or not to chance direction before moving</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool Move(Direction direction, int distance = 1, bool lockDirection = false) {
            Point destination = new Point(Position.X, Position.Y);

            // Create the offset position based on the direction we're moving in
            switch (direction) {
                case Direction.East:
                    destination.X += distance;
                    break;
                case Direction.South:
                    destination.Y += distance;
                    break;
                case Direction.West:
                    destination.X -= distance;
                    break;
                case Direction.North:
                    destination.Y -= distance;
                    break;
            }

            // If the direction is locked, the entity stays facing the same direction
            if (!lockDirection) {
                // Otherwise, direction changes and we notify listeners
                Direction oldDirection = direction;
                Direction = direction;

                DirectionChanged(this, new DirectionEventArgs { From = oldDirection, To = Direction });
            }

            return SetPosition(destination);
        }

        public virtual void PreStep() { }
        public virtual bool Step() { return false; }
        public virtual void PostStep() { }
    }
}
