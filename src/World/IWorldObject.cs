using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IWorldObject : IUpdateable, IReadonlyWorldObject, IRenderable, IStateObject
    {
    }

    interface IReadonlyWorldObject : ICollisionObject, IReadonlyPositionObject
    {
    }

    interface ISpecialBlock : IUpdateable, IRenderable, IStateObject
    {
        Vector3Int Position { get; }
        WorldBlock Type { get; }
    }

    static class SpecialBlock
    {
        public static ISpecialBlock Instanciate(ConfigFile.ConfigNode _node, ObjectIDMapper _idMapper)
        {
            switch (_node.Items["type"])
            {
                case "ReiseZumGrundDesSees.Lever":
                    Lever l = new Lever(_node, _idMapper);
                    return l;
                case "ReiseZumGrundDesSees.Spike":
                    Spike s = new Spike(_node, _idMapper);
                    return s;
                case "ReiseZumGrundDesSees.PressurePlate":
                    PressurePlate pp = new PressurePlate(_node, _idMapper);
                    return pp;
                case "ReiseZumGrundDesSees.SaveBlock":
                    SaveBlock sb = new SaveBlock(_node, _idMapper);
                    return sb;
                default:
                    throw new ArgumentException();
            }
        }
    }

    static class WorldObject
    {
        public static IWorldObject Instanciate(ConfigFile.ConfigNode _node, ObjectIDMapper _idMapper)
        {
            switch (_node.Items["type"])
            {
                case "ReiseZumGrundDesSees.MovingBlock":
                    MovingBlock b = new MovingBlock(_node, _idMapper);
                    return b;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
