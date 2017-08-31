using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    interface IReadonlyCollisionDetector
    {
        CollisionDetector.CollisionInfo CheckCollision(ref Vector3 _movement, ICollisionObject _object);
    }

    interface ICollisionDetector : IReadonlyCollisionDetector
    {
        void AddObject(ICollisionObject _object);
        void RemoveObject(ICollisionObject _object);
    }

    class CollisionDetector : ICollisionDetector
    {
        private List<ICollisionObject> objects = new List<ICollisionObject>();
        private IBlockWorld world;
        private List<ICollisionObject>[,] chunkedObjects;
        private const int CHUNK_SIZE = 64;

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

        public class CollisionInfo : IEnumerable<KeyValuePair<Direction, CollisionSource>>
        {
            public CollisionSource? Left;
            public CollisionSource? Right;
            public CollisionSource? Top;
            public CollisionSource? Bottom;
            public CollisionSource? Front;
            public CollisionSource? Back;

            public CollisionInfo() { }

            public CollisionSource this[Direction d]
            {
                get
                {
                    switch (d)
                    {
                        case Direction.Left: return Left.Value;
                        case Direction.Right: return Right.Value;
                        case Direction.Top: return Top.Value;
                        case Direction.Bottom: return Bottom.Value;
                        case Direction.Front: return Front.Value;
                        case Direction.Back: return Back.Value;
                        default: throw new ArgumentException();
                    }
                }
                set
                {
                    switch (d)
                    {
                        case Direction.Left:
                            Left = value;
                            break;
                        case Direction.Right:
                            Right = value;
                            break;
                        case Direction.Top:
                            Top = value;
                            break;
                        case Direction.Bottom:
                            Bottom = value;
                            break;
                        case Direction.Front:
                            Front = value;
                            break;
                        case Direction.Back:
                            Back = value;
                            break;
                    }
                }
            }

            public bool Any()
            {
                return Left != null | Right != null | Top != null | Bottom != null | Front != null | Back != null;
            }
            public bool ContainsKey(Direction _dir)
            {
                switch (_dir)
                {
                    case Direction.Left:
                        return Left.HasValue;
                    case Direction.Right:
                        return Right.HasValue;
                    case Direction.Top:
                        return Top.HasValue;
                    case Direction.Bottom:
                        return Bottom.HasValue;
                    case Direction.Front:
                        return Front.HasValue;
                    case Direction.Back:
                        return Back.HasValue;
                    default: throw new ArgumentException();
                }
            }

            public IEnumerator<KeyValuePair<Direction, CollisionSource>> GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public class Enumerator : IEnumerator<KeyValuePair<Direction, CollisionSource>>
            {
                private readonly CollisionInfo info;
                private int i = -1;

                public Enumerator(CollisionInfo _info)
                {
                    info = _info;
                }

                public KeyValuePair<Direction, CollisionSource> Current { get; private set; }
                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    i++;
                    if (i >= 6) return false;
                    Direction _dir = DirectionHelper.AsDirection(i);
                    while (!info.ContainsKey(_dir))
                    {
                        i++;
                        if (i >= 6) return false;
                        _dir = DirectionHelper.AsDirection(i);
                    }
                    Current = new KeyValuePair<Direction, CollisionSource>(_dir, info[_dir]);
                    return true;
                }

                public void Reset()
                {
                    i = -1;
                }
            }
        }

        public void Update()
        {
            chunkedObjects = new List<ICollisionObject>[CHUNK_SIZE, CHUNK_SIZE];

            int worldX = world.Size.X;
            int worldY = world.Size.Z;

            foreach (ICollisionObject _obj in objects)
            {
                if (!_obj.IsEnabled)
                    continue;

                Hitbox _hitbox;
                if (_obj.HasMultipleHitboxes)
                    _hitbox = _obj.Hitboxes[0];
                else
                    _hitbox = _obj.Hitbox;

                int x = (int)_hitbox.X * CHUNK_SIZE / worldX;
                int y = (int)_hitbox.Z * CHUNK_SIZE / worldY;

                if (y >= 0 & y < CHUNK_SIZE &
                    x >= 0 & x < CHUNK_SIZE)
                {
                    if (chunkedObjects[x, y] == null)
                        chunkedObjects[x, y] = new List<ICollisionObject>();
                    chunkedObjects[x, y].Add(_obj);
                }
            }
        }


        public CollisionInfo CheckCollision(ref Vector3 _movement, ICollisionObject _object)
        {
            Vector3[] _splits = splitVector(_movement);
            CollisionInfo _collInfo = new CollisionInfo();

            int worldX = world.Size.X;
            int worldY = world.Size.Z;

            if (!_object.HasMultipleHitboxes)
            {
                Hitbox _tmpHit = _object.Hitbox;
                for (int i = 0; i < _splits.Length; i++)
                {
                    checkCollisionWithWorld(ref _splits[i], _tmpHit, world, _collInfo);

                    int chkX = (int)_tmpHit.X * CHUNK_SIZE / worldX;
                    int chkY = (int)_tmpHit.Z * CHUNK_SIZE / worldY;

                    for (int x = Math.Max(0, chkX - 1); x <= Math.Min(CHUNK_SIZE - 1, chkX + 1); x++)
                        for (int y = Math.Max(0, chkY - 1); y <= Math.Min(CHUNK_SIZE - 1, chkY + 1); y++)
                        {
                            var chk = chunkedObjects[x, y];
                            if (chk != null)
                                foreach (ICollisionObject _otherObj in chk)
                                    if (_otherObj != _object && _otherObj.IsEnabled)
                                        checkCollisionWithObject(ref _splits[i], _tmpHit, _otherObj, _collInfo);
                        }

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
                        checkCollisionWithWorld(ref _splits[i], _tmpHitboxes[j], world, _collInfo);
                        foreach (ICollisionObject _otherObj in objects)
                            if (_otherObj != _object && _otherObj.IsEnabled)
                                checkCollisionWithObject(ref _splits[i], _tmpHitboxes[j], _otherObj, _collInfo);
                    }

                    for (int j = 0; j < _tmpHitboxes.Length; j++)
                        _tmpHitboxes[j] += _splits[i];
                }
            }

            if (_splits.Any())
                _movement = _splits.Aggregate((v1, v2) => v1 + v2);

            return _collInfo;
        }

        private void checkCollisionWithWorld(ref Vector3 _movement, Hitbox _hitbox, IReadonlyBlockWorld _world, CollisionInfo _info)
        {
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
                                _dir.Foreach(d => _info[d] = new CollisionSource(b));
                        }
                    }
        }

        private void checkCollisionWithObject(ref Vector3 _movement, Hitbox _hitbox, ICollisionObject _otherObj, CollisionInfo _collInfo)
        {
            if (_hitbox.CollidesWithObject(_otherObj))
            {
                if (!_otherObj.HasMultipleHitboxes)
                {
                    Hitbox _otherHitbox = _otherObj.Hitbox;
                    Direction _dir = CollisionDetection(ref _movement, _hitbox, _otherHitbox);
                    _dir.Foreach(d => _collInfo[d] = new CollisionSource(_otherObj));
                }
                else
                    foreach (Hitbox b in _otherObj.Hitboxes)
                        CollisionDetection(ref _movement, _hitbox, b)
                            .Foreach(_dir => _collInfo[_dir] = new CollisionSource(_otherObj));
            }
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

        const float FLOATING_POINT_INCORRECTION = 0.001f;
        /// <summary>
        /// Erkenne Kollisionen zwischen einer bewegten und einer statischen Hitbox
        /// </summary>
        /// <param name="_movA">Die Referenz des Bewegungsvektors der von der Funktion auf mögliche Werte begrenzt wird</param>
        /// <param name="_hitA">Die Hitbox des bewegten Objektes</param>
        /// <param name="_hitB">Die Hitbox des statischen Objektes</param>
        /// <param name="_possibleMovements">Mögliche Ausweichbewegungen des bewegten Hitbox</param>
        /// <returns>Flags, die die Seiten der bewegenden Hitbox angeben, welche mit der statischen Hitbox kollidieren</returns>
        public static int COUNTER = 0;
        private static Direction CollisionDetection(ref Vector3 _movA, Hitbox _hitA, Hitbox _hitB)
        {
            COUNTER++;
            Direction _collInfo = Direction.None;

            float xDiff = 0, yDiff = 0, zDiff = 0;
            bool xCollFlag = false, yCollFlag = false, zCollFlag = false;

            Vector3 _movement = _movA;

            // collision left
            if (_hitA.X + _movA.X < _hitB.X + _hitB.Width - FLOATING_POINT_INCORRECTION &&
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
            if (_hitA.Y + _movA.Y < _hitB.Y + _hitB.Height - FLOATING_POINT_INCORRECTION &&
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
            if (_hitA.Z + _movA.Z < _hitB.Z + _hitB.Depth - FLOATING_POINT_INCORRECTION &&
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
                    if (Math.Abs(_movA.Y) > Math.Abs(_movement.Y) && Math.Abs(_movA.Y) > 1)
                        _movA.Y = _movement.Y;
                    return _collInfo & (Direction.Top | Direction.Bottom);
                }
                else if (_collInfo.HasFlag(Direction.Left) | _collInfo.HasFlag(Direction.Right))
                {
                    _movA.X += xDiff;
                    if (Math.Abs(_movA.X) > Math.Abs(_movement.X) && Math.Abs(_movA.X) > 1)
                        _movA.X = _movement.X;
                    return _collInfo & (Direction.Left | Direction.Right);
                }
                else if (_collInfo.HasFlag(Direction.Front) | _collInfo.HasFlag(Direction.Back))
                {
                    _movA.Z += zDiff;
                    if (Math.Abs(_movA.Z) > Math.Abs(_movement.Z) && Math.Abs(_movA.Z) > 1)
                        _movA.Z = _movement.Z;
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

        private readonly Type type;

        //private static Predicate<WorldBlock> COLLIDES_WITH_ALL_WORLD_BLOCKS = (_) => true;
        //private static Predicate<ICollisionObject> COLLIDES_WITH_ALL_OBJECTS = (_) => true;

        //private Predicate<WorldBlock> collidesWithWorldBlock;
        //private Predicate<ICollisionObject> collidesWithObject;

        public Hitbox(float x, float y, float z, float _width, float _height, float _depth, Type _type = Type.Default)
        {
            Width = _width;
            Depth = _depth;
            Height = _height;
            X = x;
            Y = y;
            Z = z;
            type = _type;
        }
        public Hitbox(Hitbox b, Type t)
        {
            Width = b.Width;
            Depth = b.Depth;
            Height = b.Height;
            X = b.X;
            Y = b.Y;
            Z = b.Z;
            type = t;
        }

        public bool CollidesWithWorldBlock(WorldBlock _block)
        {
            switch (type)
            {
                case Type.PlayerBlock:
                    return !_block.IsWater();
                default:
                    return true;
            }
        }
        public bool CollidesWithObject(ICollisionObject _object)
        {
            switch (type)
            {
                case Type.Enemy:
                    return !(_object is Geschoss);
                case Type.Geschoss:
                    return !(_object is Enemy);
                default:
                    return true;
            }
        }

        public Hitbox(Vector3 _position, Vector3 _size)
            : this(_position.X, _position.Y, _position.Z, _size.X, _size.Y, _size.Z)
        { }

        public Hitbox(float x, float y, float z, Vector3 _size)
            : this(x, y, z, _size.X, _size.Y, _size.Z) { }

        public Hitbox(Vector3 _position, float _width, float _height, float _depth)
            : this(_position.X, _position.Y, _position.Z, _width, _height, _depth) { }

        public static Hitbox operator +(Hitbox h, Vector3 v) =>
            new Hitbox(h.X + v.X, h.Y + v.Y, h.Z + v.Z, h.Width, h.Height, h.Depth, h.type);

        public enum Type
        {
            Default,
            Enemy,
            Geschoss,
            PlayerBlock
        }
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
            if ((_dir & Direction.Front) == Direction.Front) _func(Direction.Front);
            if ((_dir & Direction.Back) == Direction.Back) _func(Direction.Back);
            if ((_dir & Direction.Left) == Direction.Left) _func(Direction.Left);
            if ((_dir & Direction.Right) == Direction.Right) _func(Direction.Right);
            if ((_dir & Direction.Top) == Direction.Top) _func(Direction.Top);
            if ((_dir & Direction.Bottom) == Direction.Bottom) _func(Direction.Bottom);
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

        public static Direction AsDirection(this int _int)
        {
            switch (_int)
            {
                case 0:
                    return Direction.Left;
                case 1:
                    return Direction.Right;
                case 2:
                    return Direction.Front;
                case 3:
                    return Direction.Back;
                case 4:
                    return Direction.Top;
                case 5:
                    return Direction.Bottom;
                default:
                    throw new ArgumentException();

            }
        }
    }
}
