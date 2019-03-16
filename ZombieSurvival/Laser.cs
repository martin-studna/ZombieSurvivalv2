using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  public class Laser
  {
    public RectangleShape Shape { get; set; }
    public byte Alpha { get; set; }

    public Laser()
    {
      Alpha = 255;
      Shape = new RectangleShape(new Vector2f(1500, 7)) { FillColor = Color.Green };
    }

    public void Update()
    {
      Shape.FillColor = new Color(77, 204, 74, Alpha);
      Alpha -= 25;
    }

    public void Draw(RenderWindow window)
    {
      window.Draw(Shape);
    }
  }
}
