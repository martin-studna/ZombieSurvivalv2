using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
    /// <summary>
    /// The class SetName represents window for setting the name of the player
    /// </summary>
    public static class SetName
    {
        public static RectangleShape Ok { get; set; }
        public static RectangleShape TextBox { get; set; }
        public static Text InputText { get; set; }
        public static Text OkText { get; set; }
        public static bool Pressed { get; set; }

        static SetName()
        {
            TextBox = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
            InputText = new Text("", new Font("../../../../Data/freesans.ttf"), 24) { Color = Color.Black };
            Ok = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.Magenta };
            OkText = new Text("Ok", new Font("../../../../Data/freesans.ttf"), 24) { Color = Color.Black };
        }

        public static void Update(float deltaTime, RenderWindow window, InputState inputState, ref Mode mode)
        {
            if (ButtonPressed(window, inputState, ref mode))
                return;

            ChangeButtonColor(inputState);
        }

        private static bool ButtonPressed(RenderWindow window, InputState inputState, ref Mode mode)
        {
            if (Ok.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
            {
                Ok.FillColor = Color.Red;
                mode = Mode.Game;
                Pressed = true;
                return true;
            }
            return false;
        }

        private static void ChangeButtonColor(InputState inputState)
        {
            if (Ok.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
            {
                Ok.FillColor = Color.Green;
            }
            else
            {
                Ok.FillColor = Color.Magenta;
            }
        }

        public static void Draw(RenderWindow window)
        {
            window.Draw(Ok);
            window.Draw(OkText);
            window.Draw(TextBox);
            window.Draw(InputText);
        }
    }
}
