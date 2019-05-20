using System;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace ZombieSurvival
{
    /// <summary>
    /// The Enemy class represents all types of enemies.
    /// The class holds basic properties like Health, Movement direction, Attack and etc.
    /// </summary>
    public class Enemy: Entity
    {
        public bool Collision { get; set; }
        public Vector2f DirectionNormal { get; set; }
        public int Attack { get; set; }
        public int EnemyType { get; set; }

        public Enemy()
        {
            var soundBuffer = new SoundBuffer("../../../../Data/zombie-hurt.wav");
            HurtSound = new Sound(soundBuffer);
            SetInitialPosition();
            Sprite = new Sprite(new Texture("../../../../Data/zombie.png")) { Position = Position };
            SetType();
            Collision = false;
        }

        /// <summary>
        /// The GiveScore method gives score to the player based on his type.
        /// </summary>
        private void GiveScore()
        {
            switch (EnemyType)
            {
                case 0:
                    GameWorld.Player1.Score += 10;
                    break;
                case 1:
                    GameWorld.Player1.Score += 20;
                    break;
                case 2:
                    GameWorld.Player1.Score += 30;
                    break;
            }
        }

        /// <summary>
        /// Enemy can come from one of the four sides of the window
        /// </summary>
        private void SetInitialPosition()
        {
            switch (GameWorld.Random.Next(4))
            {
                case 0:
                    Position = new Vector2f(0, (float)GameWorld.Random.NextDouble() * 750);
                    break;
                case 1:
                    Position = new Vector2f((float)GameWorld.Random.NextDouble() * 1000, 0);
                    break;
                case 2:
                    Position = new Vector2f((float)GameWorld.Random.NextDouble() * 1000, 750);
                    break;
                case 3:
                    Position = new Vector2f(1000, (float)GameWorld.Random.NextDouble() * 1000);
                    break;
            }
        }

        /// <summary>
        /// The SetType method sets type of the enemy.
        /// Red one = boss
        /// Yellow one = sprinter
        /// Without color = minion
        /// </summary>
        private void SetType()
        {
            EnemyType = GameWorld.Random.Next(3);

            switch (EnemyType)
            {
                case 0:
                    Attack = 5;
                    MovementSpeed = 8;
                    Sprite.Color = Color.Yellow;
                    Health = 50;
                    break;
                case 1:
                    Attack = 10;
                    MovementSpeed = 5;
                    Health = 100;
                    break;
                case 2:
                    Attack = 20;
                    Sprite.Color = Color.Red;
                    MovementSpeed = 3;
                    Health = 200;
                    break;
            }
        }

        public bool Collides(Player player)
        {
            if (Sprite.GetGlobalBounds().Intersects(player.Sprite.GetGlobalBounds()))
                return true;

            return false;
        }

        public void Update(Player player)
        {
            var direction = player.Position - Position;
            DirectionNormal = direction / (float)Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Y, 2));


            if (Health <= 0)
            {
                GiveScore();
                Dead = true;
            }

            if (!Collides(player))
            {
                Position += DirectionNormal * MovementSpeed;
                Sprite.Position = Position;
            }
            else
            {
                Dead = true;
                player.HurtSound.Play();
                player.Health -= Attack;
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(Sprite);
        }
    }
}
