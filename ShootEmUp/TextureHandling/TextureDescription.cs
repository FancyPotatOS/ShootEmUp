using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootEmUp.TextureHandling
{
    internal class TextureDescription
    {
        public readonly Texture2D tex;
        public readonly Rectangle bound;
        public Color color;
        public int layer;
        public TextureDescription(Texture2D t, Rectangle b, Color c, int l)
        {
            tex = t;
            bound = b;
            color = c;
            layer = l;
        }
    }
}
