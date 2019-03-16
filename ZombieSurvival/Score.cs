using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  /// <summary>
  /// The Score class represents windows, where are written the highest scores
  /// </summary>
  public static class Score
  {
    public static List<Text> Results { get; set; }
    public static RectangleShape Exit { get; set; }
    public static Text ExitText { get; set; }
    public static bool Pressed { get; set; }

    static Score()
    {
      Exit = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
      ExitText = new Text("Menu", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
      Results = new List<Text>();
    }

    public static void Update(float deltaTime, RenderWindow window, InputState inputState, ref Mode mode)
    {
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

    /// <summary>
    /// The LoadResults method loads the highest scores of the game from score file in the Score window.
    /// </summary>
    /// <param name="window"></param>
    public static void LoadResults(RenderWindow window)
    {
      using (var sr = new StreamReader("../../../../score"))
      {
        int i = 1;
        while(!sr.EndOfStream)
        {
          string val = sr.ReadLine();
          Results.Add(new Text($"{i}. Player1: {val}", new Font("../../../Data/freesans.ttf")) { Position = new Vector2f(75, i * 50) });
          i++;
        }
      }
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
