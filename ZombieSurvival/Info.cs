using System.IO;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  /// <summary>
  /// The Info class represents window with summary informations about the game.
  /// </summary>
  public static class Info
  {
    public static RectangleShape Exit { get; set; }
    public static Text ExitText { get; set; }
    public static bool Pressed { get; set; }
    public static string Content { get; set; }
    public static Text InfoText { get; set; }
    static Info()
    {
      Exit = new RectangleShape(new Vector2f(300, 50)) { FillColor = Color.White };
      ExitText = new Text("Menu", new Font("../../../Data/freesans.ttf"), 24) { Color = Color.Black };
      var streamReader = new StreamReader("../../../Data/info");
      Content = streamReader.ReadToEnd();
      InfoText = new Text(Content, new Font("../../../Data/freesans.ttf"), 24) { Color = Color.White };
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

    private static void ChangeButtonColor(InputState inputState)
    {
      Exit.FillColor = Exit.GetGlobalBounds().Contains(inputState.MousePosition.X, inputState.MousePosition.Y) ? Color.Green : Color.White;
    }

    public static void Draw(RenderWindow window)
    {
      window.Draw(InfoText);
      window.Draw(Exit);
      window.Draw(ExitText);
    }
  }
}
