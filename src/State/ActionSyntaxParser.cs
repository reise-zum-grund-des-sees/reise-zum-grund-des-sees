using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    static class ActionSyntaxParser
    {
        public static Action Parse(string _actionString, object _sender, ObjectIDMapper _idMapper)
        {
            string[] _actions = _actionString.Split(';');

            return _actions.Select(_action => ParseSingle(_action, _sender, _idMapper)).Aggregate((a1, a2) => a1.ContinueWith(a2));
        }

        private static Action ParseSingle(string _actionString, object _sender, ObjectIDMapper _idMapper)
        {
            string[] _parts = _actionString.Split('.', '(');

            Action _runningAction = () => { throw new Exception(); };
            Action _returnAction = () => { _runningAction(); };

            _idMapper.WhenObjectAdded(_parts[0].Trim().ToId(), _obj =>
            {
                switch (_parts[1].Trim())
                {
                    case "start":
                        _runningAction = (_obj as IStartStopable).Start;
                        break;
                    case "stop":
                        _runningAction = (_obj as IStartStopable).Stop;
                        break;
                }
            });

            return _returnAction;
        }
    }
}
