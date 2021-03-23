using System;
using System.Collections.Generic;
using System.Text;

namespace ShootEmUp.Animations
{
    public class Timing<T>
    {
        int[] timings;
        T[] elements;

        int cursor;
        int lifetime;
        int maxLife;

        public Timing(int[] timing, T[] elem)
        {
            // Check input
            if (timing == null || elem == null || timing.Length != elem.Length)
                throw new Exception("Timings and Elements are not valid in Timing constructor!\n" +
                    "Timings: " + timing + "\n" +
                    "Elements:" + elem + "\n" +
                    "Timing Length: " + ((timing == null) ? "N/A" : timing.Length.ToString()) + "\n" +
                    "Element Length: " + ((elem == null) ? "N/A" : elem.Length.ToString()));

            // Make timings a timeline
            // Example: [5, 8, 2, 4] -> [5, 13, 15, 19]
            timings = new int[timing.Length];
            int sum = 0;
            for (int i = 0; i < timing.Length; i++)
            {
                sum += timing[i];
                timings[i] = sum;
            }
            maxLife = sum;

            elements = elem;

            // Initialize cursor and lifetime
            cursor = 0;
            lifetime = 0;
        }

        public void Update()
        {
            // Don't update if expired
            if (IsExpired())
            {
                return;
            }

            // Increase lifetime
            lifetime++;
            if (timings[cursor] <= lifetime)
            {
                cursor++;
            }
        }

        // Reset timing
        public void Reset()
        {
            lifetime = 0;
            cursor = 0;
        }

        // Check if still running timings
        public bool IsExpired()
        {
            return (cursor >= timings.Length);
        }

        public T GetCurrElement()
        {
            // Ensure not expired
            if (IsExpired())
                throw new Exception("Cannot get element of expired timing! First item: " + elements[0].ToString());

            // Return element at cursor
            return elements[cursor];
        }

        public int GetMaxLifetime()
        {
            return maxLife;
        }

        public void SetCurrTime(int time)
        {
            if (time >= maxLife)
                throw new Exception("Attempted to set time of Timing higher than max lifetime!");
            else
            {
                // Save time
                lifetime = time;

                // Reset cursor
                cursor = 0;

                // Increase cursor to match lifetime
                while (timings[cursor] <= lifetime)
                {
                    cursor++;
                }
            }
        }

        public void Synchronize(Timing<T> change)
        {
            // If it won't fit into animation
            if (lifetime > change.GetMaxLifetime())
            {
                throw new Exception("Cannot synchronize animations! Lifetime: " + lifetime + "; Other's max lifetime: " + change.GetMaxLifetime() + ".");
            }
            else
            {
                change.SetCurrTime(lifetime);
            }
        }
    }
}
