using SFML.System;
using SFML.Window;

namespace ZombieSurvival
{
  /// <summary>
  /// The InputState class holds all important keyboard and mouse events.
  /// This class holds array of booleans, which represents if the key was pressed or released.
  /// It also contains current mouse position.
  /// </summary>
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
