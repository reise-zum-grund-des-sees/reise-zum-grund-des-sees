using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    interface IReadonlyCollisionDetector
    {
        Dictionary<Direction, CollisionDetector.CollisionSource> CheckCollision(ref Vector3 _movement, ICollisionObject _object);
    }

    interface ICollisionDetector : IReadonlyCollisionDetector
    {
        void AddObject(ICollisionObject _object);
        void RemoveObject(ICollisionObject _object);
    }

    class CollisionDetector : IReadonlyCollisionDetector
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

        public void RemoveObject(ICollisionObject _object)
        {
            objects.Remove(_object);
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


        public Dictionary<Direction, CollisionSource> CheckCollision(ref Vector3 _movement, ICollisionObject _object)
        {
            Vector3[] _splits = splitVector(_movement);
            Dictionary<Direction, CollisionSource> _collisionList = new Dictionary<Direction, CollisionSource>();

            if (!_object.HasMultipleHitboxes)
            {
                Hitbox _tmpHit = _object.Hitbox;
                for (int i = 0; i < _splits.Length; i++)
                {
                    _collisionList.Update(checkCollisionWithWorld(ref _splits[i], _tmpHit, world));
                    foreach (ICollisionObject _otherObj in objects)
                        if (_otherObj != _object && _otherObj.IsEnabled)
                            _collisionList.Update(checkCollisionWithObject(ref _splits[i], _tmpHit, _otherObj));

                    _tmpHit += _splits[i];
                }
            }
            else
            {
                Hitbox[] _tmpHitboxes = _object.Hitboxes;
                for (int i = 0; i < _splits.Length; i++)
                {
                    for (int j = 0; j < _tmpHitboxes.Length; j++)
                    {
                        _collisionList.Update(checkCollisionWithWorld(ref _splits[i], _tmpHitboxes[j], world));
                        foreach (ICollisionObject _otherObj in objects)
                            if (_otherObj != _object && _otherObj.IsEnabled)
                                _collisionList.Update(checkCollisionWithObject(ref _splits[i], _tmpHitboxes[j], _otherObj));
                    }

                    for (int j = 0; j < _tmpHitboxes.Length; j++)
                        _tmpHitboxes[j] += _splits[i];
                }
            }

            if (_splits.Any())
                _movement = _splits.Aggregate((v1, v2) => v1 + v2);

            return _collisionList;
        }

        private Dictionary<Direction, CollisionSource> checkCollisionWithWorld(ref Vector3 _movement, Hitbox _hitbox, IReadonlyBlockWorld _world)
        {
            Dictionary<Direction, CollisionSource> _dict = new Dictionary<Direction, CollisionSource>();

            int _hitX = (int)_hitbox.X;
            int _hitY = (int)_hitbox.Y;
            int _hitZ = (int)_hitbox.Z;

            // test for hitbox surrounding blocks
            for (int x = _hitX - 1; x <= _hitX + (int)_hitbox.Width + 1; x++)
                for (int y = _hitY - 1; y <= _hitY + (int)_hitbox.Height + 1; y++)
                    for (int z = _hitZ - 1; z <= _hitZ + (int)_hitbox.Depth + 1; z++)
                    {
                        WorldBlock b = _world[x, y, z];
                        if (b.HasCollision() && _hitbox.CollidesWithWorldBlock(b))
                        {
                            Direction _dir = CollisionDetection(ref _movement, _hitbox, new Hitbox(x, y, z, b.GetBounds()));
                            if (_dir != Direction.None)
                                _dir.Foreach(d => _dict[d] = new CollisionSource(b));
                        }
                    }

            return _dict;
        }

        private Dictionary<Direction, CollisionSource> checkCollisionWithObject(ref Vector3 _movement, Hitbox _hitbox, ICollisionObject _otherObj)
        {
            Dictionary<Direction, CollisionSource> _dict = new Dictionary<Direction, CollisionSource>();
            if (_hitbox.CollidesWithObject(_otherObj))
            {
                if (!_otherObj.HasMultipleHitboxes)
                    CollisionDetection(ref _movement, _hitbox, _otherObj.Hitbox)
                        .Foreach(_dir => _dict[_dir] = new CollisionSource(_otherObj));
                else
                    foreach (Hitbox b in _otherObj.Hitboxes)
                        CollisionDetection(ref _movement, _hitbox, b)
                            .Foreach(_dir => _dict[_dir] = new CollisionSource(_otherObj));
            }
            return _dict;
        }


        public static Direction CollisionDetectionWithSplittedMovement(ref Vector3 _movA, Hitbox _hitA, Hitbox _hitB)
        {
            Vector3[] _splits = splitVector(_movA);
            Direction _dir = Direction.None;

            Hitbox _tmpHit = _hitA;
            for (int i = 0; i < _splits.Length; i++)
            {
                _dir |= CollisionDetection(ref _splits[i], _tmpHit, _hitB);
                _tmpHit = new Hitbox(_tmpHit.X + _splits[i].X, _tmpHit.Y + _splits[i].Y, _tmpHit.Z + _splits[i].Z,
                    _tmpHit.Width, _tmpHit.Height, _tmpHit.Depth);
            }

            if (_splits.Length == 0)
                _movA = Vector3.Zero;
            else
                _movA = _splits.Aggregate((v1, v2) => v1 + v2);

            return _dir;
        }

        const float MAX_SPLIT_PART_LENGTH = 0.1f;
        private static Vector3[] splitVector(Vector3 v)
        {
            int _splitCountX = (int)Math.Ceiling(Math.Abs(v.X / MAX_SPLIT_PART_LENGTH));
            int _indexX = 0;
            int _splitCountY = (int)Math.Ceiling(Math.Abs(v.Y / MAX_SPLIT_PART_LENGTH));
            int _indexY = 0;
            int _splitCountZ = (int)Math.Ceiling(Math.Abs(v.Z / MAX_SPLIT_PART_LENGTH));
            int _indexZ = 0;
            Vector3[] _splits = new Vector3[_splitCountX + _splitCountY + _splitCountZ];

            for (int i = 0; i < _splits.Length; i++)
            {
                float _progressX = (_splitCountX > 0) ? _indexX / _splitCountX : float.PositiveInfinity;
                float _progressY = (_splitCountY > 0) ? _indexY / _splitCountY : float.PositiveInfinity;
                float _progressZ = (_splitCountZ > 0) ? _indexZ / _splitCountZ : float.PositiveInfinity;

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

        const float FLOATING_POINT_INCORRECTION = 0.00001f;
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
    }

    struct Hitbox
    {
        public readonly float X, Y, Z;
        public readonly float Width, Depth, Height;

        private static Predicate<WorldBlock> COLLIDES_WITH_ALL_WORLD_BLOCKS = (_) => true;
        private static Predicate<ICollisionObject> COLLIDES_WITH_ALL_OBJECTS = (_) => true;

        private Predicate<WorldBlock> collidesWithWorldBlock;
        private Predicate<ICollisionObject> collidesWithObject;

        public Hitbox(float x, float y, float z, float _width, float _height, float _depth) :
            this(x, y, z, _width, _height, _depth, COLLIDES_WITH_ALL_WORLD_BLOCKS, COLLIDES_WITH_ALL_OBJECTS)
        { }

        public Hitbox(float x, float y, float z, float _width, float _height, float _depth, Predicate<WorldBlock> _collidesWithWorldBlock, Predicate<ICollisionObject> _collidesWithObject)
        {
            Width = _width;
            Depth = _depth;
            Height = _height;
            X = x;
            Y = y;
            Z = z;
            collidesWithWorldBlock = _collidesWithWorldBlock;
            collidesWithObject = _collidesWithObject;
        }
        public Hitbox(Hitbox b, Predicate<WorldBlock> _collidesWithWorldBlock, Predicate<ICollisionObject> _collidesWithObject)
        {
            Width = b.Width;
            Depth = b.Depth;
            Height = b.Height;
            X = b.X;
            Y = b.Y;
            Z = b.Z;
            collidesWithObject = _collidesWithObject;
            collidesWithWorldBlock = _collidesWithWorldBlock;
        }

        public bool CollidesWithWorldBlock(WorldBlock _block) => collidesWithWorldBlock(_block);
        public bool CollidesWithObject(ICollisionObject _object) => collidesWithObject(_object);

        public Hitbox(Vector3 _position, Vector3 _size)
            : this(_position.X, _position.Y, _position.Z, _size.X, _size.Y, _size.Z)
        { }

        public Hitbox(float x, float y, float z, Vector3 _size)
            : this(x, y, z, _size.X, _size.Y, _size.Z) { }

        public Hitbox(Vector3 _position, float _width, float _height, float _depth)
            : this(_position.X, _position.Y, _position.Z, _width, _height, _depth) { }

        public static Hitbox operator +(Hitbox h, Vector3 v) =>
            new Hitbox(h.X + v.X, h.Y + v.Y, h.Z + v.Z, h.Width, h.Height, h.Depth,
                h.collidesWithWorldBlock, h.collidesWithObject);
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

    static class DirectionHelper
    {
        public static void Foreach(this Direction _dir, Action<Direction> _func)
        {
            if (_dir.HasFlag(Direction.Front)) _func(Direction.Front);
            if (_dir.HasFlag(Direction.Back)) _func(Direction.Back);
            if (_dir.HasFlag(Direction.Left)) _func(Direction.Left);
            if (_dir.HasFlag(Direction.Right)) _func(Direction.Right);
            if (_dir.HasFlag(Direction.Top)) _func(Direction.Top);
            if (_dir.HasFlag(Direction.Bottom)) _func(Direction.Bottom);
        }

        public static int SingleDirectionAsInt(this Direction _dir)
        {
            switch (_dir)
            {
                case Direction.Left:
                    return 0;
                case Direction.Right:
                    return 1;
                case Direction.Front:
                    return 2;
                case Direction.Back:
                    return 3;
                case Direction.Top:
                    return 4;
                case Direction.Bottom:
                    return 5;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
