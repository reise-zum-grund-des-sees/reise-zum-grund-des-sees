using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] _args)
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif

    static class Helper
    {
        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> _dict, IDictionary<TKey, TValue> _newData)
        {
            foreach (KeyValuePair<TKey, TValue> i in _newData)
                _dict[i.Key] = i.Value;
        }
        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> _dict, KeyValuePair<TKey, TValue>[] _data)
        {
            foreach (KeyValuePair<TKey, TValue> i in _data)
                _dict[i.Key] = i.Value;
        }

        public static Vector3 ToVector3(this string s)
        {
            string[] _input = s.Split(';', '-').Select(s2 => s2.Trim()).ToArray();
            return new Vector3(float.Parse(_input[0]), float.Parse(_input[1]), float.Parse(_input[2]));
        }
        public static Vector3Int ToVector3Int(this string s)
        {
            string[] _input = s.Split(';', '-').Select(s2 => s2.Trim()).ToArray();
            return new Vector3Int(int.Parse(_input[0]), int.Parse(_input[1]), int.Parse(_input[2]));
        }
        public static string ToNiceString(this Vector3 v)
        => $"{v.X}-{v.Y}-{v.Z}";

        public static Rectangle Scale(this Rectangle _rect, Vector2 _scaleFactor)
        {
            return new Rectangle(
                (_rect.Location.ToVector2() * _scaleFactor).ToPoint(),
                (_rect.Size.ToVector2()     * _scaleFactor).ToPoint());
        }

        public static int ToId(this string s)
        {
            return int.Parse(s.Select(c => ((char)(c + '0' - 'a')).ToString()).Aggregate((s1, s2) => s1 + s2));
        }

        public static string IdAsString(this int i)
        {
            return new string(i.ToString().Select(c => (char)(c + 'a' - '0')).ToArray());
        }

        public static Action ContinueWith(this Action _action1, Action _action2)
            => () => { _action1?.Invoke(); _action2?.Invoke(); };
    }
}
