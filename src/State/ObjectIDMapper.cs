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
        private Dictionary<int, List<Action<object>>> whenObjectAddedActions = new Dictionary<int, List<Action<object>>>();

        private int nextID = 0;

        public ObjectIDMapper()
        {

        }

        public int AddObject(object _object)
        {
            if (mappings.ContainsValue(_object))
            {
                var _item = mappings.First(i => i.Value == _object);
                return _item.Key;
            }


            while (mappings.ContainsKey(nextID++)) ;

            int _id = nextID - 1;

            mappings[_id] = _object;

            if (whenObjectAddedActions.ContainsKey(_id))
            {
                if (!(whenObjectAddedActions[_id] == null))
                    foreach (var _item in whenObjectAddedActions[_id])
                        _item?.Invoke(_object);

                whenObjectAddedActions.Remove(_id);
            }

            return _id;
        }

        public int AddObject(object _object, int _id)
        {
            mappings[_id] = _object;

            if (whenObjectAddedActions.ContainsKey(_id))
            {
                if (!(whenObjectAddedActions[_id] == null))
                    foreach (var _item in whenObjectAddedActions[_id])
                        _item?.Invoke(_object);

                whenObjectAddedActions.Remove(_id);
            }

            return _id;
        }

        public T GetObject<T>(int _id) where T : class
        {
            return mappings[_id] as T;
        }

        public void WhenObjectAdded(int _id, Action<object> _action)
        {
            if (mappings.ContainsKey(_id))
                _action(mappings[_id]);
            else
            {
                if (whenObjectAddedActions.ContainsKey(_id))
                {
                    if (whenObjectAddedActions[_id] == null)
                        whenObjectAddedActions[_id] = new List<Action<object>>();

                    whenObjectAddedActions[_id].Add(_action);
                }
                else
                {
                    whenObjectAddedActions.Add(_id, new List<Action<object>>());
                    whenObjectAddedActions[_id].Add(_action);
                }
            }
        }

        public int GetID(object _obj)
        {
            return (from item in mappings where item.Value == _obj select item.Key).Single();
        }
    }
}
