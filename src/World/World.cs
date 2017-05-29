using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IReadonlyWorldObjectContainer
    {
        IWorldObject ObjectAt(int x, int y, int z);
        IWorldObject ObjectAt(Vector3Int _pos);
    }

    interface IWorldObjectContainer : IReadonlyWorldObjectContainer
    {
        void AddObject(IWorldObject _obj);

        void RemoveObject(Vector3Int _pos);
        void RemoveObject(IWorldObject _obj);
    }

    class World : BaseWorld, IUpdateable, IWorldObjectContainer
    {
        protected readonly IDictionary<Vector3Int, IWorldObject> objects = new Dictionary<Vector3Int, IWorldObject>();
        private bool onBaseWorldBlockChangedBlocker = false;

        public World(string _basePath) : base(_basePath)
        {
            Blocks.OnBlockChanged += OnBaseWorldBlockChanged;
        }

        public World(int _regionSizeX, int _regionSizeY, int _regionSizeZ, int _regionsCountX, int _regionsCountZ, Vector3 _spawnPos)
            : base(_regionSizeX, _regionSizeY, _regionSizeZ, _regionsCountX, _regionsCountZ, _spawnPos)
        {
            Blocks.OnBlockChanged += OnBaseWorldBlockChanged;
        }

        private void OnBaseWorldBlockChanged(WorldBlock _oldBlock, WorldBlock _newBlock, int x, int y, int z)
        {
            if (!onBaseWorldBlockChangedBlocker && _oldBlock.IsPartOfWorldObject() && _oldBlock != _newBlock)
                RemoveObject(ObjectAt(x, y, z));
            onBaseWorldBlockChangedBlocker = false;
        }

        public IWorldObject ObjectAt(Vector3Int _pos)
        {
            IWorldObject _obj;
            if (objects.TryGetValue(_pos, out _obj))
                return _obj;
            else
                return null;
        }
        public IWorldObject ObjectAt(int x, int y, int z)
            => ObjectAt(new Vector3Int(x, y, z));

        public virtual void AddObject(IWorldObject _object)
        {
            if (objects.ContainsKey(_object.Position))
                throw new ArgumentException("There is already an object at that position.");

            objects[_object.Position] = _object;
            Blocks[_object.Position.X, _object.Position.Y, _object.Position.Z] = _object.Type;
        }

        public void RemoveObject(IWorldObject _object)
        {
            RemoveObject(_object.Position);
        }
        public void RemoveObject(Vector3Int _pos)
        {
            objects.Remove(_pos);
            onBaseWorldBlockChangedBlocker = true;
            Blocks[_pos.X, _pos.Y, _pos.Z] = WorldBlock.None;
        }

        public override UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            UpdateDelegate _delegate = base.Update(_view, _flags, _inputArgs, _passedTime);

            foreach (var _object in objects)
                _delegate = _delegate.ContinueWith(_object.Value.Update(_view, _flags, _inputArgs, _passedTime));

            return _delegate;
        }
    }
}
