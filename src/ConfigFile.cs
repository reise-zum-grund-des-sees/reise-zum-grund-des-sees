using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReiseZumGrundDesSees
{
    static class ConfigFile
    {
        public static Dictionary<string, string[]> Load(string _filepath)
        {
            Dictionary<string, string[]> _dict = new Dictionary<string, string[]>();

            string[] _lines = File.ReadAllLines(_filepath);
            foreach (string _line in _lines)
            {
                string[] _items = _line.Split('=', ',', ';');
                for (int i = 0; i < _items.Length; i++) _items[i] = _items[i].Trim();

                _dict[_items[0]] = _items.Skip(1).ToArray(); 
            }

            return _dict;
        }
    }
}
