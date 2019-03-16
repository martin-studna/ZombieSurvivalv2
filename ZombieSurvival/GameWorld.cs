using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
  /// <summary>
  /// The GameWorld class represents state of the game. It holds all data about the game environment like
  /// player's health, properties of enemies and etc.
  /// Properties of the world are updated with the Update method.
  /// The Draw method draws all the objects of the world to the window, which is passed as the parameter. 
  /// </summary>
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
      Parallel.Invoke(
      () => { Player1.Update(deltaTime, inputState); },
      SpawnEnemy,
      () => { UpdateProjectiles(window); },
      UpdateEnemies);
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

    /// <summary>
    /// The UpdateProjectiles method updates all types of projectiles and checks collissions
    /// with window borders and with enemies.
    /// </summary>
    /// <param name="window"></param>
    private static void UpdateProjectiles(RenderWindow window)
    {
      lock (window)
      {
        for (int i = 0; i < Player1.Bullets.Count; i++)
        {
          if (Player1.Bullets[i] == null)
            break;

          Player1.Bullets[i].Update();

          if (Player1.Bullets[i].Shape.Position.X > window.Size.X
              || Player1.Bullets[i].Shape.Position.X < 0
              || Player1.Bullets[i].Shape.Position.Y < 0
              || Player1.Bullets[i].Shape.Position.Y > window.Size.Y)
          {
            Player1.Bullets.RemoveAt(i);
            break;
          }
        }
      }

      lock (Player1)
      {
        for (int i = 0; i < Player1.Lasers.Count; i++)
        {
          if (Player1.Lasers[i] == null)
            break;

          Player1.Lasers[i].Update();
          if (Player1.Lasers[i].Alpha <= 0)
          {
            Player1.Lasers.RemoveAt(i);
            break;
          }
        }
      }

      lock (Watch)
      {
        for (int i = 0; i < Player1.Bombs.Count; i++)
        {
          Player1.Bombs[i].Update();

          if (Player1.Bombs[i].Ticks > 50)
          {
            Player1.Bombs.RemoveAt(i);
            break;
          }

          if (Player1.Bombs[i].Shape.Position.X > window.Size.X
              || Player1.Bombs[i].Shape.Position.X < 0
              || Player1.Bombs[i].Shape.Position.Y < 0
              || Player1.Bombs[i].Shape.Position.Y > window.Size.Y)
          {
            Player1.Bombs.RemoveAt(i);
            break;
          }
        }
      }
    }

    /// <summary>
    /// The UpdateEnemies method updates positions of enemies and checks collissions with player.
    /// </summary>
    private static void UpdateEnemies()
    {
      lock (Player1)
      {
        for (int i = 0; i < Enemies.Count; i++)
        {
          Enemies[i].Update(Player1);

          if (Collision(Enemies[i]))
            Enemies[i].HurtSound.Play();

          if (Enemies[i].Dead)
          {
            Enemies.RemoveAt(i);
            break;
          }
        }
      }
    }

    private static bool Collision(Enemy enemy)
    {
      lock (enemy)
      {
        for (int i = 0; i < Player1.Bullets.Count; i++)
        {
          if (Player1.Bullets[i] != null && enemy.Sprite.GetGlobalBounds().Intersects(Player1.Bullets[i].Shape.GetGlobalBounds()))
          {
            Parallel.Invoke(() =>
            {
              enemy.Health -= 10;
            },

            () =>
            {
              Player1.Bullets.RemoveAt(i);
            }
            );
            return true;
          }
        }
      }

      lock (Watch)
      {
        for (int i = 0; i < Player1.Lasers.Count; i++)
        {
          if (enemy.Sprite.GetGlobalBounds().Intersects(Player1.Lasers[i].Shape.GetGlobalBounds()))
          {
            enemy.Health -= 40;
            return true;
          }
        }
      }

      lock (Player1)
      {
        for (int i = 0; i < Player1.Bombs.Count; i++)
        {
          if (Player1.Bombs[i].Hit)
          {
            Player1.Bombs.RemoveAt(i);
            break;
          }
          if (enemy.Sprite.GetGlobalBounds().Intersects(Player1.Bombs[i].Shape.GetGlobalBounds()))
          {
            enemy.Health -= 20;
            Player1.Bombs[i].Hit = true;
            Player1.Bombs[i].Shape.Radius = 30;
            return true;
          }
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