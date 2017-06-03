using System;
using System.Collections.Generic;

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
    }
}
