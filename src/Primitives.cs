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
        public static Vector3Int operator -(Vector3Int x) => new Vector3Int(-x.X, -x.Y, -x.Z);

        public static Vector3Int operator -(Vector3Int x, Vector3Int y) => x + -y;

        public static bool operator ==(Vector3Int x, Vector3Int y) => x.X == y.X & x.Y == y.Y & x.Z == y.Z;
        public static bool operator !=(Vector3Int x, Vector3Int y) => !(x.X == y.X & x.Y == y.Y & x.Z == y.Z);
    }
}
