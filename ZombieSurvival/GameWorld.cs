using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  public static class GameWorld
  {
    public static Vector2f MapSize { get; set; }
    public static RectangleShape Background { get; set; }
    public static Music BackgroundMusic { get; set; }
    public static Player Player1 { get; set; }
    public static List<Enemy> Enemies { get; set; }
    public static Font Font { get; set; }
    public static Stopwatch Watch { get; set; }
    public static Stopwatch EnemyStopwatch { get; set; }
    public static Random Random { get; set; }

    static GameWorld()
    {
      Background = new RectangleShape();
      Watch = new Stopwatch();
      Enemies = new List<Enemy>();
      EnemyStopwatch = new Stopwatch();
      Random = new Random();
      Font = new Font("../../../Data/freesans.ttf");
    }

    public static void Update(float deltaTime, InputState inputState, RenderWindow window)
    {
      Player1.Update(deltaTime, inputState);
      SpawnEnemy();
      UpdateProjectiles(window);
      UpdateEnemies();
    }

    private static void SpawnEnemy()
    {
      if (EnemyStopwatch.ElapsedMilliseconds > 500)
      {
        Enemies.Add(new Enemy());
        EnemyStopwatch.Reset();
        EnemyStopwatch.Start();
      }
    }

    private static void UpdateProjectiles(RenderWindow window)
    {
      foreach (var bullet in Player1.Bullets)
      {
        bullet.Update();

        if (bullet.Shape.Position.X > window.Size.X || bullet.Shape.Position.X < 0 || bullet.Shape.Position.Y < 0 || bullet.Shape.Position.Y > window.Size.Y)
        {
          Player1.Bullets.Remove(bullet);
          break;
        }
      }

      foreach (var laser in Player1.Lasers)
      {
        laser.Update();
        if (laser.Alpha <= 0)
        {
          Player1.Lasers.Remove(laser);
          break;
        }
      }

      foreach (var bomb in Player1.Bombs)
      {
        bomb.Update();

        if (bomb.Ticks > 50)
        {
          Player1.Bombs.Remove(bomb);
          break;
        }

        if (bomb.Shape.Position.X > window.Size.X || bomb.Shape.Position.X < 0
            || bomb.Shape.Position.Y < 0 || bomb.Shape.Position.Y > window.Size.Y)
        {
          Player1.Bombs.Remove(bomb);
          break;
        }
      }
    }

    private static void UpdateEnemies()
    {
      foreach (var enemy in Enemies)
      {
        enemy.Update(Player1);

        if (Collision(enemy))
          enemy.HurtSound.Play();

        if (enemy.Dead)
        {
          Enemies.Remove(enemy);
          break;
        }
      }
    }

    private static bool Collision(Enemy enemy)
    {
      foreach (var bullet in Player1.Bullets)
      {
        if (enemy.Sprite.GetGlobalBounds().Intersects(bullet.Shape.GetGlobalBounds()))
        {
          enemy.Health -= 10;
          Player1.Bullets.Remove(bullet);
          return true;
        }
      }

      foreach (var laser in Player1.Lasers)
      {
        if (enemy.Sprite.GetGlobalBounds().Intersects(laser.Shape.GetGlobalBounds()))
        {
          enemy.Health -= 40;
          return true;
        }
      }

      foreach (var bomb in Player1.Bombs)
      {
        if (bomb.Hit)
        {
            Player1.Bombs.Remove(bomb);
            break;
        }
        if (enemy.Sprite.GetGlobalBounds().Intersects(bomb.Shape.GetGlobalBounds()))
        {
          enemy.Health -= 20;
          bomb.Hit = true;
          bomb.Shape.Radius = 30;
          return true;
        }
      }

      return false;
    }

    public static void Draw(RenderWindow window)
    {
      window.Draw(Player1.GunReloadText);
      window.Draw(Background);
      Player1.Draw(window);
      DrawBullets(window);
      DrawEnemies(window);
      DrawLasers(window);
      DrawBombs(window);
    }

    private static void DrawEnemies(RenderWindow window)
    {
      foreach (var enemy in Enemies)
        enemy.Draw(window);
    }

    private static void DrawBullets(RenderWindow window)
    {
      foreach (var bullet in Player1.Bullets)
        bullet.Draw(window);
    }

    private static void DrawLasers(RenderWindow window)
    {
      foreach (var laser in Player1.Lasers)
        laser.Draw(window);
    }
    private static void DrawBombs(RenderWindow window)
    {
      foreach (var bomb in Player1.Bombs)
        bomb.Draw(window);
    }
  }
}