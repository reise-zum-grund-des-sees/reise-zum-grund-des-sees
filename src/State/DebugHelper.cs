using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    static class DebugHelper
    {
        public static class Information
        {
            public static double FPS;
            public static TimeSpan TotalGameTime;
            public static ulong TotalFrameCount;
            public static uint RenderedWorldVertices;
            public static uint RenderedOtherVertices;
            public static uint RenderedWorldChunks;
            public static Vector3 EditorCursorPosition;
            public static Vector3 PlayerPosition;
            public static float CameraRotation;
            public static Vector3 CameraPosition;
            public static double updateTime;
            public static double renderTime;
            public static Queue<string> Logs = new Queue<string>();

            public static string ToString()
            {
                StringBuilder logs = new StringBuilder();

                if (Logs.Count > 10)
                    for (int i = 0; i < Logs.Count - 10; i++)
                        Logs.Dequeue();

                foreach (string s in Logs)
                {
                    logs.Append(s);
                    logs.Append("\n");
                }

                return
                    $"FPS: { FPS }\n" +
                    $"Vertices: { RenderedWorldVertices } (World) + { RenderedOtherVertices } (Models)\r\n" +
                    $"Update Time: { updateTime }\r\n" +
                    $"Render Time: { renderTime }\r\n" +
                    $"Player position: { PlayerPosition }\n" +
                    $"EditorCursor position: { EditorCursorPosition }\n" +
                    $"Expexted FPS: { 1.0 / (updateTime + renderTime) }\r\n" +
                    $"Camera Position: { CameraPosition }";
            }
        }

        private static string textBuffer = "";

        public static void RenderOverlay(SpriteBatch _spriteBatch, SpriteFont _font)
        {
            _spriteBatch.Begin();

            if (Information.TotalFrameCount % 10 == 0)
                textBuffer = Information.ToString();

            _spriteBatch.DrawString(_font, textBuffer, new Vector2(0, 0), Color.Black);

            _spriteBatch.End();
        }

        public static void Log(string _log)
        {
            Information.Logs.Enqueue(_log);
        }
    }
}
