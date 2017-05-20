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
        //public static DebugInfo Information = new DebugInfo(0, new TimeSpan(0), 0, 0, 0);
        public static class Information
        {
            public static double FPS;
            public static TimeSpan TotalGameTime;
            public static ulong TotalFrameCount;
            public static uint RenderedWorldVertices;
            public static uint RenderedWorldChunks;
            public static Queue<string> Logs = new Queue<string>();

            public static string ToString()
            {
                StringBuilder logs = new StringBuilder();
                foreach (string s in Logs)
                {
                    logs.Append(s);
                    logs.Append("\n");
                }

                if (Logs.Count > 10)
                    for (int i = 0; i < Logs.Count - 10; i++)
                        Logs.Dequeue();

                return
                    $"Total game time: { TotalGameTime.ToString() }\n" +
                    $"Total Frame Count: { TotalFrameCount }\n" +
                    $"FPS: { FPS }\n" +
                    $"Rendered World Vertices: { RenderedWorldVertices }\n" +
                    $"Rendered World Chunks: { RenderedWorldChunks }\n" +
                    $"Logs:\n { logs.ToString() }";

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
