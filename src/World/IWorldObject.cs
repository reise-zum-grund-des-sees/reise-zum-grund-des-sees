using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IWorldObject : IUpdateable, IRenderable
    {
        Hitbox Hitbox { get; }
        Vector3Int Position { get; }
        WorldBlock Type { get; }
    }
}
