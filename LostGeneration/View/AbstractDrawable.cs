using Microsoft.Xna.Framework.Graphics;

namespace LostGeneration.View {
    abstract class AbstractDrawable {
        public int Depth = 0;
        public bool IsVisible = true;

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
