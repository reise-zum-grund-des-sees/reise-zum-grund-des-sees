using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    static class CollisionDetector
    {
        public static CollisionInformation CollisionDetection(ref Vector3 _movA, Hitbox _hitA, Hitbox _hitB)
        {
            throw new NotImplementedException();
        }
    }

    struct Hitbox
    {
        public readonly float X, Y, Z;
        public readonly float Width, Depth, Height;

        public Hitbox(float x, float y, float z, float _width, float _depth, float _height)
        {
            Width = _width;
            Depth = _depth;
            Height = _height;
            X = x;
            Y = y;
            Z = z;
        }
    }

    [Flags]
    enum CollisionInformation
    {
        None = 0,
        Left = 1,
        Right = 2,
        Front = 4,
        Back = 8,
        Top = 16,
        Bottom = 32
    }
}
