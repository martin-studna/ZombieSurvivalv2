using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  public class Bomb
  {
    public bool Hit { get; set; }
    public int Ticks { get; set; }
    private int Radius { get; set; }
    public CircleShape Shape { get; set; }
    public Vector2f CurrentVelocity { get; set; }
    public float MaxVelocity { get; set; }
    public Stopwatch Stopwatch { get; set; }

    public Bomb()
    {
      Stopwatch = new Stopwatch();
      Hit = false;
      Ticks = 0;
      Radius = 7;
      CurrentVelocity = new Vector2f(0, 0);
      Shape = new CircleShape(Radius);
      MaxVelocity = 10;
      Shape.FillColor = Color.Yellow;
    }

    public void Update()
    {
      Ticks++;

      if (Ticks >= 30)
        Shape.Radius++;

      SwitchColor();
      if (Ticks <= 30)
        Shape.Position += CurrentVelocity;
    }

    public void Draw(RenderWindow window)
    {
      window.Draw(Shape);
    }

    private void SwitchColor()
    {
      switch (GameWorld.Random.Next(3))
      {
        case 0:
          Shape.FillColor = Color.Green;
          break;
        case 1:
          Shape.FillColor = Color.Yellow;
          break;
        case 2:
          Shape.FillColor = Color.Red;
          break;
      }
    }
  }
}
