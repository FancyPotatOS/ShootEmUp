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

        UInt64 lastUpdateID;

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

            lastUpdateID = SEU.instance.updateID;
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

        public int[] GetSizeOfCurrentAnimation()
        {
            return new int[] { texs[currTex].tex.Width, texs[currTex].tex.Height };
        }

        public void Update()
        {
            // If already updated, do not update againn
            if (SEU.instance.updateID <= lastUpdateID)
                return;

            // Otherwise this is the latest update ID
            lastUpdateID = SEU.instance.updateID;

            currTime++;
        }

        // Whether past lifetime
        public bool Expired()
        {
            return currTime >= lifetime;
        }

        public void Synchronize(Animation change)
        {
            // If it won't fit into animation
            if (currTime > change.GetLifetime())
            {
                throw new Exception("Cannot synchronize animations! Currtime: " + currTime + "; Other's lifetime: " + change.GetLifetime() + ".");
            }
            else
            {
                change.SetCurrTime(currTime);
            }
        }

        public int GetLifetime()
        {
            return lifetime;
        }

        public void SetCurrTime(int time)
        {
            currTime = time;
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
