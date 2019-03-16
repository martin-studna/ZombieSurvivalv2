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
  public class Core
  {
    private InputState _inputState;
    private RenderWindow _window;
    private Mode mode = Mode.Menu;

    public void Run()
    {
      _inputState = new InputState();

      _window = new RenderWindow(new VideoMode(800, 800), "Zombie Survival", Styles.Close, new ContextSettings(24, 8, 2));
      _window.SetFramerateLimit(100);
      _window.SetVerticalSyncEnabled(true);
      _window.SetActive();

      // Setup event handlers
      _window.Closed += OnClosed;
      _window.KeyPressed += OnKeyPressed;
      _window.KeyReleased += OnKeyReleased;
      _window.MouseMoved += OnMouseMoved;
      _window.MouseButtonPressed += OnMouseButtonPressed;
      _window.MouseButtonReleased += OnMouseButtonReleased;
      _window.MouseWheelMoved += OnMouseWheel;

      if (!File.Exists("../../../../score"))
        File.Create("../../../../score");

      InitMenu();
      InitScore();
      InitInfo();

      float lastTime = 0f;

      while (_window.IsOpen)
      {
        float currentTime = GameWorld.Watch.ElapsedMilliseconds;
        float deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        _window.DispatchEvents();

        switch (mode)
        {
          case Mode.Closing:
            _window.Close();
            break;
          case Mode.Menu:
            Menu.Update(deltaTime, _inputState, _window, ref mode);
            _window.Clear(Color.Black);
            Menu.Draw(_window);
            break;
          case Mode.Score:
            Score.Update(deltaTime, _window, _inputState, ref mode);
            _window.Clear(Color.Black);
            Score.Draw(_window);
            break;
          case Mode.Info:
            Info.Update(deltaTime, _window, _inputState, ref mode);
            _window.Clear(Color.Black);
            Info.Draw(_window);
            break;
          case Mode.Game:
            GameWorld.Update(deltaTime, _inputState, _window);
            _window.Clear(Color.White);
            GameWorld.Draw(_window);
            if (GameWorld.Player1.Dead)
            {
              mode = Mode.Menu;
              WriteResult(GameWorld.Player1.Score);
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

        if (Menu.Pressed)
        {
          Thread.Sleep(150);
          Menu.Pressed = false;
        }
      }
    }

    private void WriteResult(int score)
    {
      var results = new List<string> { score.ToString() };

      var sr = new StreamReader("../../../../score");

      while (!sr.EndOfStream)
        results.Add(sr.ReadLine());

      sr.Close();

      results = results.OrderByDescending(x => int.Parse(x.Split(' ').Last())).Distinct().ToList();

      if (results.Count > 10)
      {
        results.RemoveAt(10);
      }

      var sw = new StreamWriter("../../../../score");

      foreach (var result in results)
      {
        sw.WriteLine(result);
      }

      sw.Close();

    }

    private void InitInfo()
    {
      Info.InfoText.Position = new Vector2f(50, 50);
      Info.Exit.Position = new Vector2f(50, _window.Size.Y - 100);
      Info.ExitText.Position = new Vector2f(Score.Exit.Position.X + Score.Exit.GetGlobalBounds().Width / 2 - 30, Score.Exit.Position.Y + Score.Exit.Size.Y / 4);
    }
    private void InitScore()
    {
      Score.Exit.Position = new Vector2f(50, _window.Size.Y - 100);
      Score.ExitText.Position = new Vector2f(Score.Exit.Position.X + Score.Exit.GetGlobalBounds().Width / 2 - 30, Score.Exit.Position.Y + Score.Exit.Size.Y / 4);
    }

    private void InitMenu()
    {
      Menu.Background = new RectangleShape((Vector2f)_window.Size)
      {
        Position = new Vector2f(0, 0),
        Texture = new Texture("../../../Data/menu.png")
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

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (mode == Mode.Game)
      {
        GameWorld.Player1.GunType = (GameWorld.Player1.GunType + e.Delta) % 3;

        if (GameWorld.Player1.GunType < 0)
          GameWorld.Player1.GunType = 2;
      }
    }

    private void OnClosed(object sender, EventArgs e)
    {
      _window.Close();
    }

    private void OnKeyPressed(object sender, KeyEventArgs e)
    {
      if (e.Code == Keyboard.Key.Escape)
        _window.Close();

      int keyCode = (int)e.Code;

      if (keyCode >= 0 && keyCode < (int)Keyboard.Key.KeyCount)
        _inputState.IsKeyPressed[keyCode] = true;
    }

    private void OnKeyReleased(object sender, KeyEventArgs e)
    {
      int keyCode = (int)e.Code;

      if (keyCode >= 0 && keyCode < (int)Keyboard.Key.KeyCount)
        _inputState.IsKeyPressed[keyCode] = false;
    }

    private void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
      _inputState.MousePosition = new Vector2f(e.X, e.Y);

      _inputState.MousePositionFromCenter = new Vector2f(_inputState.MousePosition.X - _window.Size.X / 2,
        _inputState.MousePosition.Y - _window.Size.Y / 2);
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
      if (e.Button == Mouse.Button.Left)
        _inputState.IsLmbPressed = true;
    }

    private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
      if (e.Button == Mouse.Button.Left)
        _inputState.IsLmbPressed = false;
    }
  }
}
