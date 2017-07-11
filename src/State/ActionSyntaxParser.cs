using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    static class ActionSyntaxParser
    {
        public class GameAction
        {
            public Action<GameState> BaseAction;
            public Func<ObjectIDMapper, string> ActionEncoding;

            public GameAction(Action<GameState> _baseAction, Func<ObjectIDMapper, string> _actionEncoding)
            {
                BaseAction = _baseAction;
                ActionEncoding = _actionEncoding;
            }
        }

        public static GameAction Parse(string _actionString, object _sender, ObjectIDMapper _idMapper)
        {
            string[] _actions = _actionString.Split(';');

            if (_actions[0].Trim().Length > 0)
                return _actions
                    .Select(_action => ParseSingle(_action, _sender, _idMapper))
                    .Aggregate((a1, a2) => new GameAction(_gs => { a1.BaseAction(_gs); a2.BaseAction(_gs); },
                                                          _idM => a1.ActionEncoding(_idM) + ":" + a2.ActionEncoding(_idM)));
            else
                return null;
        }

        private static GameAction ParseSingle(string _actionString, object _sender, ObjectIDMapper _idMapper)
        {
            string[] _parts = splitAction(_actionString);

            GameAction _gameAction = new GameAction(_ => throw new Exception(), 
                                                    _ => throw new Exception());

            if (_parts[0] == "world")
            {
                if (_parts[1] == "set-block")
                {
                    Vector3Int _pos = _parts[2].ToVector3Int();
                    _gameAction.BaseAction = gs =>
                    {
                        gs.World.Blocks[_pos.X, _pos.Y, _pos.Z] = (WorldBlock)Enum.Parse(typeof(WorldBlock), _parts[3]);
                    };
                    _gameAction.ActionEncoding = _ =>
                    {
                        return _actionString;
                    };
                }
            }
            else
                _idMapper.WhenObjectAdded(_parts[0].ToId(), _obj =>
            {
                switch (_parts[1])
                {
                    case "start":
                        _gameAction.BaseAction = _ => (_obj as IStartStopable).Start();
                        _gameAction.ActionEncoding = _otherIdMapper =>
                            _otherIdMapper.AddObject(_obj).IdAsString() + ".start";
                        break;
                    case "stop":
                        _gameAction.BaseAction = _ => (_obj as IStartStopable).Stop();
                        _gameAction.ActionEncoding = _otherIdMapper =>
                            _otherIdMapper.AddObject(_obj).IdAsString() + ".stop";
                        break;
                }
            });

            return _gameAction;
        }

        private static string[] splitAction(string _input)
        {
            int _objectEndIndex = _input.IndexOf('.');
            string _objectStr = _input.Substring(0, _objectEndIndex).Trim();

            bool _containsPrantheses = _input.Contains('(');

            int _actionEndIndex = (_containsPrantheses) ? _input.IndexOf('(', _objectEndIndex) : _input.Length;
            string _actionStr = _input.Substring(_objectEndIndex + 1, _actionEndIndex - _objectEndIndex - 1).Trim();

            if (_containsPrantheses)
            {
                string[] _arguments = _input
                    .Substring(_actionEndIndex + 1, _input.IndexOf(')', _actionEndIndex + 1) - _actionEndIndex - 1)
                    .Split(':');
                for (int i = 0; i < _arguments.Length; i++) _arguments[i] = _arguments[i].Trim();

                string[] _returnValue = new string[_arguments.Length + 2];
                _returnValue[0] = _objectStr;
                _returnValue[1] = _actionStr;
                Array.Copy(_arguments, 0, _returnValue, 2, _arguments.Length);

                return _returnValue;
            }
            else
            {
                return new string[] { _objectStr, _actionStr };
            }
        }
    }
}
