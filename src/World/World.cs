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
    class World : BaseWorld, IUpdateable
    {
        private readonly IDictionary<Vector3Int, IWorldObject> objects = new Dictionary<Vector3Int, IWorldObject>();
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

        public IWorldObject ObjectAt(int x, int y, int z)
            => objects[new Vector3Int(x, y, z)];

        public void AddObject(IWorldObject _object)
        {
            if (objects.ContainsKey(_object.Position))
                throw new ArgumentException("There is already an object at that position.");

            objects[_object.Position] = _object;
            Blocks[_object.Position.X, _object.Position.Y, _object.Position.Z] = _object.Type;
        }

        public void RemoveObject(IWorldObject _object)
        {
            objects.Remove(_object.Position);
            onBaseWorldBlockChangedBlocker = true;
            Blocks[_object.Position.X, _object.Position.Y, _object.Position.Z] = WorldBlock.None;
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
