using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ZombieSurvival
{
    /// <summary>
    /// The Core class initialize all important resources for the game execution.
    /// As we can see there are Update and Draw methods, which are constantly called in the while loop.
    /// Game contains several states or we can say modes like Menu, Game, Score, Info and Closing. Each time
    /// we click at one of those buttons in the Game menu, we change this mode and switch to a next window.
    /// </summary>
    /// <remarks>
    /// This class also process keyboard events and mouse events. 
    /// </remarks>
    public class Core
    {
        private InputState _inputState;
        private RenderWindow _window;
        private Mode _mode = Mode.Menu;

        /// <summary>
        /// <code>
        /// if (Info.Pressed)
        /// {
        ///   Thread.Sleep(150);
        ///   Info.Pressed = false;
        /// }
        /// </code>
        /// Everytime we press a button, we would like to change it's color, but also
        /// we would like to remain for some time in a current window, so we can see that the
        /// color of the button has changed. We can do this by making the thread sleep.
        /// </summary>
        public void Run()
        {
            _inputState = new InputState();

            _window = new RenderWindow(new VideoMode(800, 800), "Zombie Survival", Styles.Default, new ContextSettings(24, 8, 2));
            _window.SetFramerateLimit(100);
            _window.SetVerticalSyncEnabled(true);
            _window.SetActive(false);
            

            // Setup event handlers
            _window.Closed += OnClosed;
            _window.KeyPressed += OnKeyPressed;
            _window.KeyReleased += OnKeyReleased;
            _window.MouseMoved += OnMouseMoved;
            _window.MouseButtonPressed += OnMouseButtonPressed;
            _window.MouseButtonReleased += OnMouseButtonReleased;
            _window.MouseWheelMoved += OnMouseWheel;

            if (!File.Exists("../../../../score"))
                File.Create("../../../../score").Close();

            InitMenu();
            InitScore();
            InitInfo();
            InitSetName();

            float lastTime = 0f;

            while (_window.IsOpen)
            {
                float currentTime = GameWorld.Watch.ElapsedMilliseconds;
                float deltaTime = currentTime - lastTime;
                lastTime = currentTime;

                _window.DispatchEvents();

                switch (_mode)
                {
                    case Mode.Closing:
                        _window.Close();
                        break;
                    case Mode.SetName:
                        SetName.Update(deltaTime, _window, _inputState, ref _mode);
                        _window.Clear(Color.Black);
                        SetName.Draw(_window);
                        break;
                    case Mode.Menu:
                        Menu.Update(deltaTime, _inputState, _window, ref _mode);
                        _window.Clear(Color.Black);
                        Menu.Draw(_window);
                        break;
                    case Mode.Score:
                        Score.Update(deltaTime, _window, _inputState, ref _mode);
                        _window.Clear(Color.Black);
                        Score.Draw(_window);
                        break;
                    case Mode.Info:
                        Info.Update(deltaTime, _window, _inputState, ref _mode);
                        _window.Clear(Color.Black);
                        Info.Draw(_window);
                        break;
                    case Mode.Game:
                        GameWorld.Update(deltaTime, _inputState, _window);
                        _window.Clear(Color.White);
                        GameWorld.Draw(_window);
                        if (GameWorld.Player1.Dead)
                        {
                            _mode = Mode.Menu;
                            WriteResult(GameWorld.Player1.Name, GameWorld.Player1.Score);
                        }
                        break;
                }

                if (!_window.IsOpen)
                    break;

                _window.Display();

                if (Info.Pressed)
                {
                    Thread.Sleep(150);
                    Info.Pressed = false;
                }

                if (Score.Pressed)
                {
                    Thread.Sleep(150);
                    Score.Pressed = false;
                }

                if (SetName.Pressed)
                {
                    Thread.Sleep(150);
                    GameWorld.Player1.Name = SetName.InputText.DisplayedString;
                    SetName.Pressed = false;
                }

                if (Menu.Pressed)
                {
                    Thread.Sleep(150);
                    Menu.Pressed = false;
                }
            }
        }

        /// <summary>
        /// The WriteResult method stores highest scores in to the file. It also sorts the results in descending order.  
        /// </summary>
        /// <param name="score"></param>
        private void WriteResult(string name, int score)
        {
            Score.Results = new List<Text>();
            var results = new List<string> { name + ": " + score };

            using (var sr = new StreamReader("../../../../score"))
            {
                while (!sr.EndOfStream)
                {
                    string val = sr.ReadLine();
                    if (!string.IsNullOrEmpty(val))
                        results.Add(val);
                }
            }

            results = results.OrderByDescending(x => int.Parse(x.Length == 0 ? "0" : x.Split(' ').Last())).Distinct().ToList();

            if (results.Count > 10)
            {
                results.RemoveAt(10);
            }

            using (var sw = new StreamWriter("../../../../score"))
            {
                foreach (var result in results)
                {
                    sw.WriteLine(result);
                }
            }
        }

        private void InitSetName()
        {
            SetName.Ok.Position = new Vector2f(_window.Size.X / 2 - SetName.Ok.Size.X / 2, 500);
            SetName.OkText.Position = new Vector2f(SetName.Ok.Position.X + SetName.Ok.Size.X / 2 - SetName.OkText.GetGlobalBounds().Width / 2,
              SetName.Ok.Position.Y + SetName.Ok.Size.Y / 2 - SetName.OkText.GetGlobalBounds().Height / 2);
            SetName.TextBox.Position = new Vector2f(_window.Size.X / 2 - SetName.Ok.Size.X / 2, 500 - 75);
            SetName.InputText.Position = new Vector2f(SetName.TextBox.Position.X + 10, SetName.TextBox.Position.Y + 10);
        }

        /// <summary>
        /// The InitInfo method initialize all important properties of the Info class before it is used.
        /// </summary>
        private void InitInfo()
        {
            Info.InfoText.Position = new Vector2f(50, 50);
            Info.Exit.Position = new Vector2f(50, _window.Size.Y - 100);
            Info.ExitText.Position = new Vector2f(Score.Exit.Position.X + Score.Exit.GetGlobalBounds().Width / 2 - 30, Score.Exit.Position.Y + Score.Exit.Size.Y / 4);
        }

        /// <summary>
        /// The InitScore method initialize all important properties of the Score class before it is used.
        /// </summary>
        private void InitScore()
        {
            Score.Exit.Position = new Vector2f(50, _window.Size.Y - 100);
            Score.ExitText.Position = new Vector2f(Score.Exit.Position.X + Score.Exit.GetGlobalBounds().Width / 2 - 30, Score.Exit.Position.Y + Score.Exit.Size.Y / 4);
        }

        /// <summary>
        /// The InitMenu method initialize all important properties of the Menu class before it is used.
        /// </summary>
        private void InitMenu()
        {
            Menu.Background = new RectangleShape((Vector2f)_window.Size)
            {
                Position = new Vector2f(0, 0),
                Texture = new Texture("../../../../Data/menu.png")
            };

            Menu.Start.Position = new Vector2f(_window.Size.X / 2 - Menu.Start.Size.X / 2, _window.Size.Y - (_window.Size.Y - 200));
            Menu.Info.Position = new Vector2f(_window.Size.X / 2 - Menu.Start.Size.X / 2, _window.Size.Y - (_window.Size.Y - 300));
            Menu.Score.Position = new Vector2f(_window.Size.X / 2 - Menu.Start.Size.X / 2, _window.Size.Y - (_window.Size.Y - 400));
            Menu.Exit.Position = new Vector2f(_window.Size.X / 2 - Menu.Start.Size.X / 2, _window.Size.Y - (_window.Size.Y - 500));

            Menu.StartText.Position = new Vector2f(_window.Size.X / 2 - Menu.StartText.GetGlobalBounds().Width / 2, _window.Size.Y - (_window.Size.Y - 200) + (Menu.Start.Size.Y / 4));
            Menu.InfoText.Position = new Vector2f(_window.Size.X / 2 - Menu.InfoText.GetGlobalBounds().Width / 2, _window.Size.Y - (_window.Size.Y - 300) + (Menu.Info.Size.Y / 4));
            Menu.ScoreText.Position = new Vector2f(_window.Size.X / 2 - Menu.ScoreText.GetGlobalBounds().Width / 2, _window.Size.Y - (_window.Size.Y - 400) + (Menu.Score.Size.Y / 4));
            Menu.ExitText.Position = new Vector2f(_window.Size.X / 2 - Menu.ExitText.GetGlobalBounds().Width / 2, _window.Size.Y - (_window.Size.Y - 500) + (Menu.Exit.Size.Y / 4));
        }

        /// <summary>
        /// The OnMouseWheel method processes mouse wheel events.
        /// With this method we can choose a gun with which we want to shoot enemies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_mode == Mode.Game)
            {
                GameWorld.Player1.GunType = (GameWorld.Player1.GunType + e.Delta) % 3;

                if (GameWorld.Player1.GunType < 0)
                    GameWorld.Player1.GunType = 2;
            }
        }

        /// <summary>
        /// The OnClosed method closes current window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, EventArgs e)
        {
            _window.Close();
        }

        /// <summary>
        /// The OnKeyPressed processes all keyboard pressed events and store them in the InputState class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                _window.Close();

            int keyCode = (int)e.Code;

            if (keyCode >= 0 && keyCode < (int)Keyboard.Key.KeyCount)
                _inputState.IsKeyPressed[keyCode] = true;

            if (_mode == Mode.SetName)
            {
                if (e.Code != Keyboard.Key.BackSpace)
                    SetName.InputText.DisplayedString += e.Code.ToString().ToLower();
                else
                    SetName.InputText.DisplayedString = SetName.InputText.DisplayedString.Substring(0, SetName.InputText.DisplayedString.Length - 1);
            }
        }

        /// <summary>
        /// The OnKeyPressed processes all keyboard released events and store them in the InputState class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            int keyCode = (int)e.Code;

            if (keyCode >= 0 && keyCode < (int)Keyboard.Key.KeyCount)
                _inputState.IsKeyPressed[keyCode] = false;
        }

        /// <summary>
        /// The OnMouseMoved method stores the current mouse position in the InputState class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            _inputState.MousePosition = new Vector2f(e.X, e.Y);

            _inputState.MousePositionFromCenter = new Vector2f(_inputState.MousePosition.X - _window.Size.X / 2,
              _inputState.MousePosition.Y - _window.Size.Y / 2);
        }

        /// <summary>
        /// The OnMouseButtonPressed method stores mouse button pressed events in the InputState class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
                _inputState.IsLmbPressed = true;
        }

        /// <summary>
        /// The OnMouseButtonReleased method stores mouse button released events in the InputState class. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
                _inputState.IsLmbPressed = false;
        }
    }
}
