using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Font = new Font("../../../../Data/freesans.ttf");
        }

        public static void Update(float deltaTime, InputState inputState, RenderWindow window)
        {
            Parallel.Invoke(
            () => { Player1.Update(deltaTime, inputState); },
            () => { SpawnEnemy(); },
            () => { UpdateProjectiles(window); },
            () => { UpdateEnemies(); }
            );
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
            lock (Enemies)
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
                        Player1.Bullets.RemoveElem(i);
                        break;
                    }
                }
            }

            lock (Enemies)
            {
                for (int i = 0; i < Player1.Lasers.Count; i++)
                {
                    if (Player1.Lasers[i] == null)
                        break;

                    Player1.Lasers[i].Update();
                    if (Player1.Lasers[i].Alpha <= 10)
                    {
                        Player1.Lasers.RemoveElem(i);
                        break;
                    }
                }
            }

            lock (Enemies)
            {
                for (int i = 0; i < Player1.Bombs.Count; i++)
                {
                    Player1.Bombs[i].Update();

                    if (Player1.Bombs[i].Ticks > 50)
                    {
                        Player1.Bombs.RemoveElem(i);
                        break;
                    }

                    if (Player1.Bombs[i].Shape.Position.X > window.Size.X
                        || Player1.Bombs[i].Shape.Position.X < 0
                        || Player1.Bombs[i].Shape.Position.Y < 0
                        || Player1.Bombs[i].Shape.Position.Y > window.Size.Y)
                    {
                        Player1.Bombs.RemoveElem(i);
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
            lock (Enemies)
            {
                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemies[i].Update(Player1);

                    if (Collision(Enemies[i]))
                        Enemies[i].HurtSound.Play();

                    if (Enemies[i].Dead)
                    {
                        Enemies.RemoveElem(i);
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
                        enemy.Health -= 10;
                        Player1.Bullets.RemoveElem(i);

                        return true;
                    }
                }
            }

            lock (EnemyStopwatch)
            {
                for (int i = 0; i < Player1.Lasers.Count; i++)
                {
                    if (RectanglesOverlap(Player1.Lasers[i].Shape, enemy.Sprite))
                    {
                        enemy.Health -= 100;
                        return true;
                    }
                }
            }

            lock (Font)
            {
                for (int i = 0; i < Player1.Bombs.Count; i++)
                {
                    if (i >= Player1.Bombs.Count)
                        break;

                    if (Player1.Bombs[i].Hit)
                    {
                        Player1.Bombs.RemoveElem(i);
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

        private static void RemoveElem<T>(this List<T> list, int i)
        {
            if (list.Count != 0)
            {
                list[i] = list.Last();
                list.Remove(list.Last());
            }
        }

        private static bool RectanglesOverlap(RectangleShape shape, Sprite sprite)
        {
            FloatRect rect1Bounds = shape.GetLocalBounds();
            FloatRect rect2Bounds = (sprite.GetLocalBounds());
            Vector2f rect1Size = new Vector2f(rect1Bounds.Width, rect1Bounds.Height);
            Vector2f rect2Size = new Vector2f(rect2Bounds.Width, rect2Bounds.Height);
            Vector2f rect1TopLeft = sprite.InverseTransform.TransformPoint(shape.Transform.TransformPoint(0, 0));
            Vector2f rect1TopRight = sprite.InverseTransform.TransformPoint(shape.Transform.TransformPoint(rect1Size.X, 0));
            Vector2f rect1BottomRight = sprite.InverseTransform.TransformPoint(shape.Transform.TransformPoint(rect1Size));
            Vector2f rect1BottomLeft = sprite.InverseTransform.TransformPoint(shape.Transform.TransformPoint(0, rect1Size.Y));
            Vector2f rect2TopLeft = shape.InverseTransform.TransformPoint(sprite.Transform.TransformPoint(0, 0));
            Vector2f rect2TopRight = shape.InverseTransform.TransformPoint(sprite.Transform.TransformPoint(rect2Size.X, 0));
            Vector2f rect2BottomRight = shape.InverseTransform.TransformPoint(sprite.Transform.TransformPoint(rect2Size));
            Vector2f rect2BottomLeft = shape.InverseTransform.TransformPoint(sprite.Transform.TransformPoint(0, rect2Size.Y));

            bool level1 = (rect1Bounds.Contains(rect2TopLeft.X, rect1TopLeft.Y)) ||
                   (rect1Bounds.Contains(rect2TopRight.X, rect2TopRight.Y)) ||
                   (rect1Bounds.Contains(rect2BottomLeft.X, rect2BottomLeft.Y)) ||
                   (rect1Bounds.Contains(rect2BottomRight.X, rect1BottomRight.Y)) ||
                   (rect2Bounds.Contains(rect1TopLeft.X, rect1TopLeft.Y)) ||
                   (rect2Bounds.Contains(rect1TopRight.X, rect1TopRight.Y)) ||
                   (rect2Bounds.Contains(rect1BottomLeft.X, rect1BottomLeft.Y)) ||
                   (rect2Bounds.Contains(rect1BottomRight.X, rect1BottomRight.Y));


            List<Vector2f> rect1Points = new List<Vector2f>
            {
                rect1BottomLeft,
                rect1BottomRight,
                rect1TopRight,
                rect1TopLeft,
            };
            if (!satRectangleAndPoints(rect2Size, rect1Points))
                return false;
            List<Vector2f> rect2Points = new List<Vector2f>
            {
                rect2BottomLeft,
                rect2BottomRight,
                rect2TopRight,
                rect2TopLeft,
            };
            return satRectangleAndPoints(rect1Size, rect2Points);


            //return xOverlap && yOverlap;
        }

        private static bool satRectangleAndPoints(Vector2f rectangleSize, List<Vector2f> points)
        {
            bool allPointsLeftOfRectangle = true;
            bool allPointsRightOfRectangle = true;
            bool allPointsAboveRectangle = true;
            bool allPointsBelowRectangle = true;
            foreach (var point in points)
            {
                if (point.X >= 0)
                    allPointsLeftOfRectangle = false;
                if (point.X <= rectangleSize.X)
                    allPointsRightOfRectangle = false;
                if (point.Y >= 0)
                    allPointsAboveRectangle = false;
                if (point.Y <= rectangleSize.Y)
                    allPointsBelowRectangle = false;
            }
            return !(allPointsLeftOfRectangle || allPointsRightOfRectangle || allPointsAboveRectangle || allPointsBelowRectangle);
        }

        private static float ToRadians(this float angle)
        {
            angle %= 360;
            return (float)(Math.PI * angle / 180.0);
        }

        private static bool ValueInRange(float value, float min, float max)
        {
            return (value >= min) && (value <= max);
        }

        private static void DrawEnemies(RenderWindow window)
        {
            for (int i = 0; i < Enemies.Count; i++)
                Enemies[i].Draw(window);
        }

        private static void DrawBullets(RenderWindow window)
        {
            for (int i = 0; i < Player1.Bullets.Count; i++)
                Player1.Bullets[i].Draw(window);
        }

        private static void DrawLasers(RenderWindow window)
        {
            for (int i = 0; i < Player1.Lasers.Count; i++)
                Player1.Lasers[i].Draw(window);
        }
        private static void DrawBombs(RenderWindow window)
        {
            for (int i = 0; i < Player1.Bombs.Count; i++)
                Player1.Bombs[i].Draw(window);
        }
    }
}