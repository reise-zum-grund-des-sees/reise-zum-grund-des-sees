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
        ISpecialBlock BlockAt(int x, int y, int z);
        ISpecialBlock BlockAt(Vector3Int _pos);

        IReadOnlyList<IReadonlyWorldObject> Objects { get; }
    }

    interface IWorldObjectContainer : IReadonlyWorldObjectContainer
    {
        new IList<IWorldObject> Objects { get; }

        void AddObject(IWorldObject _object);
        void RemoveObject(IWorldObject _object);
    }

    class World : BaseWorld, IUpdateable, IWorldObjectContainer
    {
        protected readonly IDictionary<Vector3Int, ISpecialBlock> specialBlocks = new Dictionary<Vector3Int, ISpecialBlock>();
        protected readonly IList<IWorldObject> objects = new List<IWorldObject>();

        public IList<IWorldObject> Objects => objects;
        IReadOnlyList<IReadonlyWorldObject> IReadonlyWorldObjectContainer.Objects => (IReadOnlyList<IWorldObject>)objects;

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
            if (!onBaseWorldBlockChangedBlocker)
            {
                if (_oldBlock.IsSpecialBlock() && _oldBlock != _newBlock)
                    removeBlock(BlockAt(x, y, z));
                if (_newBlock.IsSpecialBlock())
                    AddSpecialBlock(_newBlock.Instanciate(new Vector3Int(x, y, z)));
            }
            onBaseWorldBlockChangedBlocker = false;
        }

        public ISpecialBlock BlockAt(Vector3Int _pos)
        {
            if (specialBlocks.TryGetValue(_pos, out ISpecialBlock _obj))
                return _obj;
            else
                return null;
        }
        public ISpecialBlock BlockAt(int x, int y, int z)
            => BlockAt(new Vector3Int(x, y, z));

        protected virtual void AddSpecialBlock(ISpecialBlock _object)
        {
            if (specialBlocks.ContainsKey(_object.Position))
                throw new ArgumentException("There is already an object at that position.");

            specialBlocks[_object.Position] = _object;
            Blocks[_object.Position.X, _object.Position.Y, _object.Position.Z] = _object.Type;
        }

        private void removeBlock(ISpecialBlock _object)
        {
            removeBlock(_object.Position);
        }
        private void removeBlock(Vector3Int _pos)
        {
            specialBlocks.Remove(_pos);
            onBaseWorldBlockChangedBlocker = true;
            Blocks[_pos.X, _pos.Y, _pos.Z] = WorldBlock.None;
        }

        public virtual void AddObject(IWorldObject _object)
        {
            objects.Add(_object);
        }
        public void RemoveObject(IWorldObject _object)
        {
            objects.Remove(_object);
        }

        public override UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            UpdateDelegate _delegate = base.Update(_view, _flags, _inputArgs, _passedTime);

            foreach (var _block in specialBlocks)
                _delegate = _delegate.ContinueWith(_block.Value.Update(_view, _flags, _inputArgs, _passedTime));

            foreach (var _object in objects)
                _delegate = _delegate.ContinueWith(_object.Update(_view, _flags, _inputArgs, _passedTime));

            return _delegate;
        }
    }
}
