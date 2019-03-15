using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ZombieSurvival
{
  public class Player: Entity
  {
    private Sound ShotSound { get; set; }
    private Sound BombSound { get; set; }
    private Sound LaserSound { get; set; }
    public Text HealthText { get; set; }
    public Text GunText { get; set; }
    public Text GunReloadText { get; set; }
    public Text ScoreText { get; set; }
    public int Score { get; set; }
    public int GunType { get; set; }
    public Vector2f AimDirection { get; set; }
    public Vector2f AimDirectionNormal { get; set; }
    public float Rotation { get; set; }
    public bool LaserFired { get; set; }
    public float BulletReload { get; set; }
    public float BombReload { get; set; }
    public float LaserReload { get; set; }
    public int BulletCounter { get; set; }
    public int BombCounter { get; set; }
    public List<Bullet> Bullets { get; set; }
    public List<Laser> Lasers { get; set; }
    public List<Bomb> Bombs { get; set; }
    public Vector2f Center { get; set; }

    public Player()
    {
      HealthText = new Text("Health: " + Health, GameWorld.Font, 24)
      {
        Position = new Vector2f(GameWorld.MapSize.X - 160, 10),
        Color = Color.White
      };

      GunText = new Text("Gun: " + GunType, GameWorld.Font, 24)
      {
        Position = new Vector2f(GameWorld.MapSize.X - 160, 40),
        Color = Color.White
      };

      GunReloadText = new Text("Reload: " + BulletReload, GameWorld.Font, 24)
      {
        Position = new Vector2f(GameWorld.MapSize.X - 160, 70),
        Color = Color.White
      };

      ScoreText = new Text("Score: " + Score, GameWorld.Font, 24)
      {
        Position = new Vector2f(GameWorld.MapSize.X - 160, 100),
        Color = Color.White
      };

      Bullets = new List<Bullet>();
      Lasers = new List<Laser>();
      Bombs = new List<Bomb>();

      ShotSound = new Sound(new SoundBuffer("../../../Data/gunshot.wav"));
      HurtSound = new Sound(new SoundBuffer("../../../Data/hurt.wav"));
      LaserSound = new Sound(new SoundBuffer("../../../Data/laser.wav"));
      BombSound = new Sound(new SoundBuffer("../../../Data/bomb.wav"));

      Position = new Vector2f(200, 200);
      Sprite = new Sprite { Position = Position, Texture = new Texture($"../../../Data/player.png") };

      Health = 100;

      MovementSpeed = 0.5f;
    }

    private void SetGunText()
    {
      switch (GunType)
      {
        case 0:
          GunText.DisplayedString = "Gun: Pistol";
          break;
        case 1:
          GunText.DisplayedString = "Gun: Laser";
          break;
        case 2:
          GunText.DisplayedString = "Gun: Bomb";
          break;
      }
    }

    private void SetReloadText()
    {
      switch (GunType)
      {
        case 0:
          double reload = (BulletReload / 200) * 100;
          GunReloadText.DisplayedString = "Reload: " + (int)reload + "%";
          break;
        case 1:
          double reload2 = (LaserReload / 100) * 100;
          GunReloadText.DisplayedString = "Reload: " + (int)reload2 + "%";
          break;
        case 2:
          double reload3 = (BombReload / 250) * 100;
          GunReloadText.DisplayedString = "Reload: " + (int)reload3 + "%";
          break;
      }
    }

    private void Move(float deltaTime, InputState inputState)
    {
      if (Dead)
        return;

      if (inputState.IsKeyPressed[(int)Keyboard.Key.A])
        Position.X -= deltaTime * MovementSpeed;

      if (inputState.IsKeyPressed[(int)Keyboard.Key.D])
        Position.X += deltaTime * MovementSpeed;

      if (inputState.IsKeyPressed[(int)Keyboard.Key.W])
        Position.Y -= deltaTime * MovementSpeed;

      if (inputState.IsKeyPressed[(int)Keyboard.Key.S])
        Position.Y += deltaTime * MovementSpeed;

      if (Position.X > 0 && Position.X < GameWorld.MapSize.X && Position.Y > 0 && Position.Y < GameWorld.MapSize.Y)
        Sprite.Position = Position;
    }

    private void BulletShoot()
    {
      ShotSound.Play();
      BulletCounter++;
      var bullet = new Bullet { Shape = { Position = Center } };
      bullet.CurrentVelocity = AimDirectionNormal * bullet.MaxVelocity;
      Bullets.Add(bullet);
    }

    private void LaserShoot()
    {
      LaserSound.Play();
      LaserFired = true;
      var laser = new Laser
      {
        Shape =
        {
          Position = Center,
          Rotation = Sprite.Rotation
        }
      };
      Lasers.Add(laser);
    }

    private void BombShoot()
    {
      BombSound.Play();
      BombCounter++;
      var bomb = new Bomb { Shape = { Position = Center } };
      bomb.CurrentVelocity = AimDirectionNormal * bomb.MaxVelocity;
      Bombs.Add(bomb);
    }

    private void SetDirection(Vector2f mousePosWindow)
    {
      float dx = Center.X - mousePosWindow.X;
      float dy = Center.Y - mousePosWindow.Y;
      Rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
      Sprite.Rotation = Rotation + 180;

      AimDirection = mousePosWindow - Center;

      AimDirectionNormal = AimDirection / (float)Math.Sqrt(Math.Pow(AimDirection.X, 2) + Math.Pow(AimDirection.Y, 2));
    }

    private void Shoot(InputState inputState)
    {
      if (inputState.IsLmbPressed)
      {
        if (GunType == 0 && BulletCounter <= 200)
          BulletShoot();
        if (GunType == 1 && !LaserFired)
          LaserShoot();
        if (GunType == 2 && BombCounter <= 200)
          BombShoot();
      }
    }

    private void Reload()
    {
      if (BulletCounter >= 200)
      {
        BulletReload++;
        if (BulletReload >= 200)
        {
          BulletCounter = 0;
          BulletReload = 0;
        }
      }
      if (BombCounter >= 100)
      {
        BombReload++;
        if (BombReload >= 250)
        {
          BombCounter = 0;
          BombReload = 0;
        }
      }
      if (LaserFired)
      {
        LaserReload++;
        if (LaserReload >= 100)
        {
          LaserReload = 0;
          LaserFired = false;
        }
      }
    }

    public void Update(float deltaTime, InputState inputState)
    {
      float num1 = Math.Min(Sprite.GetGlobalBounds().Left, Sprite.GetGlobalBounds().Left + Sprite.GetGlobalBounds().Width);
      float num2 = Math.Max(Sprite.GetGlobalBounds().Left, Sprite.GetGlobalBounds().Left + Sprite.GetGlobalBounds().Width);
      float num3 = Math.Min(Sprite.GetGlobalBounds().Top, Sprite.GetGlobalBounds().Top + Sprite.GetGlobalBounds().Height);
      float num4 = Math.Max(Sprite.GetGlobalBounds().Top, Sprite.GetGlobalBounds().Top + Sprite.GetGlobalBounds().Height);

      if (Health <= 0)
      {
        Dead = true;
      }

      Center = new Vector2f((num1 + num2) / 2, (num3 + num4) / 2);

      SetGunText();
      SetReloadText();
      ScoreText.DisplayedString = "Score: " + Score;
      HealthText.DisplayedString = "Health: " + Health;
      SetDirection(inputState.MousePosition);
      Move(deltaTime, inputState);
      Shoot(inputState);
      Reload();
    }

    public void Draw(RenderWindow window)
    {
      window.Draw(Sprite);
      window.Draw(GunText);
      window.Draw(GunReloadText);
      window.Draw(HealthText);
      window.Draw(ScoreText);
    }
  }
}
