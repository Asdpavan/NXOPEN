using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using pavan;

namespace SimpleGame
{
    class Program : GameWindow
    {
        private const float PlayerSpeed = 0.01f; // Slower player speed
        private const float ObstacleSpeed = 0.005f; // Slower obstacle speed
        private const float ObstacleSpawnInterval = 4.0f; // Slower obstacle spawn interval

        private float playerX = 0.0f;
        private float playerY = 0.0f;
        private List<float> obstaclePositions = new List<float>();
        private float obstacleSpawnTimer = 0.0f;

        public Program() : base(800, 600, GraphicsMode.Default, "Simple Game") { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Move player based on keyboard input
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Key.Up))
                playerY += PlayerSpeed;
            if (keyboardState.IsKeyDown(Key.Down))
                playerY -= PlayerSpeed;
            if (keyboardState.IsKeyDown(Key.Left))
                playerX -= PlayerSpeed;
            if (keyboardState.IsKeyDown(Key.Right))
                playerX += PlayerSpeed;

            // Clamp player position within the screen bounds
            playerX = MathHelper.Clamp(playerX, -1.0f + PlayerSpeed / 2, 1.0f - PlayerSpeed / 2);
            playerY = MathHelper.Clamp(playerY, -1.0f + PlayerSpeed / 2, 1.0f - PlayerSpeed / 2);

            // Update obstacle positions
            for (int i = 0; i < obstaclePositions.Count; i++)
            {
                obstaclePositions[i] -= ObstacleSpeed;
                if (obstaclePositions[i] < -1.0f)
                {
                    obstaclePositions.RemoveAt(i);
                    i--;
                }
            }

            // Spawn new obstacles periodically
            obstacleSpawnTimer += (float)e.Time;
            if (obstacleSpawnTimer > ObstacleSpawnInterval)
            {
                obstaclePositions.Add(1.0f);
                obstacleSpawnTimer = 0.0f;
            }

            // Check for collisions
            foreach (var obstaclePosition in obstaclePositions)
            {
                if (Math.Abs(obstaclePosition - playerX) < PlayerSpeed / 2 &&
                    Math.Abs(playerY) < PlayerSpeed / 2)
                {
                    Console.WriteLine("Game Over!");
                    Exit();
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw player
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(1.0f, 0.0f, 0.0f); // Red
            GL.Vertex2(playerX - PlayerSpeed / 2, playerY - PlayerSpeed / 2);
            GL.Vertex2(playerX + PlayerSpeed / 2, playerY - PlayerSpeed / 2);
            GL.Vertex2(playerX + PlayerSpeed / 2, playerY + PlayerSpeed / 2);
            GL.Vertex2(playerX - PlayerSpeed / 2, playerY + PlayerSpeed / 2);
            GL.End();

            // Draw obstacles
            foreach (var obstaclePosition in obstaclePositions)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(0.0f, 1.0f, 0.0f); // Green
                GL.Vertex2(obstaclePosition - PlayerSpeed / 2, -1.0f);
                GL.Vertex2(obstaclePosition + PlayerSpeed / 2, -1.0f);
                GL.Vertex2(obstaclePosition + PlayerSpeed / 2, -1.0f + PlayerSpeed);
                GL.Vertex2(obstaclePosition - PlayerSpeed / 2, -1.0f + PlayerSpeed);
                GL.End();
            }

            SwapBuffers();
        }

        [STAThread]
        static void Main()
        {
            using (var program = new Program())
            {
                program.Run(60.0);
            }
        }
    }
}
