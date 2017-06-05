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

        void SetState(IReadOnlyDictionary<string, string[]> _state);
    }

    interface IReadonlyWorldObject : IRenderable, ICollisionObject, IPositionObject
    {
        IReadOnlyDictionary<string, string[]> GetState();
    }

    interface ISpecialBlock : IUpdateable, IRenderable
    {
        Vector3Int Position { get; }
        WorldBlock Type { get; }
    }
}
