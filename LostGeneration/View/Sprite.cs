using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LostGeneration.View {
    class Sprite : AbstractDrawable, IUpdateable {
        public Vector2 Position;
        public float X {
            get { return Position.X; }
            set { Position.X = value; }
        }
        public float Y {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public bool IsEnabled;
        public float FramesPerSecond {
            get { return 1.0F / secondsPerFrame; }
            set { secondsPerFrame = 1.0F / value; }
        }

        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effects;
        public bool IsFinished;

        private Texture2D texture;
        private int frameWidth;
        private int frameHeight;
        private int widthInFrames;

        private bool isPlaying;
        private int currentFrame;
        private Dictionary<string, List<Rectangle>> animations;
        private List<Rectangle> currentAnimation;
        private int loops;
        private float secondsPerFrame;
        private float time;

        public Sprite(Texture2D texture, int frameWidth, int frameHeight) {
            IsEnabled = true;
            Color = Color.White;
            Rotation = 0.0F;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Effects = SpriteEffects.None;
            IsFinished = false;

            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            widthInFrames = texture.Width / frameWidth;

            isPlaying = false;
            currentFrame = 0;
            animations = new Dictionary<string, List<Rectangle>>();
            currentAnimation = null;
            loops = 0;
            secondsPerFrame = 1.0F / 30.0F;
            time = 0.0F;
        }

        public void AddAnimation(string name, int[] frames, bool overwrite=true) {
            if (animations.ContainsKey(name) && !overwrite) {
                return;
            }

            List<Rectangle> rects = new List<Rectangle>();
            Rectangle rect = new Rectangle(0, 0, frameWidth, frameHeight);

            foreach (int i in frames) {
                rect.X = (i % widthInFrames) * frameWidth;
                rect.Y = (i / widthInFrames) * frameHeight;
                rects.Add(rect);
            }

            animations.Add(name, rects);
        }

        public void Play(string name, int loops = -1, bool forceRestart = false) {
            if (!animations.ContainsKey(name)) {
                return;
            }
            
            List<Rectangle> anim = animations[name];
            if (currentAnimation == anim && forceRestart) {
                currentFrame = 0;
            } else {
                currentAnimation = anim;
            }
            
            IsFinished = false;
            this.loops = loops;
            isPlaying = true;
        }

        public void Update(GameTime gameTime) {
            if (isPlaying) {
                time += gameTime.ElapsedGameTime.Milliseconds / 1000.0F;
                if (time > secondsPerFrame) {
                    time = 0.0F;
                    currentFrame++;
                    if (currentFrame >= currentAnimation.Count()) {
                        currentFrame = 0;
                        if (loops == 0) {
                            isPlaying = false;
                            IsFinished = true;
                        } else if (loops > 0) {
                            loops--;
                        }
                    }
                }
            }
        }

        override public void Draw(SpriteBatch spriteBatch) {
            if (IsVisible && currentAnimation != null) {
                Rectangle frame = currentAnimation[currentFrame];
                spriteBatch.Draw(texture, Position, frame, Color, Rotation, Origin, Scale, Effects, 0.0F);
            }
        }
    }
}
