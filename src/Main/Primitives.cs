using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    struct Vector3Int
    {
        public int X, Y, Z;

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3(Vector3Int _item) => new Vector3(_item.X, _item.Y, _item.Z);
        public static explicit operator Vector3Int(Vector3 _item) => new Vector3Int((int)_item.X, (int)_item.Y, (int)_item.Z);

        public static Vector3Int operator +(Vector3Int x, Vector3Int y) => new Vector3Int(x.X + y.X, x.Y + y.Y, x.Z + y.Z);
        public static Vector3Int operator *(Vector3Int x, Vector3Int y) => new Vector3Int(x.X * y.X, x.Y * y.Y, x.Z * y.Z);
        public static Vector3Int operator -(Vector3Int x) => new Vector3Int(-x.X, -x.Y, -x.Z);

        public static Vector3Int operator -(Vector3Int x, Vector3Int y) => x + -y;

        public static bool operator ==(Vector3Int x, Vector3Int y) => x.X == y.X & x.Y == y.Y & x.Z == y.Z;
        public static bool operator !=(Vector3Int x, Vector3Int y) => !(x.X == y.X & x.Y == y.Y & x.Z == y.Z);

        public override bool Equals(object obj)
        {
            if (obj is Vector3Int)
                return (Vector3Int)obj == this;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return unchecked(X + Y * 233 + Z * 107);
        }
        public override string ToString()
            => $"{X}-{Y}-{Z}";
        public static Vector3Int Parse(string s)
        {
            var _items = s.Split(';', '-').Select(s2 => int.Parse(s2.Trim())).GetEnumerator();
            _items.MoveNext();
            int x = _items.Current;
            _items.MoveNext();
            int y = _items.Current;
            _items.MoveNext();
            int z = _items.Current;
            return new Vector3Int(x, y, z);
        }
    }
    struct Vector2Int
    {
        public int X, Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(Vector2Int _item) => new Vector2(_item.X, _item.Y);
        public static explicit operator Vector2Int(Vector2 _item) => new Vector2Int((int)_item.X, (int)_item.Y);

        public static Vector2Int operator +(Vector2Int x, Vector2Int y) => new Vector2Int(x.X + y.X, x.Y + y.Y);
        public static Vector2Int operator -(Vector2Int x) => new Vector2Int(-x.X, -x.Y);

        public static Vector2Int operator -(Vector2Int x, Vector2Int y) => x + -y;

        public static bool operator ==(Vector2Int x, Vector2Int y) => x.X == y.X & x.Y == y.Y;
        public static bool operator !=(Vector2Int x, Vector2Int y) => !(x.X == y.X & x.Y == y.Y);

        public override string ToString()
            => $"{X}-{Y}";

        public static Vector2Int Parse(string s)
        {
            var _items = s.Split(';', '-').Select(s2 => int.Parse(s2.Trim())).GetEnumerator();
            _items.MoveNext();
            int x = _items.Current;
            _items.MoveNext();
            int y = _items.Current;
            return new Vector2Int(x, y);
        }
    }
}
