using SFML.System;
using SFML.Window;

namespace ZombieSurvival
{
  public class InputState
  {
    public Vector2f MousePosition { get; set; }
    public Vector2f MousePositionFromCenter { get; set; }
    public bool[] IsKeyPressed { get; set; }
    public bool IsLmbPressed { get; set; }

    public InputState()
    {
      IsKeyPressed = new bool[(int)Keyboard.Key.KeyCount];
    }
  }
}
