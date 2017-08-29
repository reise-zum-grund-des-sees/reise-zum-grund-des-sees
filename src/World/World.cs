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
        public readonly IDictionary<Vector3Int, ISpecialBlock> specialBlocks = new Dictionary<Vector3Int, ISpecialBlock>();
        protected readonly IList<IWorldObject> objects = new List<IWorldObject>();

        public IList<IWorldObject> Objects => objects;
        IReadOnlyList<IReadonlyWorldObject> IReadonlyWorldObjectContainer.Objects => (IReadOnlyList<IWorldObject>)objects;

        private bool onBaseWorldBlockChangedBlocker = false;

        public World(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper, string _baseDir) : base(_config, _baseDir)
        {
            Blocks.OnBlockChanged += OnBaseWorldBlockChanged;

            foreach (var _obj in _config.Nodes["special_blocks"].Nodes)
            {
                ISpecialBlock _spBlck = SpecialBlock.Instanciate(_obj.Value, _idMapper);
                _idMapper.AddObject(_spBlck, _obj.Key.ToId());
                AddSpecialBlock(_spBlck);
            }

            foreach (var _obj in _config.Nodes["objects"].Nodes)
            {
                IWorldObject _object = WorldObject.Instanciate(_obj.Value, _idMapper);
                _idMapper.AddObject(_object, _obj.Key.ToId());
                AddObject(_object);
            }
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
                if (_oldBlock == WorldBlock.PressurePlateDown && _newBlock == WorldBlock.PressurePlateUp)
                {
                    specialBlocks[new Vector3Int(x, y, z)] = BlockAt(new Vector3Int(x, y, z));
                    //onBaseWorldBlockChangedBlocker = true;
                   // Blocks[x, y, z] = _newBlock;
                }
                else if (_oldBlock == WorldBlock.PressurePlateUp && _newBlock == WorldBlock.PressurePlateDown)
                {
                    specialBlocks[new Vector3Int(x, y, z)] = BlockAt(new Vector3Int(x, y, z));
                   // onBaseWorldBlockChangedBlocker = true;
                  //  Blocks[x, y, z] = _newBlock;
                }
                else
                {
                    if (_oldBlock.IsSpecialBlock() && _oldBlock != _newBlock)
                        removeBlock(BlockAt(x, y, z));
                    if (_newBlock.IsSpecialBlock() && _oldBlock != _newBlock)
                        AddSpecialBlock(_newBlock.Instanciate(new Vector3Int(x, y, z)));
                }
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
            onBaseWorldBlockChangedBlocker = true;
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

        public override ConfigFile.ConfigNode GetState(ObjectIDMapper _idMapper)
        {
            ConfigFile.ConfigNode _specialBlocks = new ConfigFile.ConfigNode();
            ConfigFile.ConfigNode _objects = new ConfigFile.ConfigNode();

            foreach (var _specialBlock in specialBlocks)
            {
                int _id = _idMapper.AddObject(_specialBlock.Value);
                var _state = _specialBlock.Value.GetState(_idMapper);
                _state.Items["type"] = _specialBlock.Value.GetType().ToString();
                _specialBlocks.Nodes[_id.IdAsString()] =
                    _state;
            }

            foreach (var _object in objects)
            {
                int _id = _idMapper.AddObject(_object);
                var _state = _object.GetState(_idMapper);
                _state.Items["type"] = _object.GetType().ToString();
                _objects.Nodes[_id.IdAsString()] =
                    _state;
            }

            ConfigFile.ConfigNode _base = base.GetState(_idMapper);
            _base.Nodes["special_blocks"] = _specialBlocks;
            _base.Nodes["objects"] = _objects;
            return _base;
        }

        public World ResizeRegions(Rectangle _rect)
        {
            World _resizedWorld = new World(
                RegionSize.X,
                RegionSize.Y,
                RegionSize.Z,
                _rect.Width,
                _rect.Height,
                SpawnPos);

            for (int x = 0; x < _resizedWorld.RegionSize.X * _resizedWorld.RegionsCount.X; x++)
                for (int y = 0; y < _resizedWorld.RegionSize.Y; y++)
                    for (int z = 0; z < _resizedWorld.RegionSize.Z * _resizedWorld.RegionsCount.Y; z++)
                    {
                        int _oldX = x + _rect.X * RegionSize.X;
                        int _oldZ = z + _rect.Y * RegionSize.Z;
                        _resizedWorld.Blocks[x, y, z] = this.Blocks[_oldX, y, _oldZ];
                    }

            return _resizedWorld;
        }
    }
}
