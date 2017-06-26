using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    interface ICollisionObject
    {
        bool HasMultipleHitboxes { get; }
        Hitbox Hitbox { get; }
        Hitbox[] Hitboxes { get; }

        bool IsEnabled { get; }
    }

    class SimpleCollisionObject : ICollisionObject
    {
        public bool HasMultipleHitboxes { get; set; }

        public Hitbox Hitbox { get; set; }

        public Hitbox[] Hitboxes { get; set; }

        public bool IsEnabled { get; set; }

        public SimpleCollisionObject(Hitbox _hitbox)
        {
            Hitbox = _hitbox;
            HasMultipleHitboxes = false;
            IsEnabled = true;
        }

        public SimpleCollisionObject(Hitbox[] _hitboxes)
        {
            Hitboxes = _hitboxes;
            HasMultipleHitboxes = true;
            IsEnabled = true;
        }
    }
}
