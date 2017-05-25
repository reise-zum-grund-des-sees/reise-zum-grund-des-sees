﻿using System;
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
        private readonly List<IWorldObject> objects = new List<IWorldObject>();
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
        {
            foreach (IWorldObject _obj in objects)
                if (_obj.Postion == new Vector3Int(x, y, z))
                    return _obj;
            throw new ArgumentException("Argument is not an Object.");
        }

        public void AddObject(IWorldObject _object)
        {
            objects.Add(_object);
            Blocks[_object.Postion.X, _object.Postion.Y, _object.Postion.Z] = _object.Type;
        }

        public void RemoveObject(IWorldObject _object)
        {
            objects.Remove(_object);
            onBaseWorldBlockChangedBlocker = true;
            Blocks[_object.Postion.X, _object.Postion.Y, _object.Postion.Z] = WorldBlock.None;
        }

        public override UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            UpdateDelegate _delegate = base.Update(_view, _inputArgs, _passedTime);

            foreach (IWorldObject _object in objects)
                _delegate = _delegate.ContinueWith(_object.Update(_view, _inputArgs, _passedTime));

            return _delegate;
        }
    }
}