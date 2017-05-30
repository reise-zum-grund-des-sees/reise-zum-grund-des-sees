using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    interface ICollisionObject
    {
        bool HasMultipleHitboxes { get; }
        Hitbox Hitbox { get; }
        Hitbox[] Hitboxes { get; }

        bool CollidesWithWorldBlock(WorldBlock _block);
        bool CollidesWithObject(ICollisionObject _object);
    }
}
