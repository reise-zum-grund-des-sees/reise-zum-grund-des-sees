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
        /// <summary>
        /// Erkenne Kollisionen zwischen einer bewegten und einer statischen Hitbox
        /// </summary>
        /// <param name="_movA">Die Referenz des Bewegungsvektors der von der Funktion auf mögliche Werte begrenzt wird</param>
        /// <param name="_hitA">Die Hitbox des bewegten Objektes</param>
        /// <param name="_hitB">Die Hitbox des statischen Objektes</param>
        /// <returns>Flags, die die Seiten der bewegenden Hitbox angeben, welche mit der statischen Hitbox kollidieren</returns>
        public static CollisionInformation CollisionWithObject(ref Vector3 _movA, Hitbox _hitA, Hitbox _hitB)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Erkenne Kollisionen zwischen einer Hitbox und der Welt
        /// </summary>
        /// <param name="_movment">Die Referenz des Bewegungsvektors der von der Funktion auf mögliche Werte begrenzt wird</param>
        /// <param name="_hitbox">Die Hitbox des bewegten Objektes</param>
        /// <param name="_world">Die Welt, auf der sich das Objekt befindet</param>
        /// <returns>Flags, die die Seiten der bewegenden Hitbox angeben, welche mit der Welt kollidieren</returns>
        public static CollisionInformation CollisionWithWorld(ref Vector3 _movment, Hitbox _hitbox, World _world)
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
