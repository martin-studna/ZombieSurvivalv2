using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
    /// <summary>
    /// The Menu class represents window with menu.
    /// </summary>
    public static class Menu
    {
        public static RectangleShape Start { get; set; }
        public static RectangleShape Info { get; set; }
        public static RectangleShape Score { get; set; }
        public static RectangleShape Exit { get; set; }
        public static Text StartText { get; set; }
        public static Text ExitText { get; set; }
        public static Text InfoText { get; set; }
        public static Text ScoreText { get; set; }
        public static RectangleShape Background { get; set; }
        public static bool Pressed { get; set; }

        static Menu()
        {
            Start = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
            Info = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
            Score = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
            Exit = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
            StartText = new Text("Start", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
            InfoText = new Text("Info", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
            ScoreText = new Text("Score", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
            ExitText = new Text("Exit", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
        }

        public static void Update(float deltaTime, InputState inputState, RenderWindow window, ref Mode mode)
        {
            if (ButtonPressed(window, inputState, ref mode))
                return;

            ChangeButtonColor(inputState);
        }

        private static void InitGameWorld(RenderWindow window)
        {
            GameWorld.MapSize = new Vector2f(800, 800);
            GameWorld.Player1 = new Player();
            GameWorld.Watch.Start();
            GameWorld.EnemyStopwatch.Start();
            GameWorld.Background = new RectangleShape((Vector2f)window.Size)
            {
                Texture = new Texture("../../../Data/grass.png") { Repeated = true },
                Position = new Vector2f(0, 0),
                TextureRect = new IntRect(0, 0, 500, 500)
            };
        }

        /// <summary>
        /// The ButtonPressed method sets color of the button to red,
        /// if the cursor position is at the position of button and is clicked.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="inputState"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static bool ButtonPressed(RenderWindow window, InputState inputState, ref Mode mode)
        {
            if (Start.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
            {
                Start.FillColor = Color.Red;
                mode = Mode.SetName;
                InitGameWorld(window);
                Pressed = true;
                return true;
            }

            if (Info.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
            {
                Info.FillColor = Color.Red;
                mode = Mode.Info;
                Pressed = true;
                return true;
            }

            if (Score.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
            {
                Score.FillColor = Color.Red;
                mode = Mode.Score;
                ZombieSurvival.Score.LoadResults(window);
                Pressed = true;
                return true;
            }

            if (Exit.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
            {
                Exit.FillColor = Color.Red;
                mode = Mode.Closing;
                Pressed = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// The ChangeButtonColor method sets a color of the button to green, if the cursor position is at the position of the button.
        /// </summary>
        /// <param name="inputState"></param>
        private static void ChangeButtonColor(InputState inputState)
        {
            if (Start.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
            {
                Start.FillColor = Color.Green;
            }
            else if (Info.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
            {
                Info.FillColor = Color.Green;
            }
            else if (Score.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
            {
                Score.FillColor = Color.Green;
            }
            else if (Exit.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
            {
                Exit.FillColor = Color.Green;
            }
            else
            {
                Start.FillColor = Color.White;
                Info.FillColor = Color.White;
                Score.FillColor = Color.White;
                Exit.FillColor = Color.White;
            }
        }

        /// <summary>
        /// The Draw method draws all buttons of the Menu window.
        /// </summary>
        /// <param name="window"></param>
        public static void Draw(RenderWindow window)
        {
            window.Draw(Background);
            window.Draw(Start);
            window.Draw(Info);
            window.Draw(Score);
            window.Draw(Exit);
            window.Draw(StartText);
            window.Draw(InfoText);
            window.Draw(ScoreText);
            window.Draw(ExitText);
        }
    }
}
