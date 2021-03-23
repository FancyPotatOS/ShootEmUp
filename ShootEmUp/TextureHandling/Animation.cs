using Microsoft.Xna.Framework.Graphics;
using ShootEmUp.Animations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.TextureHandling
{
    public class Animation
    {
        UInt64 lastUpdateID;

        public readonly Timing<Texture2D> timings;

        public Animation(int[] frameTiming, Texture2D[] textures)
        {
            timings = new Timing<Texture2D>(frameTiming, textures);

            lastUpdateID = SEU.instance.updateID;
        }

        // Reset animation to beginning
        public void Reset()
        {
            timings.Reset();
        }

        // Get current texture in animation
        public Texture2D GetTexture()
        {
            return timings.GetCurrElement();
        }

        public int[] GetSizeOfCurrentAnimation()
        {
            Texture2D curr = timings.GetCurrElement();
            return new int[] { curr.Width, curr.Height };
        }

        public void Update()
        {
            // If already updated, do not update again
            if (SEU.instance.updateID <= lastUpdateID)
                return;

            // Otherwise this is the latest update ID
            lastUpdateID = SEU.instance.updateID;

            // Update the timing
            timings.Update();
        }

        // Whether past lifetime
        public bool Expired()
        {
            return timings.IsExpired();
        }

        public void Synchronize(Animation change)
        {
            timings.Synchronize(change.timings);
        }

        public int GetLifetime()
        {
            return timings.GetMaxLifetime();
        }

        public void SetCurrTime(int time)
        {
            timings.SetCurrTime(time);
        }
    }
}
