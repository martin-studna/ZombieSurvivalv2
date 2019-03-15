using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  public class Entity
  {
    public Sprite Sprite { get; set; }
    public bool Dead { get; set; }
    public Sound HurtSound { get; set; }
    public float Health { get; set; }
    public float MovementSpeed { get; set; }
    public Vector2f Position;
  }
}
