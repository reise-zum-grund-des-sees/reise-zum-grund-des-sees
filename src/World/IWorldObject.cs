using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IWorldObject : IUpdateable, IReadonlyWorldObject, IRenderable
    {
        void SetState(IReadOnlyDictionary<string, string[]> _state);
    }

    interface IReadonlyWorldObject : ICollisionObject, IReadonlyPositionObject
    {
        IReadOnlyDictionary<string, string[]> GetState();
    }

    interface ISpecialBlock : IUpdateable, IRenderable
    {
        Vector3Int Position { get; }
        WorldBlock Type { get; }
    }
}
