﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    class CollisionDetector
    {
        private List<ICollisionObject> objects = new List<ICollisionObject>();
        private IBlockWorld world;

        public CollisionDetector(IBlockWorld _world)
        {
            world = _world;
        }

        public void AddObject(ICollisionObject _object)
        {
            objects.Add(_object);
        }

        public struct CollisionSource
        {
            public ICollisionObject Object;
            public WorldBlock WorldBlock;

            public Type CollisionType;

            public enum Type
            {
                WithObject,
                WithWorldBlock
            }

            public CollisionSource(ICollisionObject _object)
            {
                CollisionType = Type.WithObject;
                Object = _object;
                WorldBlock = WorldBlock.None;
            }

            public CollisionSource(WorldBlock _worldBlock)
            {
                CollisionType = Type.WithWorldBlock;
                Object = null;
                WorldBlock = _worldBlock;
            }
        }


        public KeyValuePair<Direction, CollisionSource>[] CheckCollision(Vector3 _idealMovement, ICollisionObject _object)
        {
            throw new NotImplementedException();
        }


        public static Direction CollisionDetectionWithSplittedMovement(ref Vector3 _movA, Hitbox _hitA, Hitbox _hitB)
        {
            Vector3[] _splits = splitVector(_movA);
            Direction _dir = Direction.None;

            Hitbox _tmpHit = _hitA;
            for (int i = 0; i < _splits.Length; i++)
            {
                int c = 0;
                if (_splits[i].X == 0) c++;
                if (_splits[i].Y == 0) c++;
                if (_splits[i].Z == 0) c++;
                if (c < 2) throw new ArgumentException();
                _dir |= CollisionDetection(ref _splits[i], _tmpHit, _hitB);
                _tmpHit = new Hitbox(_tmpHit.X + _splits[i].X, _tmpHit.Y + _splits[i].Y, _tmpHit.Z + _splits[i].Z,
                    _tmpHit.Width, _tmpHit.Height, _tmpHit.Depth);
            }

            _movA = _splits.Aggregate((v1, v2) => v1 + v2);
            return _dir;
        }

        const float MAX_SPLIT_PART_LENGTH = 0.1f;
        private static Vector3[] splitVector(Vector3 v)
        {
            int _splitCountX = (int)Math.Abs(v.X / MAX_SPLIT_PART_LENGTH) + 1;
            int _indexX = 0;
            int _splitCountY = (int)Math.Abs(v.Y / MAX_SPLIT_PART_LENGTH) + 1;
            int _indexY = 0;
            int _splitCountZ = (int)Math.Abs(v.Z / MAX_SPLIT_PART_LENGTH) + 1;
            int _indexZ = 0;
            Console.WriteLine(_splitCountX + ", " + _splitCountY + ", " + _splitCountZ);
            Vector3[] _splits = new Vector3[_splitCountX + _splitCountY + _splitCountZ];

            for (int i = 0; i < _splits.Length; i++)
            {
                float _progressX = _indexX / _splitCountX;
                float _progressY = _indexY / _splitCountY;
                float _progressZ = _indexZ / _splitCountZ;

                if (_progressX <= _progressY && _progressX <= _progressZ)
                {
                    _splits[i] = new Vector3(v.X / _splitCountX, 0, 0);
                    _indexX++;
                }
                else if (_progressZ <= _progressY)
                {
                    _splits[i] = new Vector3(0, 0, v.Z / _splitCountZ);
                    _indexZ++;
                }
                else
                {
                    _splits[i] = new Vector3(0, v.Y / _splitCountY, 0);
                    _indexY++;
                }
            }

            return _splits;
        }

        const float FLOATING_POINT_INCORRECTION = 0.0001f;
        /// <summary>
        /// Erkenne Kollisionen zwischen einer bewegten und einer statischen Hitbox
        /// </summary>
        /// <param name="_movA">Die Referenz des Bewegungsvektors der von der Funktion auf mögliche Werte begrenzt wird</param>
        /// <param name="_hitA">Die Hitbox des bewegten Objektes</param>
        /// <param name="_hitB">Die Hitbox des statischen Objektes</param>
        /// <param name="_possibleMovements">Mögliche Ausweichbewegungen des bewegten Hitbox</param>
        /// <returns>Flags, die die Seiten der bewegenden Hitbox angeben, welche mit der statischen Hitbox kollidieren</returns>
        private static Direction CollisionDetection(ref Vector3 _movA, Hitbox _hitA, Hitbox _hitB)
        {
            Direction _collInfo = Direction.None;

            float xDiff = 0, yDiff = 0, zDiff = 0;
            bool xCollFlag = false, yCollFlag = false, zCollFlag = false;

            // collision left
            if (_hitA.X + _movA.X < _hitB.X + _hitB.Width - FLOATING_POINT_INCORRECTION &
                _hitA.X + _hitA.Width + _movA.X > _hitB.X + FLOATING_POINT_INCORRECTION)
            {
                xCollFlag = true;
                if (_movA.X > 0)
                {
                    xDiff = (_hitB.X - (_hitA.X + _hitA.Width)) - _movA.X;
                    _collInfo |= Direction.Right;
                }
                else if (_movA.X < 0)
                {
                    xDiff = (_hitB.X + _hitB.Width - _hitA.X) - _movA.X;
                    _collInfo |= Direction.Left;
                }
            }

            // collision top/bottom
            if (_hitA.Y + _movA.Y < _hitB.Y + _hitB.Height - FLOATING_POINT_INCORRECTION &
                _hitA.Y + _hitA.Height + _movA.Y > _hitB.Y + FLOATING_POINT_INCORRECTION)
            {
                yCollFlag = true;
                if (_movA.Y > 0)
                {
                    yDiff = (_hitB.Y - (_hitA.Y + _hitA.Height)) - _movA.Y;
                    _collInfo |= Direction.Top;
                }
                else if (_movA.Y < 0)
                {
                    yDiff = (_hitB.Y + _hitB.Height - _hitA.Y) - _movA.Y;
                    _collInfo |= Direction.Bottom;
                }
            }

            // collision front/back
            if (_hitA.Z + _movA.Z < _hitB.Z + _hitB.Depth - FLOATING_POINT_INCORRECTION &
                _hitA.Z + _hitA.Depth + _movA.Z > _hitB.Z + FLOATING_POINT_INCORRECTION)
            {
                zCollFlag = true;
                if (_movA.Z > 0)
                {
                    zDiff = (_hitB.Z - (_hitA.Z + _hitA.Depth)) - _movA.Z;
                    _collInfo |= Direction.Front;
                }
                else if (_movA.Z < 0)
                {
                    zDiff = (_hitB.Z + _hitB.Depth - _hitA.Z) - _movA.Z;
                    _collInfo |= Direction.Back;
                }
            }

            if (xCollFlag & yCollFlag & zCollFlag)
            {
                if (_collInfo.HasFlag(Direction.Top) | _collInfo.HasFlag(Direction.Bottom))
                {
                    _movA.Y += yDiff;
                    return _collInfo & (Direction.Top | Direction.Bottom);
                }
                else if (_collInfo.HasFlag(Direction.Left) | _collInfo.HasFlag(Direction.Right))
                {
                    _movA.X += xDiff;
                    return _collInfo & (Direction.Left | Direction.Right);
                }
                else if (_collInfo.HasFlag(Direction.Front) | _collInfo.HasFlag(Direction.Back))
                {
                    _movA.Z += zDiff;
                    return _collInfo & (Direction.Front | Direction.Back);
                }
                else return Direction.None;
            }
            else return Direction.None;
        }

        /// <summary>
        /// Erkenne Kollisionen zwischen einer Hitbox und der Welt
        /// </summary>
        /// <param name="_movement">Die Referenz des Bewegungsvektors der von der Funktion auf mögliche Werte begrenzt wird</param>
        /// <param name="_hitbox">Die Hitbox des bewegten Objektes</param>
        /// <param name="_world">Die Welt, auf der sich das Objekt befindet</param>
        /// <returns>Flags, die die Seiten der bewegenden Hitbox angeben, welche mit der Welt kollidieren</returns>
        public static Direction CollisionWithWorld(ref Vector3 _movement, Hitbox _hitbox, IReadonlyBlockWorld _world)
        {
            int _hitX = (int)_hitbox.X;
            int _hitY = (int)_hitbox.Y;
            int _hitZ = (int)_hitbox.Z;

            Direction _collInfo = Direction.None;

            // test for hitbox surrounding blocks
            for (int x = _hitX - 1; x <= _hitX + (int)_hitbox.Width + 1; x++)
                for (int y = _hitY - 1; y <= _hitY + (int)_hitbox.Height + 1; y++)
                    for (int z = _hitZ - 1; z <= _hitZ + (int)_hitbox.Depth + 1; z++)
                    {
                        WorldBlock b = _world[x, y, z];
                        if (b.HasCollision())
                            _collInfo |= CollisionDetectionWithSplittedMovement(ref _movement, _hitbox, new Hitbox(x, y, z, b.GetBounds()));
                    }

            return _collInfo;
        }
    }

    struct Hitbox
    {
        public readonly float X, Y, Z;
        public readonly float Width, Depth, Height;

        public Hitbox(float x, float y, float z, float _width, float _height, float _depth)
        {
            Width = _width;
            Depth = _depth;
            Height = _height;
            X = x;
            Y = y;
            Z = z;
        }

        public Hitbox(Vector3 _position, Vector3 _size)
            : this(_position.X, _position.Y, _position.Z, _size.X, _size.Y, _size.Z)
        { }

        public Hitbox(float x, float y, float z, Vector3 _size)
            : this(x, y, z, _size.X, _size.Y, _size.Z) { }
    }

    [Flags]
    enum Direction
    {
        None = 0,
        Left = 1,
        Right = 2,
        Front = 4,
        Back = 8,
        Top = 16,
        Bottom = 32,
        All = 63
    }
}
