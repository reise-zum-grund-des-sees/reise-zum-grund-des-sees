using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    interface IPlayer : IUpdateable, IHitable, IReadonlyPlayer//, IRenderable
    {
        new IList<IPlayerBlock> Blocks { get; }

        void Hit();
    }

    interface IReadonlyPlayer : IPositionObject, ICollisionObject
    {
        IReadOnlyList<IReadonlyPlayerBlock> Blocks { get; }
        int Health { get; }
        double Healthcd { get; }
        float Blickrichtung { get; }
    }

    interface IPlayerBlock : IUpdateable, IReadonlyPlayerBlock//, IRenderable
    {
        //void PlaceInWorld(Vector3 _position);
    }

    interface IReadonlyPlayerBlock : ICollisionObject, IPositionObject
    {
        PlayerBlock.State CurrentState { get; }
        PlayerBlock.Type BlockType { get; }

        /// <summary>
        /// Die Prozentangabe des aktuellen Lebenszyklusses zwischen 0.0 und 1.0,
        /// wobei 0.0 ausdrückt, dass der Block grade erst Platziert wurde,
        /// und 1.0 angibt, dass der Block erneut platziert werden kann
        /// </summary>
        float LifetimePercentage { get; }
    }
}
