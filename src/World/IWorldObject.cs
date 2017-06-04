using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IWorldObject : IUpdateable, IReadonlyWorldObject
    {
        void Activate();
        void Deactivate();
    }

    interface IReadonlyWorldObject : IRenderable, ICollisionObject, IPositionObject
    { }

    interface ISpecialBlock : IUpdateable, IRenderable
    {
        Vector3Int Position { get; }
        WorldBlock Type { get; }
    }
}
