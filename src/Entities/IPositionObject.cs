using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IPositionObject : IReadonlyPositionObject
    {
        new Vector3 Position { get; set; }
    }

    interface IMovingObject
    {
        Vector3 Velocity { get; }
    }

    interface IMoveable : IReadonlyPositionObject, ICollisionObject
    {
        void Move(Vector3 _movement, IReadonlyCollisionDetector _detector);
    }

    interface IReadonlyPositionObject
    {
        Vector3 Position { get; }
    }
}
