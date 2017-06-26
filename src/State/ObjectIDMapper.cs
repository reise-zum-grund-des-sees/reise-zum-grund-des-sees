using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class ObjectIDMapper
    {
        private Dictionary<int, object> mappings = new Dictionary<int, object>();

        private int nextID = 0;

        public ObjectIDMapper()
        {

        }

        public int AddObject(object _object)
        {
            while (mappings.ContainsKey(nextID++));

            mappings[nextID - 1] = _object;

            return nextID - 1;
        }

        public int AddObject(object _object, int _id)
        {
            mappings[_id] = _object;
            return _id;
        }

        public T GetObject<T>(int _id) where T : class
        {
            return mappings[_id] as T;
        }

        public int GetID(object _obj)
        {
            return (from item in mappings where item.Value == _obj select item.Key).Single();
        }
    }
}
