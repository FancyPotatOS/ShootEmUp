using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlsHandler
{
    class KeyboardController : IController
    {
        /*  Pairing Variables   */
        int curr;
        bool connecting;
        /**/

        StringTypePair<Keys>[] pairs;

        string[] all;

        List<Keys> preKeys;
        List<Keys> newKeys;
        List<Keys> accKeys;

        public KeyboardController()
        {
            pairs = new StringTypePair<Keys>[0];

            preKeys = new List<Keys>();
            accKeys = new List<Keys>();
            newKeys = new List<Keys>();
        }

        public bool IsPressed(string key)
        {
            int index = -1;
            for (int i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].name == key)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                if (pairs.Length == 0)
                {
                    throw new Exception("Keyboards's names have not been initialized yet!");
                }
                else
                {
                    throw new Exception("Cannot find key name '" + key + "'!");
                }
            }

            // Whether current pressed keys contains the requested key
            return preKeys.Contains(pairs[index].value);
        }

        public void Update()
        {
            preKeys = Keyboard.GetState().GetPressedKeys().ToList();
            newKeys = preKeys.FindAll(key => !accKeys.Contains(key));
            accKeys = preKeys;
        }

        public void SetValues(string[] names)
        {
            all = names;
            // Default to tab keys for each
            pairs = new StringTypePair<Keys>[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                pairs[i] = new StringTypePair<Keys>(names[i], Keys.Tab);
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
                throw new Exception("Already connecting keyboard, cannot start pairing!");
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
                if (newKeys.Count == 0)
                {
                    return;
                }

                { }

                // Set as pair, with the name
                pairs[curr] = new StringTypePair<Keys>(all[curr], newKeys[0]);

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
                curr = pairs.Length + 1;
            }
        }
    }
}
