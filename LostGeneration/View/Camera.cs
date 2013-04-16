using Microsoft.Xna.Framework;

namespace LostGeneration.View {
    class Camera {
        public Vector2 Position;
        public Vector2 Size;

        public float X {
            get { return Position.X; }
            set { Position.X = value; }
        }
        public float Y {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public float Left {
            get { return X - (Size.X * 0.5F) / Zoom; }
            set { X = value + (Size.X * 0.5F) / Zoom; }
        }

        public float Top {
            get { return Y - (Size.Y * 0.5F) / Zoom; }
            set { Y = value + (Size.Y * 0.5F) / Zoom; }
        }

        public float Right {
            get { return X + (Size.X * 0.5F) / Zoom; }
            set { X = value - (Size.X * 0.5F) / Zoom; }
        }

        public float Bottom {
            get { return Y + (Size.Y * 0.5F) / Zoom; }
            set { Y = value - (Size.Y * 0.5F) / Zoom; }
        }

        public float Width {
            get { return Size.X; }
            set { Size.X = value; }
        }

        public float Height {
            get { return Size.Y; }
            set { Size.Y = value; }
        }

        public float Zoom;
        public float Rotation;

        public Matrix Transform {
            get {
                return Matrix.CreateTranslation(-Position.X, -Position.Y, 0.0F) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateScale(new Vector3(Zoom, Zoom, 1.0F)) *
                       Matrix.CreateTranslation(new Vector3(Size.X * 0.5F, Size.Y * 0.5F, 0.0F));
            }
        }

        public Camera(float width, float height) {
            Position = new Vector2();
            Size = new Vector2(width, height);
            Zoom = 1.0F;
            Rotation = 0.0F;
        }

        public Camera(Vector2 size) {
            Position = new Vector2();
            Size = size;
            Zoom = 1.0F;
            Rotation = 0.0F;
        }
    }
}
