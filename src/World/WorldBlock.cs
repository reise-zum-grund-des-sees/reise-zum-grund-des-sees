using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    enum WorldBlock : byte
    {
        None,
        Wall, // add more wall types?
        InvisibleWall,
        Water1,
        Water2,
        Water3,
        Water4,
        Lever,
        Spikes,
        PressurePlateUp,
        PressurePlateDown,
        Water4Infinite,
        Unknown
    }

    static class WorldBlockHelper
    {
        public static bool IsSpecialBlock(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Lever:
                case WorldBlock.Spikes:
                case WorldBlock.PressurePlateUp:
                case WorldBlock.PressurePlateDown:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFullBlock(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                case WorldBlock.Lever:
                case WorldBlock.Wall:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetWaterLevel(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.None: return 0;
                case WorldBlock.Water1: return 1;
                case WorldBlock.Water2: return 2;
                case WorldBlock.Water3: return 3;
                case WorldBlock.Water4: return 4;
                case WorldBlock.Water4Infinite: return 100;
                default: throw new ArgumentException();
            }
        }

        public static WorldBlock GetWaterFromLevel(int _level)
        {
            switch (_level)
            {
                case 0: return WorldBlock.None;
                case 1: return WorldBlock.Water1;
                case 2: return WorldBlock.Water2;
                case 3: return WorldBlock.Water3;
                default: return WorldBlock.Water4;
            }
        }

        public static ISpecialBlock Instanciate(this WorldBlock b, Vector3Int _position)
        {
            switch (b)
            {
                case WorldBlock.Lever:
                    return new Lever(_position);
                case WorldBlock.Spikes:
                    return new Spike(_position);
                case WorldBlock.PressurePlateUp:
                    return new PressurePlate(_position, 0);
                case WorldBlock.PressurePlateDown:
                    return new PressurePlate(_position, 1);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Vector3 GetBounds(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                case WorldBlock.InvisibleWall:
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                case WorldBlock.Unknown:
                    return new Vector3(1.00f, 1.00f, 1.00f);
                case WorldBlock.Water3:
                    return new Vector3(1.00f, 0.75f, 1.00f);
                case WorldBlock.Water2:
                    return new Vector3(1.00f, 0.50f, 1.00f);
                case WorldBlock.Water1:
                    return new Vector3(1.00f, 0.25f, 1.00f);
                case WorldBlock.Spikes:
                    return new Vector3(1.00f, 0.5f, 1.00f);
                case WorldBlock.Lever:
                    return new Vector3(1f, 1.00f, 1f);
                case WorldBlock.PressurePlateUp:
                    return new Vector3(1f, 0.5f, 1f);
                case WorldBlock.PressurePlateDown:
                    return new Vector3(1f, 0.1f, 1f);
                default:
                    throw new ArgumentException($"{b} has no Bounds");
            }
        }

        public static Vector2[] GetTextureOffsets(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                    return new Vector2[] {
                            new Vector2(0.0f, 0 / 3f),
                            new Vector2(0.5f, 0 / 3f),
                            new Vector2(0.0f, 1 / 3f),
                            new Vector2(0.5f, 1 / 3f),
                            new Vector2(0.0f, 2 / 3f)
                            //new Vector2(0.5f, 2 / 3f)
                    };
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                case WorldBlock.Water3:
                case WorldBlock.Water2:
                case WorldBlock.Water1:
                    return new Vector2[] {
                            new Vector2(0.5f, 2 / 3f)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool HasCollision(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                case WorldBlock.InvisibleWall:
                case WorldBlock.Spikes:
                case WorldBlock.Lever:
                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                case WorldBlock.Unknown:
                case WorldBlock.PressurePlateUp:
                case WorldBlock.PressurePlateDown:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsVisible(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Wall:
                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsWater(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.Water1:
                case WorldBlock.Water2:
                case WorldBlock.Water3:
                case WorldBlock.Water4:
                case WorldBlock.Water4Infinite:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsPressurePlate(this WorldBlock b)
        {
            switch (b)
            {
                case WorldBlock.PressurePlateUp:
                case WorldBlock.PressurePlateDown:
                    return true;
                default:
                    return false;
            }
        }
    }
}
