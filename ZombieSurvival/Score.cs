using System.IO;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  public static class Score
  {
    public static Text[] Results { get; set; }
    public static RectangleShape Exit { get; set; }
    public static Text ExitText { get; set; }
    public static bool Pressed { get; set; }

    static Score()
    {
      Exit = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
      ExitText = new Text("Menu", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
      Results = new Text[5];
    }

    public static void Update(float deltaTime, RenderWindow window, InputState inputState, ref Mode mode)
    {
      using (var sr = new StreamReader("../../../Data/score"))
      {
        for (int i = 0; i < 5; i++)
        {
          if (sr.EndOfStream)
            break;

          string val = sr.ReadLine();
          Results[i] = new Text($"{(i + 1)}. Score: {val}", new Font("../../../Data/freesans.ttf")) { Position = new Vector2f(window.Size.X / 2, (i + 1) * 100) };
        }
      }
      if (ButtonPressed(window, inputState, ref mode))
        return;

      ChangeButtonColor(inputState);
    }

    private static bool ButtonPressed(RenderWindow window, InputState inputState, ref Mode mode)
    {
      if (Exit.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) && inputState.IsLmbPressed)
      {
        Exit.FillColor = Color.Red;
        mode = Mode.Menu;
        Pressed = true;
        return true;
      }
      return false;
    }

    private static void ChangeButtonColor(InputState inputState)
    {
      if (Exit.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y))
      {
        Exit.FillColor = Color.Green;
      }
      else
      {
        Exit.FillColor = Color.White;
      }
    }

    public static void Draw(RenderWindow window)
    {
      window.Draw(Exit);
      window.Draw(ExitText);

      foreach (var result in Results)
      {
        if (result == null)
          break;
        window.Draw(result);
      }
    }
  }
}
