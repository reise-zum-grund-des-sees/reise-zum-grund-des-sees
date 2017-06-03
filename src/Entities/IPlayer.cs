using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    interface IPlayer : IUpdateable, IPositionObject, ICollisionObject//, IRenderable
    {
        IReadOnlyList<IPlayerBlock> Blocks { get; }

        int Health { get; }
        double Healthcd { get; }
        float Blickrichtung { get; }
    }

    interface IPlayerBlock : IUpdateable, IPositionObject//, IRenderable
    {
        PlayerBlock.State CurrentState { get; }
        PlayerBlock.Type BlockType { get; }
        Hitbox Hitbox { get; }
        /// <summary>
        /// Die Prozentangabe des aktuellen Lebenszyklusses zwischen 0.0 und 1.0,
        /// wobei 0.0 ausdrückt, dass der Block grade erst Platziert wurde,
        /// und 1.0 angibt, dass der Block erneut platziert werden kann
        /// </summary>
        float LifetimePercentage { get; }
    }
}
