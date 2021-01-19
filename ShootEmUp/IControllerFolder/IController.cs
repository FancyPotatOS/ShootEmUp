using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlsHandler
{
    public interface IController
    {
        void Update();
        bool IsPressed(string name);
        void SetValues(string[] all);
        StringTypePair<object>[] GetValues();
        bool IsPairing();
        bool IsDone();
        void StartPairing();
        void NextPairing();
        int CurrPairing();
        // Bad stop to pairing
        void EndPairing();

        // Returns true when done connecting all
        public static bool Connect(IController[] controllersInOrder, string[] all)
        {
            bool updated = false;
            foreach (IController ic in controllersInOrder)
            {
                // If already pairing
                if (ic.IsPairing())
                {
                    // If not finished
                    if (!ic.IsDone())
                    {
                        // Get next if can, and mark to redo
                        ic.NextPairing();
                        updated = true;
                    }
                }
                // Not currently pairing
                else
                {
                    // Give it the values
                    ic.SetValues(all);

                    // Begin pairing
                    ic.StartPairing();

                    updated = true;
                }
            }

            // If no updates were made
            if (!updated)
            {
                // For each controller
                foreach (IController ic in controllersInOrder)
                {
                    // Stop the updating
                    ic.EndPairing();
                }
                // Done connecting all
                return true;
            }
            // Not done connecting all
            return false;
        }

        public static List<ToDraw> GetDrawing(Game game, IController[] controlsInOrder, Texture2D controls_handler_gui, Texture2D[] controls)
        {
            List<ToDraw> td = new List<ToDraw>();

            // Find middle of window
            Point pos = new Point(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            // Snap to half of controls gui
            pos -= new Point(controls_handler_gui.Width / 2, controls_handler_gui.Height / 2);
            // Store size
            Point size = new Point(controls_handler_gui.Width, controls_handler_gui.Height);

            // Add basic bounding box
            td.Add(new ToDraw(controls_handler_gui, new Rectangle(pos, size), Color.White));

            // For each controller
            int controlNum = controlsInOrder.Length;
            // Width of column for each
            int colWidth = controls_handler_gui.Width / controlNum;
            for (int i = 0; i < controlNum; i++)
            {
                Point middleCol = new Point((int)(colWidth * (i + 0.5f)), 25);

                int controlIndex = 0;
                foreach (StringTypePair<object> stp in controlsInOrder[i].GetValues())
                {
                    // Add given symbol to 
                    Texture2D tex = controls[controlIndex];
                    pos = middleCol + new Point(-tex.Width / 2, (tex.Height + 15) * controlIndex);
                    size = new Point(tex.Width, tex.Height);

                    Color color = Color.DarkGray;
                    // Brighten if paired
                    if (controlIndex < controlsInOrder[i].CurrPairing())
                    {
                        color = Color.White;
                    }

                    td.Add(new ToDraw(tex, new Rectangle(pos, size), color));

                    controlIndex++;
                }
            }

            return td;
        }

        // Find anything that is pressing Exit (Back, or Escape)
        public static bool IsExit()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return true;
            }
            else
            {
                // For each PlayerIndex
                foreach (PlayerIndex pi in Enum.GetValues(typeof(PlayerIndex)))
                {
                    // If pressing back
                    if (GamePad.GetState(pi).IsButtonDown(Buttons.Back))
                    {
                        return true;
                    }
                }

                // Nothing is pressing back
                return false;
            }
        }
    }

    public class StringTypePair<T>
    {
        public readonly string name;
        public readonly T value;

        public StringTypePair(string n, T v)
        {
            name = n;
            value = v;
        }
    }

    public class ToDraw
    {
        public Texture2D tex;
        public Rectangle bound;
        public Color color;

        public ToDraw(Texture2D t, Rectangle b, Color c)
        {
            tex = t;
            bound = b;
            color = c;
        }
    }
}
