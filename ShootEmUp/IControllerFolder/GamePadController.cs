using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlsHandler
{
    class GamePadController : IController
    {

        /*  Pairing Variables   */
        int curr;
        bool connecting;
        /**/

        StringTypePair<Buttons>[] pairs;

        string[] all;

        List<Buttons> preButs;
        List<Buttons> newButs;
        List<Buttons> accButs;

        int gamePadIndex;

        public static Array ButtonsValues;
        public static bool[] taken;

        public GamePadController(/** /int gpi/**/)
        {
            pairs = new StringTypePair<Buttons>[0];

            preButs = new List<Buttons>();
            accButs = new List<Buttons>();
            newButs = new List<Buttons>();

            gamePadIndex = -1;

            // Ensure taken is initialized
            if (taken == null)
            {
                taken = new bool[4];
            }

            /**/
            // Set all button values as array if not initialized
            if (ButtonsValues == null)
            {
                ButtonsValues = Enum.GetValues(typeof(Buttons));
            }
            /**/
        }

        public bool IsPressed(string name)
        {
            // Find and store index of pair with matching name
            int index = -1;
            for (int i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].name == name)
                {
                    index = i;
                    break;
                }
            }
            // Did not find, throw appropriate exception
            if (index == -1)
            {
                if (pairs.Length == 0)
                {
                    throw new Exception("GamePad's names have not been initialized yet!");
                }
                else
                {
                    throw new Exception("Cannot find button name '" + name + "'!");
                }
            }

            // Whether current pressed keys contains the requested key
            return preButs.Contains(pairs[index].value);
        }

        public void Update()
        {
            // Untake disconnected controller
            if (gamePadIndex >= 0 && taken[gamePadIndex] && !GamePad.GetState(gamePadIndex).IsConnected)
            {
                taken[gamePadIndex] = false;
                gamePadIndex = -1;
            }
            // If gamepadIndex is not taken
            if (gamePadIndex < 0 || !taken[gamePadIndex])
            {
                // Decide what index I am that is not taking in static array
                int index = 0;
                foreach (PlayerIndex pi in Enum.GetValues(typeof(PlayerIndex)))
                {
                    // If not taken and connected
                    if (!taken[index] && GamePad.GetState(pi).IsConnected)
                    {
                        // Take it
                        taken[index] = true;
                        gamePadIndex = index;
                        break;
                    }
                    index++;
                }
            }

            // Get the current state (To avoid running over and over again)
            GamePadState gps = GamePad.GetState(gamePadIndex);

            preButs = new List<Buttons>();
            // For each type of button
            foreach (Buttons but in ButtonsValues)
            {
                // If pressed, add
                if (gps.IsButtonDown(but))
                {
                    preButs.Add(but);
                }
            }

            // Find pressed keys that are not accounted for, these are new
            newButs = preButs.FindAll(but => !accButs.Contains(but));

            // Account for all pressed keys
            accButs = preButs;
        }

        public void SetValues(string[] all)
        {
            this.all = all;
            // Default to left shoulder keys for each
            pairs = new StringTypePair<Buttons>[all.Length];;
            for (int i = 0; i < all.Length; i++)
            {
                pairs[i] = new StringTypePair<Buttons>(all[i], Buttons.LeftShoulder);
            }
        }

        public StringTypePair<object>[] GetValues()
        {
            StringTypePair<object>[] stp = new StringTypePair<object>[pairs.Length];
            for (int i = 0; i < stp.Length; i++)
            {
                stp[i] = new StringTypePair<object>(pairs[i].name, null);
            }
            return stp;
        }

        public bool IsPairing()
        {
            return connecting;
        }

        public bool IsDone()
        {
            return curr == all.Length;
        }

        public void StartPairing()
        {
            if (connecting)
            {
                throw new Exception("Already connecting gamepad, cannot start pairing!");
            }
            else
            {
                connecting = true;
                curr = 0;
            }
        }

        public int CurrPairing()
        {
            return curr;
        }

        public void NextPairing()
        {
            if (!connecting)
            {
                throw new Exception("Cannot get next pairing, not currently connecting");
            }
            else if (curr >= pairs.Length)
            {
                throw new Exception("No more controls to pair!");
            }
            else
            {
                // Get first new key pressed
                Update();
                if (newButs.Count == 0)
                {
                    return;
                }

                // Set as pair, with the name
                pairs[curr] = new StringTypePair<Buttons>(all[curr], newButs[0]);

                // Go to next key
                curr++;
            }
        }

        public void EndPairing()
        {
            if (!connecting)
            {
                throw new Exception("Currently not connecting keyboard, cannot end pairing!");
            }
            else
            {
                connecting = false;
                curr = pairs.Length;
            }
        }
    }
}
