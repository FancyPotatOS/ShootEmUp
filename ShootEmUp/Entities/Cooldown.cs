using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.Entities
{
    public class Cooldown
    {
        uint value;
        readonly uint reset;

        public Cooldown(uint val, uint res)
        {
            value = val;
            reset = res;
        }

        // Decrease cooldown
        public void Update()
        {
            if (value > 0)
                value--;
        }

        // Whether cooldown is done
        public bool CanUse()
        {
            return (value == 0);
        }

        // Reset the cooldown
        public void Reset()
        {
            value = reset;
        }

        public void AtMost(uint val)
        {
            value = Math.Min(val, value);
        }

        public void AtLeast(uint val)
        {
            value = Math.Max(val, value);
        }
    }
}
