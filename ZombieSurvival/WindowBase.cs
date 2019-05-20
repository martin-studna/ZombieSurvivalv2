using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace ZombieSurvival
{
    public class WindowBase
    {
        public static bool Pressed { get; set; }
        private static bool ButtonPressed(RectangleShape button, InputState inputState)
        {
            if (button.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
            {
                button.FillColor = Color.Red;
                Pressed = true;
                return true;
            }
            return false;
        }

        private static void ChangeButtonColor(RectangleShape button, InputState inputState)
        {
            if (button.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
                button.FillColor = Color.Green;
            else
                button.FillColor = Color.Magenta;
        }
    }
}
