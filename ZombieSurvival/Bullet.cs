using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  public class Bullet
  {
    public CircleShape Shape { get; set; }
    public Vector2f CurrentVelocity { get; set; }
    public int MaxVelocity { get; set; }
    public Bullet()
    {
      Shape = new CircleShape(5) { FillColor = Color.Green };
      CurrentVelocity = new Vector2f(0, 0);
      MaxVelocity = 15;
    }

    public void Update()
    {
      Shape.Position += CurrentVelocity;
    }

    public void Draw(RenderWindow window)
    {
      window.Draw(Shape);
    }
  }
}
