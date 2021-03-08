using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.TextureHandling
{
    public class Animation
    {
        int currTex;
        int currTime;
        int lifetime;

        TimedTexture[] texs;

        public Animation(int[] frameTiming, Texture2D[] textures)
        {
            if (textures == null || frameTiming == null)
                throw new Exception("Attempted to make an animation with null timings/animations!");
            else if (textures.Length == 0)
                throw new Exception("Attempted to make an animation with no textures!");
            else if (textures.Length != frameTiming.Length)
                throw new Exception("Attempted to make an animation with improper frame timing!");

            // Calculate lifetime
            lifetime = 0;
            for (int i = 0; i < frameTiming.Length; i++)
                lifetime += frameTiming[i];

            // Initalize array
            texs = new TimedTexture[textures.Length];

            // Initalize first frame
            texs[0] = new TimedTexture(0, textures[0]);
            for (int i = 1; i < textures.Length; i++)
                texs[i] = new TimedTexture(texs[i - 1].frame + frameTiming[i - 1], textures[i]);
        }

        // Reset animation to beginning
        public void Reset()
        {
            currTex = 0;
            currTime = 0;
        }

        // Get current texture in animation
        public Texture2D GetTexture()
        {
            // Fault check
            if (Expired())
                throw new Exception("Attempted to get texture for animation that has expired!");
            else
            {
                // If there is another animation, and its for this frame
                if (currTex < texs.Length - 1 && texs[currTex + 1].frame <= currTime)
                {
                    // Update texture pointer
                    currTex++;
                }

                return texs[currTex].tex;
            }
        }

        public void Update()
        {
            currTime++;
        }

        // Whether past lifetime
        public bool Expired()
        {
            return currTime >= lifetime;
        }
    }

    class TimedTexture
    {
        public int frame;
        public Texture2D tex;

        public TimedTexture(int f, Texture2D t)
        {
            frame = f;
            tex = t;
        }
    }
}
